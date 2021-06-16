using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventsManager : MonoBehaviour
{
    public static EventsManager current;

    public enum GameState { Start, Win, GameOver}

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        DontDestroyOnLoad(gameObject);
    }

    public event Action<GameState> onGameStateChangeTrigger;
    public void GameStateChangeTrigger(GameState state)
    {
        onGameStateChangeTrigger?.Invoke(state);
    }
}
