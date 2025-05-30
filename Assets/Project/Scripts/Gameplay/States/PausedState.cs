using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedState : MonoBehaviour, IGameState
{
    public void Enter(GameState currentState, GameState lastState)
    {
        GameEvents.Instance.TriggerGamePaused();
        AudioManager.Instance.PauseBGM();
    }

    public void Update() { }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameStateMachine.Instance.ChangeState(GameState.Playing);
        }
    }

    public void Exit()
    {
        GameEvents.Instance.TriggerGameResumed();
        AudioManager.Instance.ResumeBGM();
    }
}
