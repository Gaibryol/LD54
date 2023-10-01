using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TetrisPieces
{
    public PieceTemplate[] LReversePieces;
    public PieceTemplate[] LPieces;
    public PieceTemplate[] IPieces;
    public PieceTemplate[] ZReversePieces;
    public PieceTemplate[] ZPieces;
    public PieceTemplate[] TPieces;
    public PieceTemplate[] SquarePieces;

    private List<int> spawningBag;

    public TetrisPieces()
    {
        FillSpawningBag();
    }

    public PieceTemplate[] GetRandomTemplateList()
    {
        if (spawningBag.Count == 0)
        {
            FillSpawningBag();
        }

		int index = UnityEngine.Random.Range(0, spawningBag.Count);
        int pieceType = spawningBag[index];
        spawningBag.RemoveAt(index);
        pieceType = 0;
        switch (pieceType)
        {
            case 0:
                return IPieces;
            case 1:
                return SquarePieces;
            case 2:
                return LPieces;
            case 3:
                return ZReversePieces;
            case 4:
                return ZPieces;
            case 5:
                return TPieces;
            case 6:
                return LReversePieces;
            default:
                return null;
        }
    }

    private void FillSpawningBag()
    {
        spawningBag = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
    }
}
