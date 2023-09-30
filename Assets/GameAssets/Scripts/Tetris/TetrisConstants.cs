using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TetrisConstants
{
    public const float SPAWN_INTERVAL = 5f;
    public const int SPAWN_HEIGHT = 20;
    public const int BLOCK_SIZE = 1;
    public const float TICK_RATE = .1f;

    public const int ROWS = 24;
    public const int COLS = 10;

    public enum GameState { Waiting, Playing, Paused, Win, Lose }
}
