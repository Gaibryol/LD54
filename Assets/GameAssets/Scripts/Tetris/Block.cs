using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector2Int localPosition;    // x: rows(0, 24); y: cols(0, 10)

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        name = UnityEngine.Random.Range(0, 100).ToString();
    }

    public Vector2Int DetermineNewPosition()
    {
        return localPosition + new Vector2Int(1, 0);
    }

    public Vector2Int GetCurrentPosition()
    {
        return localPosition;
    }

    public void AddPositionOffset(int offset=1)
    {
        localPosition += new Vector2Int(1*offset, 0);
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y - TetrisConstants.BLOCK_SIZE * offset, currentPosition.z);
        transform.position = newPosition;
    }

    public void SetPosition(Vector2Int position)
    {
        localPosition = position;
        Vector2Int worldposition = position * TetrisConstants.BLOCK_SIZE;
        transform.position = new Vector3(worldposition.y, worldposition.x * -1, transform.position.z);
    }
}
