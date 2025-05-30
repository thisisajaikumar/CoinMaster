using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }

    [Header("Game Events")]
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<float> OnTimerChanged;
    public UnityEvent<int> OnGameEnded;
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    public UnityEvent OnCoinCollected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
    public void TriggerTimerChanged(float timeLeft) => OnTimerChanged?.Invoke(timeLeft);
    public void TriggerGameEnded(int finalScore) => OnGameEnded?.Invoke(finalScore);
    public void TriggerGamePaused() => OnGamePaused?.Invoke();
    public void TriggerGameResumed() => OnGameResumed?.Invoke();
    public void TriggerCoinCollected() => OnCoinCollected?.Invoke();
}