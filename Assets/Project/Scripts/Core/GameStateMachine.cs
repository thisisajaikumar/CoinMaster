using UnityEngine;
using System;

public enum GameState
{
    Loading,
    Playing,
    Paused,
    GameOver,
}

public interface IGameState
{
    void Enter(GameState currentState, GameState lastState);
    void Update();
    void Exit();
    void HandleInput();
}

public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine Instance { get; private set; }

    public GameState CurrentState;
    public GameState LastState;
    public event Action<GameState> OnStateChanged;

    private IGameState[] states;
    private IGameState currentStateHandler;

    [Header("State References")]
    [SerializeField] private PlayingState playingState;
    [SerializeField] private PausedState pausedState;
    [SerializeField] private GameOverState gameOverState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeStates();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeStates()
    {
        states = new IGameState[5];
        states[(int)GameState.Playing] = playingState ?? gameObject.AddComponent<PlayingState>();
        states[(int)GameState.Paused] = pausedState ?? gameObject.AddComponent<PausedState>();
        states[(int)GameState.GameOver] = gameOverState ?? gameObject.AddComponent<GameOverState>();

        ChangeState(GameState.Loading);
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;

        currentStateHandler?.Exit();
        LastState = CurrentState;
        CurrentState = newState;
        currentStateHandler = states[(int)newState];
        currentStateHandler?.Enter(CurrentState, LastState);

        OnStateChanged?.Invoke(newState);
    }

    private void Update()
    {
        currentStateHandler?.Update();
        currentStateHandler?.HandleInput();
    }
}