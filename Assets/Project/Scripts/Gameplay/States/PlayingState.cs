using UnityEngine;
using System;

public class PlayingState : MonoBehaviour, IGameState
{
    private GameManager gameManager;

    public void Enter(GameState currentState, GameState lastState)
    {
        gameManager = GameManager.Instance;

        if (lastState != GameState.Paused)
            gameManager.StartGameplay();
        else
            gameManager.ResumeGameplay();

        AudioManager.Instance.PlayBGM(BGMType.Gameplay);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameStateMachine.Instance.ChangeState(GameState.Paused);
        }
    }

    public void HandleInput()
    {
        // Handle game-specific input
    }

    public void Exit()
    {
        // Cleanup when leaving playing state
    }
}