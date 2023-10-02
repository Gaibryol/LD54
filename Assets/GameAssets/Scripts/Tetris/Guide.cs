using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Guide : MonoBehaviour
{
    [SerializeField] private Block blockTemplate;
    [SerializeField] private Transform centerGuideLine;

    [SerializeField, Header("Prefabs")] private GameObject centerWithBorder;
    [SerializeField] private GameObject centerNoBorder;
    [SerializeField] private GameObject leftBorder;
    [SerializeField] private GameObject rightBorder;

    List<Block> blocks = new List<Block>();
    List<GameObject> spawnedGuideLines = new List<GameObject>();

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

        float xPos = position.y * TetrisConstants.BLOCK_SIZE - TetrisConstants.BLOCK_SIZE / 2 - TetrisConstants.BLOCK_SIZE * pieceTemplate.blocksLeftOfCenter;
        xPos = Mathf.Clamp(xPos, -TetrisConstants.BLOCK_SIZE / 2, (TetrisConstants.BLOCK_SIZE * TetrisConstants.COLS) - (TetrisConstants.BLOCK_SIZE / 2) - (TetrisConstants.BLOCK_SIZE * (spawnedGuideLines.Count)));
        centerGuideLine.localPosition = new Vector3(xPos, centerGuideLine.transform.localPosition.y, centerGuideLine.transform.localPosition.z);
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
        blocks = pieceTemplate.SpawnTemplate(blockTemplate, Vector2Int.zero, transform, true, 3);
        foreach (Block block in blocks)
        {
            block.transform.localPosition += (Vector3)pieceTemplate.guidePreviewOffset * TetrisConstants.BLOCK_SIZE;
        }
        DestroyGuideLines();
        SpawnGuideLines();
    }

    private void DestroyGuideLines()
    {
        foreach (GameObject guide in spawnedGuideLines)
        {
            Destroy(guide);
        }
        spawnedGuideLines.Clear();
    }

    private void SpawnGuideLines()
    {
        int width = 1 + pieceTemplate.blocksLeftOfCenter + pieceTemplate.blockRightOfCenter;
        if (width == 1)
        {
            GameObject guideLine = Instantiate(centerWithBorder, centerGuideLine);
            guideLine.transform.localPosition = new Vector3(TetrisConstants.BLOCK_SIZE/2, 0, 0);
            spawnedGuideLines.Add(guideLine);
            return;
        }

        GameObject guideLineLeft = Instantiate(leftBorder, centerGuideLine);
        guideLineLeft.transform.localPosition = new Vector3(TetrisConstants.BLOCK_SIZE/2, 0, 0);
        GameObject guideLineRight = Instantiate(rightBorder, centerGuideLine);
        guideLineRight.transform.localPosition = new Vector3(TetrisConstants.BLOCK_SIZE * (width - 1) + TetrisConstants.BLOCK_SIZE / 2, 0, 0);

        for (int i = 1; i < width - 1; i++)
        {
            GameObject guideLineMiddle = Instantiate(centerNoBorder, centerGuideLine);
            guideLineMiddle.transform.localPosition = new Vector3((TetrisConstants.BLOCK_SIZE) * i + TetrisConstants.BLOCK_SIZE / 2, 0, 0);
            spawnedGuideLines.Add(guideLineMiddle);
        }

        spawnedGuideLines.Add(guideLineLeft);
        spawnedGuideLines.Add(guideLineRight);
        
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
