using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPreviewWindow : MonoBehaviour
{
    [SerializeField] private Vector2Int WindowLocation;
    [SerializeField] private Block blockTemplate;

    private List<Block> blocks = new List<Block>();

    private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();

    private void OnEnable()
    {
        eventBrokerComponent.Subscribe<TetrisEvents.UpdatePreviewWindow>(UpdatePreviewWindowHandler);
    }

    private void OnDisable()
    {
        eventBrokerComponent.Unsubscribe<TetrisEvents.UpdatePreviewWindow>(UpdatePreviewWindowHandler);
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
            block.transform.localPosition += (Vector3)pieceTemplate.previewPieceOffset;
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
