using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisEvents
{
    public class UpdatePreviewWindow
    {
        public readonly PieceTemplate PieceTemplate;

        public UpdatePreviewWindow(PieceTemplate pieceTemplate)
        {
            PieceTemplate = pieceTemplate;
        }
    }
}
