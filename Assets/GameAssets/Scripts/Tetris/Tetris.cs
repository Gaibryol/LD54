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

    private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();
    void Start()
    {
        GetNextPiece();
        UpdateGuideWindow();
    }

    private void OnEnable()
    {
        eventBrokerComponent.Subscribe<TetrisEvents.RotatePreviewBlock>(RotatePreviewBlockHandler);
        eventBrokerComponent.Subscribe<GameStateEvents.StartGame>(StartGameHandler);
        eventBrokerComponent.Subscribe<GameStateEvents.EndGame>(EndGameHandler);
        eventBrokerComponent.Subscribe<GameStateEvents.RestartGame>(RestartGameHandler);
    }

    private void OnDisable()
    {
        eventBrokerComponent.Unsubscribe<TetrisEvents.RotatePreviewBlock>(RotatePreviewBlockHandler);
        eventBrokerComponent.Unsubscribe<GameStateEvents.StartGame>(StartGameHandler);
        eventBrokerComponent.Unsubscribe<GameStateEvents.EndGame>(EndGameHandler);
        eventBrokerComponent.Unsubscribe<GameStateEvents.RestartGame>(RestartGameHandler);
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
        StartGame();
    }

    private void StartGameHandler(BrokerEvent<GameStateEvents.StartGame> @event)
    {
        StartGame();
    }

    public void StartGame()
    {
        playspace = new Playspace();
        activeBlocks = new List<Block>();
        allBlocks = new List<Block>();
        playing = true;

        StartCoroutine(TickPiece());
    }

    private void FixedUpdate()
    {
        if (!playing) return;
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

            foreach (Block block in activeBlocks)
            {
                playspace.SetBoard(block.GetCurrentPosition(), block);
            }
            activeBlocks.Clear();
        }
    }
    #endregion

    #region Line clear
    private void ClearLines()
    {
        List<int> rows = playspace.ClearedLines();
        if (rows.Count == 0) return;
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

        ShiftBlocks(rows.Min());
    }

    private void ShiftBlocks(int startingRow)
    {
        for (int row = startingRow; row >= 0; row--)
        {
            foreach (Block block in playspace.GetAllBlocksInRow(row))
            {
                Vector2Int blockPosition = block.GetCurrentPosition();
                playspace.SetBoard(blockPosition, null);
                Vector2 newPosition = playspace.GetNextFreeSpaceInCol(blockPosition);
                block.AddPositionOffset((int)(newPosition.x - blockPosition.x));
                playspace.SetBoard(block.GetCurrentPosition(), block);
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
        while (playing)
        {
            if (activeBlocks.Count == 0)
            {
                SpawnPiece();
                if (!IsValidSpawn() || !BlockPassedThreshold())
                {
					eventBrokerComponent.Publish(this, new GameStateEvents.EndGame());
                    break;
                }
                GetNextPiece();
                UpdateGuideWindow();
            } else
            {
                MovePiece();
            }
            
            yield return new WaitForSeconds(TetrisConstants.TICK_RATE);
        }
    }

}
