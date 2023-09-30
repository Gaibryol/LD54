using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PieceTemplate : ScriptableObject
{
    [Serializable]
    public class Row
    {
        public bool[] cols = new bool[5];
    }

    [HideInInspector]
    public Row[] pieceDefinition = new Row[5];
    public Vector2Int pieceCenter;
    public int blocksLeftOfCenter; // Blocks from center
    public int blockRightOfCenter; // Blocks from center

    public List<Block> SpawnTemplate(Block blockTemplate, Vector2Int startingSpawnPosition, bool constrain = true)
    {
        if (constrain)
        {
            if (startingSpawnPosition.y + blockRightOfCenter >= TetrisConstants.COLS)
            {
                startingSpawnPosition.y -= Mathf.Abs(TetrisConstants.COLS - (startingSpawnPosition.y + blockRightOfCenter)) + 1;
            } else if (startingSpawnPosition.y - blocksLeftOfCenter < 0)
            {
                startingSpawnPosition.y = blocksLeftOfCenter;
            }
        }

        List<Block> blocks = new List<Block>();
        //Piece piece = Instantiate(pieceTemplate, startingSpawnPosition, Quaternion.identity);
        for (int i = 0; i < TetrisConstants.ROWS; i++)
        {
            for (int j = 0; j < TetrisConstants.COLS; j++)
            {
                if (!pieceDefinition[i].cols[j]) continue;

                Vector2Int localSpawnOffset = new Vector2Int(i, j) - pieceCenter;
                Vector2 localSpawnPosition = localSpawnOffset * TetrisConstants.BLOCK_SIZE;
                Vector2 worldSpawnPosition = startingSpawnPosition * TetrisConstants.BLOCK_SIZE + localSpawnPosition * new Vector2Int(-1, 1);
                worldSpawnPosition = new Vector2(worldSpawnPosition.y, worldSpawnPosition.x);
                Block block = Instantiate(blockTemplate, worldSpawnPosition, Quaternion.identity);
                block.localPosition = localSpawnOffset + startingSpawnPosition;
                blocks.Add(block);
            }
        }
        return blocks;
    }
}
