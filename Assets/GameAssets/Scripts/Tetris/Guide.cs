using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Guide : MonoBehaviour
{
    [SerializeField] private Block blockTemplate;
    [SerializeField] private Transform guideLine;
    List<Block> blocks = new List<Block>();

    private PieceTemplate pieceTemplate;
    EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();

    private void OnEnable()
    {
        eventBrokerComponent.Subscribe<TetrisEvents.UpdateGuideWindow>(UpdateGuideWindowHandler);
    }

    private void OnDisable()
    {
        eventBrokerComponent.Unsubscribe<TetrisEvents.UpdateGuideWindow>(UpdateGuideWindowHandler);
    }

    private void FixedUpdate()
    {
        if (pieceTemplate == null) return;
        Vector2Int position = MapPlayerLocation();

        Vector2Int constrainedPosition = position;
        if (constrainedPosition.y + pieceTemplate.blockRightOfCenter >= TetrisConstants.COLS)
        {
            constrainedPosition.y -= Mathf.Abs(TetrisConstants.COLS - (constrainedPosition.y + pieceTemplate.blockRightOfCenter)) + 1;
        }
        else if (constrainedPosition.y - pieceTemplate.blocksLeftOfCenter < 0)
        {
            constrainedPosition.y = pieceTemplate.blocksLeftOfCenter;
        }
        guideLine.localPosition = new Vector3(position.y * TetrisConstants.BLOCK_SIZE, guideLine.transform.localPosition.y, guideLine.transform.localPosition.z);
        transform.localPosition = new Vector3((constrainedPosition.y - pieceTemplate.pieceCenter.y) * TetrisConstants.BLOCK_SIZE, transform.localPosition.y, transform.localPosition.z);
    }

    private void UpdateGuideWindowHandler(BrokerEvent<TetrisEvents.UpdateGuideWindow> inEvent)
    {
        foreach (Block block in blocks)
        {
            Destroy(block.gameObject);
        }

        if (inEvent.Payload.PieceTemplate == null) return;
        pieceTemplate = inEvent.Payload.PieceTemplate;
        blocks = pieceTemplate.SpawnTemplate(blockTemplate, Vector2Int.zero, transform, true, 2);
        foreach (Block block in blocks)
        {
            block.transform.localPosition += (Vector3)pieceTemplate.guidePreviewOffset * TetrisConstants.BLOCK_SIZE;
        }
    }

    private Vector2Int MapPlayerLocation()
    {
        PlayerEvents.GetPlayerWorldLocation e = new PlayerEvents.GetPlayerWorldLocation();
        eventBrokerComponent.Publish(this, e);
        Vector3 playerWorldPosition = e.WorldPosition;

        float playerXPosition = playerWorldPosition.x;
        float originXPosition = transform.parent.position.x;

        int adjustedPosition = Mathf.RoundToInt((playerXPosition - originXPosition) / TetrisConstants.BLOCK_SIZE);
        return new Vector2Int(0, adjustedPosition);
    }
}
