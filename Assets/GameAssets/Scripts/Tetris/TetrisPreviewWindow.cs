using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPreviewWindow : MonoBehaviour
{
    [SerializeField] private Vector2Int WindowLocation;
    [SerializeField] private Block blockTemplate;
    [SerializeField] private bool isSavedWindow;

    private List<Block> blocks = new List<Block>();

    private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();

    private void OnEnable()
    {
        if (isSavedWindow)
        {
            eventBrokerComponent.Subscribe<TetrisEvents.UpdateSavedWindow>(UpdateSavedWindowHandler);
        } else
        {
            eventBrokerComponent.Subscribe<TetrisEvents.UpdatePreviewWindow>(UpdatePreviewWindowHandler);
        }
    }


    private void OnDisable()
    {
        if (isSavedWindow)
        {
            eventBrokerComponent.Unsubscribe<TetrisEvents.UpdateSavedWindow>(UpdateSavedWindowHandler);
        } else 
        {
            eventBrokerComponent.Unsubscribe<TetrisEvents.UpdatePreviewWindow>(UpdatePreviewWindowHandler);
        }
    }
    private void UpdateSavedWindowHandler(BrokerEvent<TetrisEvents.UpdateSavedWindow> inEvent)
    {
        UpdatePreviewWindow(inEvent.Payload.PieceTemplate);
    }

    private void UpdatePreviewWindowHandler(BrokerEvent<TetrisEvents.UpdatePreviewWindow> inEvent)
    {
        UpdatePreviewWindow(inEvent.Payload.PieceTemplate);
    }

    public void UpdatePreviewWindow(PieceTemplate pieceTemplate)
    {
        DestoryOldPreview();
        blocks = pieceTemplate.SpawnTemplate(blockTemplate, WindowLocation, transform, false);
        foreach (Block block in blocks)
        {
            block.transform.localPosition += (Vector3)pieceTemplate.previewPieceOffset * TetrisConstants.BLOCK_SIZE;
        }
    }

    private void DestoryOldPreview()
    {
        foreach (Block block in blocks)
        {
            Destroy(block.gameObject);
        }
    }
}
