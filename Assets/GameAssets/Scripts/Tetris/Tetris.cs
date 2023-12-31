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
    private PieceTemplate[] currentPiecesTemplates;
    #endregion

    #region Next
    private PieceTemplate[] nextPiecesTemplates;
    private PieceTemplate nextPieceToSpawn;
    #endregion

    #region Saved
    private PieceTemplate[] savedPiecesTemplates;
    private PieceTemplate savedPiece;
    private bool isSwappedPiece;
    #endregion

    private List<Block> allBlocks;

    private bool playing;
    private bool updatingBoard;

	private int numCombos;

    private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();

    void Start()
    {
        GetNextPiece(true);
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
        StopAllCoroutines();
        playing = false;

        foreach (Block block in allBlocks)
        {
            if (block != null)
                Destroy(block.gameObject);
        }
        activeBlocks.Clear();
        currentTemplate = null;
        currentPiecesTemplates = null;

        GetNextPiece(true);

        savedPiecesTemplates = null;
        savedPiece = null;
        isSwappedPiece = false;

        eventBrokerComponent.Publish(this, new TetrisEvents.UpdatePreviewWindow(nextPieceToSpawn));
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdateGuideWindow(nextPieceToSpawn));
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdateSavedWindow(savedPiece));
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
        updatingBoard = false;
        isSwappedPiece = false;

		numCombos = 0;

        StartCoroutine(TickPiece());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SavePiece();
        }
    }

    private void FixedUpdate()
    {
        if (!playing || updatingBoard) return;
        ClearLines();
    }

    #region Saving
    private void SavePiece()
    {
        if (!playing) return;
        if (activeBlocks.Count == 0) return; // There is no active piece to swap
        if (updatingBoard) return;
        if (isSwappedPiece) return;

        isSwappedPiece = true;
        if (savedPiece == null) // add current piece to saved, spawn next piece on next tick
        {
            savedPiece = currentTemplate;
            savedPiecesTemplates = currentPiecesTemplates;
            currentPiecesTemplates = nextPiecesTemplates;
            currentTemplate = nextPieceToSpawn;
            GetNextPiece();
            
        } else
        {
            // there exists a saved piece, swap with active piece and spawn
            PieceTemplate temp = savedPiece;
            PieceTemplate[] tempTemplates = savedPiecesTemplates;

            savedPiece = currentTemplate;
            savedPiecesTemplates = currentPiecesTemplates;
            currentTemplate = temp;
            currentPiecesTemplates = tempTemplates;

        }


        DestoryActivePiece();
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdatePreviewWindow(nextPieceToSpawn));
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdateGuideWindow(nextPieceToSpawn));
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdateSavedWindow(savedPiece));
    }

    private void DestoryActivePiece()
    {
        foreach (Block block in activeBlocks)
        {
            Destroy(block.gameObject);
        }
        activeBlocks.Clear();
    }
    #endregion

    #region Preview
    private void GetNextPiece(bool isStarting = false)
    {
        nextPiecesTemplates = tetrisPieces.GetRandomTemplateList(isStarting);
        nextPieceToSpawn = nextPiecesTemplates[Random.Range(0, nextPiecesTemplates.Length)];
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdatePreviewWindow(nextPieceToSpawn));
    }

    private void RotatePreviewBlockHandler(BrokerEvent<TetrisEvents.RotatePreviewBlock> inEvent)
    {
        RotatePreviewPiece(inEvent.Payload.Clockwise);
    }

    public void RotatePreviewPiece(bool clockwise)
    {
        int currentIndex = nextPiecesTemplates.ToList().IndexOf(nextPieceToSpawn);
        int nextIndex = clockwise ? currentIndex + 1 : currentIndex - 1;
        if (nextIndex < 0)
        {
            nextIndex = nextPiecesTemplates.Length - 1;
        }
        else if (nextIndex >= nextPiecesTemplates.Length)
        {
            nextIndex = 0;
        }
        nextPieceToSpawn = nextPiecesTemplates[nextIndex];
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdatePreviewWindow(nextPieceToSpawn));
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdateGuideWindow(nextPieceToSpawn));
    }

    private void UpdateGuideWindow()
    {
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdateGuideWindow(nextPieceToSpawn));
    }
    #endregion

    #region Spawning
    private int counter = 0;    // TEMP DEBUG PURPOSES
    private void SpawnPiece()
    {
        if (!isSwappedPiece)
        {
            currentTemplate = nextPieceToSpawn;
            currentPiecesTemplates = nextPiecesTemplates;
        }
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
            isSwappedPiece = false;
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
				yield return new WaitForSeconds(tickRate);
                SpawnPiece();
				eventBrokerComponent.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.PieceDrop));
                tickRate = Mathf.Clamp(tickRate - TetrisConstants.TICK_RATE_DECREASE_AMOUNT, TetrisConstants.MIN_TICK_RATE, TetrisConstants.TICK_RATE);
                if (!IsValidSpawn() || !BlockPassedThreshold())
                {
					eventBrokerComponent.Publish(this, new GameStateEvents.EndGame());
					eventBrokerComponent.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Death));
					break;
                }
                if (!isSwappedPiece)
                {
                    GetNextPiece();
                }
                UpdateGuideWindow();
            } else
            {
                MovePiece();
            }
            
            yield return new WaitForSeconds(tickRate);
        }
    }

}
