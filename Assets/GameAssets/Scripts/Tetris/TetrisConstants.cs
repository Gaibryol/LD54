using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TetrisConstants
{
    public const float SPAWN_INTERVAL = 5f;
    public const int DEATH_HEIGHT = 3;
    public const float BLOCK_SIZE = 0.333f;
    public const float TICK_RATE = .2f;

    public const int ROWS = 24;
    public const int COLS = 10;

    public enum GameState { Waiting, Playing, Paused, Win, Lose }
}
