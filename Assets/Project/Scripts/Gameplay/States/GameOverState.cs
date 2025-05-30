using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverState : MonoBehaviour, IGameState
{
    public void Enter(GameState currentState, GameState lastState)
    {
        GameEvents.Instance.TriggerGameEnded(GameManager.Instance.Score);
        AudioManager.Instance.StopBGM();
    }

    public void Update() { }
    public void HandleInput() { }
    public void Exit() { }
}
