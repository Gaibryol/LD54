using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Tetris : MonoBehaviour
{
    [HideInInspector]
    public Playspace playspace;
    [SerializeField] private TetrisPieces tetrisPieces;

    [SerializeField] private Block blockTemplate;
    [SerializeField] private GameObject LineClearTemplate;
    [SerializeField] private GameObject comboTemplate;

    #region Current 
    private List<Block> activeBlocks;
    private PieceTemplate currentTemplate;
    #endregion

    #region Next
    private PieceTemplate[] nextPiecesTemplates;
    private PieceTemplate pieceToSpawn;
    #endregion

    private List<Block> allBlocks;

    private bool playing;
    private bool updatingBoard;

	private int numCombos;

    private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();

    void Start()
    {
        GetNextPiece();
        UpdateGuideWindow();

		numCombos = 0;
    }

    private void OnEnable()
    {
        eventBrokerComponent.Subscribe<TetrisEvents.RotatePreviewBlock>(RotatePreviewBlockHandler);
        eventBrokerComponent.Subscribe<GameStateEvents.StartGame>(StartGameHandler);
        eventBrokerComponent.Subscribe<GameStateEvents.EndGame>(EndGameHandler);
        eventBrokerComponent.Subscribe<GameStateEvents.RestartGame>(RestartGameHandler);
		eventBrokerComponent.Subscribe<GameStateEvents.SecretEnding>(SecretEndingHandler);
    }

	private void OnDisable()
    {
        eventBrokerComponent.Unsubscribe<TetrisEvents.RotatePreviewBlock>(RotatePreviewBlockHandler);
        eventBrokerComponent.Unsubscribe<GameStateEvents.StartGame>(StartGameHandler);
        eventBrokerComponent.Unsubscribe<GameStateEvents.EndGame>(EndGameHandler);
        eventBrokerComponent.Unsubscribe<GameStateEvents.RestartGame>(RestartGameHandler);
		eventBrokerComponent.Unsubscribe<GameStateEvents.SecretEnding>(SecretEndingHandler);
	}

    private void EndGameHandler(BrokerEvent<GameStateEvents.EndGame> @event)
    {
        playing = false;
        StopAllCoroutines();
    }

    private void RestartGameHandler(BrokerEvent<GameStateEvents.RestartGame> @event)
    {
        foreach (Block block in  allBlocks)
        {
            Destroy(block.gameObject);
        }
    }

    private void StartGameHandler(BrokerEvent<GameStateEvents.StartGame> @event)
    {
        StartGame();
    }

	private void SecretEndingHandler(BrokerEvent<GameStateEvents.SecretEnding> inEvent)
	{
		playing = false;
		StopAllCoroutines();
	}

	public void StartGame()
    {
        playspace = new Playspace();
        activeBlocks = new List<Block>();
        allBlocks = new List<Block>();
        playing = true;

		numCombos = 0;

        StartCoroutine(TickPiece());
    }

    private void FixedUpdate()
    {
        if (!playing || updatingBoard) return;
        ClearLines();
    }

    #region Preview
    private void GetNextPiece()
    {
        nextPiecesTemplates = tetrisPieces.GetRandomTemplateList();
        pieceToSpawn = nextPiecesTemplates[Random.Range(0, nextPiecesTemplates.Length)];
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdatePreviewWindow(pieceToSpawn));
    }

    private void RotatePreviewBlockHandler(BrokerEvent<TetrisEvents.RotatePreviewBlock> inEvent)
    {
        RotatePreviewPiece(inEvent.Payload.Clockwise);
    }

    public void RotatePreviewPiece(bool clockwise)
    {
        int currentIndex = nextPiecesTemplates.ToList().IndexOf(pieceToSpawn);
        int nextIndex = clockwise ? currentIndex + 1 : currentIndex - 1;
        if (nextIndex < 0)
        {
            nextIndex = nextPiecesTemplates.Length - 1;
        }
        else if (nextIndex >= nextPiecesTemplates.Length)
        {
            nextIndex = 0;
        }
        pieceToSpawn = nextPiecesTemplates[nextIndex];
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdatePreviewWindow(pieceToSpawn));
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdateGuideWindow(pieceToSpawn));
    }

    private void UpdateGuideWindow()
    {
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdateGuideWindow(pieceToSpawn));
    }
    #endregion

    #region Spawning
    private int counter = 0;    // TEMP DEBUG PURPOSES
    private void SpawnPiece()
    {
        currentTemplate = pieceToSpawn;
        // TODO: Change Spawn location to player location

        //List<Block> newBlocks = currentTemplate.SpawnTemplate(blockTemplate, new Vector2Int(0, counter%TetrisConstants.COLS), transform);
        List<Block> newBlocks = currentTemplate.SpawnTemplate(blockTemplate, MapPlayerLocation(), transform);
        foreach (Block block in newBlocks)
        {
            block.AssignSprite(currentTemplate.glowBlockSprite, 1);
        }
        counter += 1;
        activeBlocks.AddRange(newBlocks);
        allBlocks.AddRange(newBlocks);
    }

    private Vector2Int MapPlayerLocation()
    {
        PlayerEvents.GetPlayerWorldLocation e = new PlayerEvents.GetPlayerWorldLocation();
        eventBrokerComponent.Publish(this, e);
        Vector3 playerWorldPosition = e.WorldPosition;

        float playerXPosition = playerWorldPosition.x;
        float originXPosition = transform.position.x;

        int adjustedPosition = Mathf.RoundToInt((playerXPosition - originXPosition)/TetrisConstants.BLOCK_SIZE);
        return new Vector2Int(0, adjustedPosition);
    }

    private bool IsValidSpawn()
    {
        foreach (Block block in activeBlocks)
        {
            if (!playspace.IsValidMove(block.GetCurrentPosition()))
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    #region Moving
    private void MovePiece()
    {
        bool valid = true;
        foreach (Block block in activeBlocks)
        {
            Vector2Int newPiecePosition = block.DetermineNewPosition();
            if (!playspace.IsValidMove(newPiecePosition))
            {
                valid = false;
                break;
            }
        }

        if (valid)
        {
            foreach (Block block in activeBlocks)
            {
                block.AddPositionOffset();
            }
        } else
        {
            updatingBoard = true;
            foreach (Block block in activeBlocks)
            {
                playspace.SetBoard(block.GetCurrentPosition(), block);
                block.AssignSprite(currentTemplate.blockSprite, 1);
                block.isMoving = false;
            }
            updatingBoard = false;
            activeBlocks.Clear();
			eventBrokerComponent.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.PieceLand));
		}
    }
    #endregion

    #region Line clear
    private void ClearLines()
    {
        List<int> rows = playspace.ClearedLines();
        if (rows.Count == 0) return;

        if (rows.Count >= 2)
        {
            int topRow = rows.Min();
            Vector3 localPosition = new Vector3(-8 * TetrisConstants.BLOCK_SIZE / 2, -topRow * TetrisConstants.BLOCK_SIZE);
            GameObject combo = Instantiate(comboTemplate, transform);
            combo.transform.localPosition = localPosition;
            eventBrokerComponent.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.ComboClear));
        } else
        {
			eventBrokerComponent.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.RowClear));
        }

        foreach (int row in rows)
        {
            GameObject lineClear = Instantiate(LineClearTemplate, transform);
            lineClear.transform.localPosition = new Vector3(TetrisConstants.COLS * TetrisConstants.BLOCK_SIZE / 2, -row * TetrisConstants.BLOCK_SIZE);
            foreach(Block block in playspace.GetAllBlocksInRow(row))
            {
                allBlocks.Remove(block);
                playspace.SetBoard(block.GetCurrentPosition(), null);
                Destroy(block.gameObject);
            }
		}

		eventBrokerComponent.Publish(this, new PlayerEvents.ClearLines(rows.Count));
		if (rows.Count >= 2)
		{
			numCombos += 1;

			if (numCombos >= Constants.Achievements.numTimesToCombo)
			{
				eventBrokerComponent.Publish(this, new AchievementEvents.EarnAchievement(Constants.Achievements.Combo20Times));
			}

			if (rows.Count == 4)
			{
				eventBrokerComponent.Publish(this, new AchievementEvents.EarnAchievement(Constants.Achievements.EarnBigCombo));
			}
		}

        ShiftBlocks(rows);
    }

    private void ShiftBlocks(List<int> rows)
    {
        foreach (int clearedRow in rows.OrderBy(row => row))
        {
            // shift everything "above" down
            for (int row = clearedRow - 1; row >= 0; row--)
            {
                foreach (Block block in playspace.GetAllBlocksInRow(row))
                {
                    Vector2Int blockPosition = block.GetCurrentPosition();
                    playspace.SetBoard(blockPosition, null);
                    block.isMoving = true;
                    block.AddPositionOffset();
                    //Vector2 newPosition = playspace.GetNextFreeSpaceInCol(blockPosition);
                    //block.AddPositionOffset((int)(newPosition.x - blockPosition.x));
                    playspace.SetBoard(block.GetCurrentPosition(), block);
                    block.isMoving = false;
                }
            }
        }
    }
    #endregion

    #region End Condition
    private bool BlockPassedThreshold()
    {
        for (int row = TetrisConstants.DEATH_HEIGHT; row >= 0; row--)
        {
            if (playspace.GetAllBlocksInRow(row).Exists(x => x != null))
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    private IEnumerator TickPiece()
    {
        float tickRate = TetrisConstants.TICK_RATE;
        while (playing)
        {
            if (activeBlocks.Count == 0)
            {
                SpawnPiece();
                tickRate = Mathf.Clamp(tickRate - TetrisConstants.TICK_RATE_DECREASE_AMOUNT, TetrisConstants.MIN_TICK_RATE, TetrisConstants.TICK_RATE);
                if (!IsValidSpawn() || !BlockPassedThreshold())
                {
					eventBrokerComponent.Publish(this, new GameStateEvents.EndGame());
					eventBrokerComponent.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Death));
					break;
                }
                GetNextPiece();
                UpdateGuideWindow();
            } else
            {
                MovePiece();
            }
            
            yield return new WaitForSeconds(tickRate);
        }
    }

}
