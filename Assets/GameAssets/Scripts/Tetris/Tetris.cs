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

    #region Current 
    private List<Block> activeBlocks;
    private PieceTemplate currentTemplate;
    #endregion

    #region Next
    private PieceTemplate[] nextPiecesTemplates;
    private PieceTemplate pieceToSpawn;
    #endregion

    private List<Block> allBlocks;

    private float gameTime;
    private TetrisConstants.GameState gameState;

    private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();
    void Start()
    {
        playspace = new Playspace();
        gameState = TetrisConstants.GameState.Waiting;
        StartGame();
    }

    public void StartGame()
    {
        gameTime = 0f;
        activeBlocks = new List<Block>();
        allBlocks = new List<Block>();
        gameState = TetrisConstants.GameState.Playing;

        GetNextPiece();

        StartCoroutine(TickPiece());
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != TetrisConstants.GameState.Playing) return;
        gameTime += Time.deltaTime;

        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            RotatePreviewPiece(true);
        }
    }

    #region Preview
    private void GetNextPiece()
    {
        nextPiecesTemplates = tetrisPieces.GetRandomTemplateList();
        pieceToSpawn = nextPiecesTemplates[Random.Range(0, nextPiecesTemplates.Length)];
        eventBrokerComponent.Publish(this, new TetrisEvents.UpdatePreviewWindow(pieceToSpawn));
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
    }
    #endregion

    #region Spawning
    private int counter = 0;    // TEMP DEBUG PURPOSES
    private void SpawnPiece()
    {
        currentTemplate = pieceToSpawn;
        // TODO: Change Spawn location to player location
        List<Block> newBlocks = currentTemplate.SpawnTemplate(blockTemplate, new Vector2Int(0, counter%TetrisConstants.COLS));
        counter += 1;
        activeBlocks.AddRange(newBlocks);
        allBlocks.AddRange(newBlocks);
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
                Vector2Int newPosition = playspace.GetNextFreeSpaceInCol(blockPosition);
                block.AddPositionOffset(newPosition.x - blockPosition.x);
                playspace.SetBoard(block.GetCurrentPosition(), block);
            }
        }
    }

    #endregion

    private IEnumerator TickPiece()
    {
        while (true)
        {
            if (activeBlocks.Count == 0)
            {
                SpawnPiece();
                if (!IsValidSpawn())
                {
                    Debug.Log("Game over");
                    break;
                }
                GetNextPiece();
            } else
            {
                MovePiece();
                ClearLines();
            }
            
            yield return new WaitForSeconds(TetrisConstants.TICK_RATE);
        }
    }

}
