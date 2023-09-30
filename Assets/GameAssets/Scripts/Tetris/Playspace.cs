using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Playspace
{
    [Serializable]
    public class Row
    {
        public Block[] cols = new Block[TetrisConstants.COLS];
    }

    [HideInInspector]
    public Row[] board = new Row[TetrisConstants.ROWS];

    //private bool[,] board;

    public Playspace()
    {
        board = new Row[TetrisConstants.ROWS];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = new Row();
        }
    }

    public bool IsValidMove(Vector2Int position)
    {
        if (position.x >= TetrisConstants.ROWS) return false;
        if (board[position.x].cols[position.y] != null)
        {
            return false;
        }
        return true;
    }

    public void SetBoard(Vector2Int position, Block block)
    {
        board[position.x].cols[position.y] = block;
    }

    public List<int> ClearedLines()
    {
        List<int> lines = new List<int>();
        for (int row = 0; row < TetrisConstants.ROWS; row++)
        {
            if (board[row].cols.All(x => x != null))
            {
                lines.Add(row);
            }
        }
        return lines;
    }

    public Vector2 GetNextFreeSpaceInCol(Vector2Int position)
    {
        Vector2Int freePosition = position;
        for (int row = freePosition.x + 1; row < TetrisConstants.ROWS; ++row)
        {
            if (board[row].cols[position.y] != null) return freePosition;
            freePosition.x = row;
        }
        return freePosition;
    }

    public List<Block> GetAllBlocksInRow(int row)
    {
        List<Block> blocks = new List<Block>();
        foreach (Block block in board[row].cols)
        {
            if (block != null)
            {
                blocks.Add(block);
            }
        }
        return blocks;
    }
}
