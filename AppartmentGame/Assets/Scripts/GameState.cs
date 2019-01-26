using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State { DIALOGUE, MINIGAME }

public static class GameState
{
    public static State state = State.DIALOGUE;
}
