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

    public PieceTemplate[] GetRandomTemplateList()
    {
        int index = UnityEngine.Random.Range(0, 7);
        switch (index)
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
}
