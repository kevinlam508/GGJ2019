using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State { DIALOGUE, MINIGAME }
public enum MinigameResult { LOSE, WIN }

public static class GameState
{
    public static State state = State.DIALOGUE;
    public static MinigameResult res = MinigameResult.LOSE;
}
