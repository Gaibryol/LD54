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

    public class RotatePreviewBlock
    {
        public readonly bool Clockwise;

        public RotatePreviewBlock(bool clockwise)
        {
            Clockwise = clockwise;
        }
    }

    public class UpdateGuideWindow
    {
        public readonly PieceTemplate PieceTemplate;

        public UpdateGuideWindow(PieceTemplate pieceTemplate)
        {
            PieceTemplate = pieceTemplate;
        }
    }
}
