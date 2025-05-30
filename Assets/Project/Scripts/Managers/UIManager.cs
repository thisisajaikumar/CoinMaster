using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button pauseButton;

    [Header("End Game UI")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backButton;

    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;

    // Cached strings to avoid allocations
    private string scoreFormat = "Score: {0}";
    private string timerFormat = "Time: {0:F0}";
    private string finalScoreFormat = "Final Score: {0}";

    // Animation sequences
    private Sequence scoreAnimSequence;

    private void Awake()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        pauseButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
            GameStateMachine.Instance.ChangeState(GameState.Paused);
        });

        restartButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
            endGamePanel.transform.DOScale(Vector3.zero, 0.5f)
        .SetEase(Ease.InBack)
        .OnComplete(() =>
        {
            endGamePanel.SetActive(false);
            GameManager.Instance.RestartGame();
        });
        });

        backButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
            GameManager.Instance.BackToMenu();
        });

        resumeButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
            GameStateMachine.Instance.ChangeState(GameState.Playing);
        });
    }

    void Start()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameEvents.Instance.OnScoreChanged.AddListener(UpdateScore);
        GameEvents.Instance.OnTimerChanged.AddListener(UpdateTimer);
        GameEvents.Instance.OnGameEnded.AddListener(ShowEndScreen);
        GameEvents.Instance.OnGamePaused.AddListener(ShowPauseScreen);
        GameEvents.Instance.OnGameResumed.AddListener(HidePauseScreen);
        GameEvents.Instance.OnCoinCollected.AddListener(PlayScoreAnimation);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = string.Format(scoreFormat, score);
    }

    public void UpdateTimer(float timeLeft)
    {
        timerText.text = string.Format(timerFormat, timeLeft);
    }

    public void ShowEndScreen(int finalScore)
    {
        finalScoreText.text = string.Format(finalScoreFormat, finalScore);

        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (finalScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", finalScore);
            highScoreText.text = "NEW HIGH SCORE!";
            highScoreText.color = Color.yellow;
        }
        else
        {
            highScoreText.text = $"High Score: {highScore}";
            highScoreText.color = Color.white;
        }

        endGamePanel.SetActive(true);
        endGamePanel.transform.localScale = Vector3.zero;
        endGamePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void ShowPauseScreen()
    {
        pausePanel.SetActive(true);
        pausePanel.transform.localScale = Vector3.zero;
        pausePanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad);
    }

    public void HidePauseScreen()
    {
        pausePanel.transform.DOScale(Vector3.zero, 0.2f)
            .OnComplete(() => pausePanel.SetActive(false));
    }

    private void PlayScoreAnimation()
    {
        scoreAnimSequence?.Kill();
        scoreAnimSequence = DOTween.Sequence()
            .Append(scoreText.transform.DOScale(1.2f, 0.1f))
            .Append(scoreText.transform.DOScale(1f, 0.1f));
    }

    private void OnDestroy()
    {
        scoreAnimSequence?.Kill();

        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnScoreChanged.RemoveListener(UpdateScore);
            GameEvents.Instance.OnTimerChanged.RemoveListener(UpdateTimer);
            GameEvents.Instance.OnGameEnded.RemoveListener(ShowEndScreen);
            GameEvents.Instance.OnGamePaused.RemoveListener(ShowPauseScreen);
            GameEvents.Instance.OnGameResumed.RemoveListener(HidePauseScreen);
            GameEvents.Instance.OnCoinCollected.RemoveListener(PlayScoreAnimation);
        }
    }
}