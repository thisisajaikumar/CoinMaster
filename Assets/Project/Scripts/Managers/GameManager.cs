using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Configuration")]
    [SerializeField] private GameConfig gameConfig;

    //[Header("Runtime Data")]
    public int Score { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool IsGameActive { get; private set; }

    private Coroutine gameTimerCoroutine;
    private WaitForSeconds timerUpdateInterval;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            timerUpdateInterval = new WaitForSeconds(0.1f); // Update UI 10 times per second
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameStateMachine.Instance.ChangeState(GameState.Playing);
    }

    public void StartGameplay()
    {
        Score = 0;
        TimeRemaining = gameConfig.GameDuration;
        IsGameActive = true;

        GameEvents.Instance.TriggerScoreChanged(Score);
        GameEvents.Instance.TriggerTimerChanged(TimeRemaining);

        if (gameTimerCoroutine != null)
            StopCoroutine(gameTimerCoroutine);

        CoinSpawner.Instance.StartSpawning();
        gameTimerCoroutine = StartCoroutine(GameTimerCoroutine());
    }

    public void ResumeGameplay()
    {
        IsGameActive = true;
    }

    public void AddScore(int points)
    {
        if (!IsGameActive) return;

        Score += points;
        GameEvents.Instance.TriggerScoreChanged(Score);
    }

    private IEnumerator GameTimerCoroutine()
    {
        while (TimeRemaining > 0 && IsGameActive)
        {
            if (GameStateMachine.Instance.CurrentState == GameState.Playing)
            {
                TimeRemaining -= 0.1f;
                GameEvents.Instance.TriggerTimerChanged(TimeRemaining);
            }

            yield return timerUpdateInterval;
        }

        if (TimeRemaining <= 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        IsGameActive = false;
        GameStateMachine.Instance.ChangeState(GameState.GameOver);
    }

    public void RestartGame()
    {
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        GameStateMachine.Instance.ChangeState(GameState.Playing);
    }

    public void BackToMenu()
    {
        SceneLoader.Instance.LoadScene("MainMenuScene");
    }
}