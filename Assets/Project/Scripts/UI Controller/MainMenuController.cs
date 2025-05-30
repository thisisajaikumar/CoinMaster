using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button playGameButton;
    [SerializeField] private Button exitAppButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private TMP_Text welcomeText;
    [SerializeField] private TMP_Text highScoreText;

    [Header("Exit Confirmation")]
    [SerializeField] private GameObject exitConfirmationPanel;
    [SerializeField] private Button confirmExitButton;
    [SerializeField] private Button cancelExitButton;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Button muteButton;
    [SerializeField] private Button closeSettingsButton;

    [Header("Animation")]
    [SerializeField] private RectTransform[] animatedElements;
    [SerializeField] private float animationDelay = 0.1f;

    private Sequence menuAnimationSequence;

    private void Start()
    {
        InitializeUI();
        SetupAudio();
        PlayMenuAnimation();
        LoadPlayerData();
    }

    private void InitializeUI()
    {
        // Setup main buttons
        if (playGameButton != null)
        {
            playGameButton.onClick.AddListener(OnPlayGameClicked);
            AddButtonFeedback(playGameButton);
        }

        if (exitAppButton != null)
        {
            exitAppButton.onClick.AddListener(OnExitAppClicked);
            AddButtonFeedback(exitAppButton);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsClicked);
            AddButtonFeedback(settingsButton);
        }

        // Setup exit confirmation
        if (confirmExitButton != null)
        {
            confirmExitButton.onClick.AddListener(OnConfirmExitClicked);
            AddButtonFeedback(confirmExitButton);
        }

        if (cancelExitButton != null)
        {
            cancelExitButton.onClick.AddListener(OnCancelExitClicked);
            AddButtonFeedback(cancelExitButton);
        }

        // Setup settings
        if (closeSettingsButton != null)
        {
            closeSettingsButton.onClick.AddListener(OnCloseSettingsClicked);
            AddButtonFeedback(closeSettingsButton);
        }

        SetupVolumeSliders();

        // Initialize panels
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void AddButtonFeedback(Button button)
    {
        ButtonFeedback feedback = button.GetComponent<ButtonFeedback>();
        if (feedback == null)
            feedback = button.gameObject.AddComponent<ButtonFeedback>();
    }

    private void SetupVolumeSliders()
    {
        if (AudioManager.Instance == null) return;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = AudioManager.Instance.MasterVolume;
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = AudioManager.Instance.BGMVolume;
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = AudioManager.Instance.SFXVolume;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        if (muteButton != null)
        {
            muteButton.onClick.AddListener(OnMuteClicked);
            UpdateMuteButtonText();
        }
    }

    private void SetupAudio()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(BGMType.MainMenu);
        }
    }

    private void PlayMenuAnimation()
    {
        if (menuAnimationSequence != null)
            menuAnimationSequence.Kill();

        menuAnimationSequence = DOTween.Sequence();

        // Animate welcome text
        if (welcomeText != null)
        {
            welcomeText.transform.localScale = Vector3.zero;
            menuAnimationSequence.Append(welcomeText.transform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack));
        }

        // Animate UI elements
        for (int i = 0; i < animatedElements.Length; i++)
        {
            if (animatedElements[i] != null)
            {
                animatedElements[i].localScale = Vector3.zero;
                menuAnimationSequence.Append(animatedElements[i].DOScale(Vector3.one, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(animationDelay * i));
            }
        }
    }

    private void LoadPlayerData()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = $"High Score: {highScore}";
        }
    }

    // ==================== BUTTON EVENTS ====================

    private void OnPlayGameClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        SceneLoader.Instance.LoadScene("CoinTapGameScene");
    }

    private void OnExitAppClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        ShowExitConfirmation();
    }

    private void OnSettingsClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        ShowSettings();
    }

    private void OnConfirmExitClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        StartCoroutine(ExitApplication());
    }

    private void OnCancelExitClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        HideExitConfirmation();
    }

    private void OnCloseSettingsClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        HideSettings();
    }

    // ==================== VOLUME CONTROLS ====================

    private void OnMasterVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(value);
    }

    private void OnBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnMuteClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
            UpdateMuteButtonText();
        }
    }

    private void UpdateMuteButtonText()
    {
        if (muteButton != null && AudioManager.Instance != null)
        {
            TMP_Text buttonText = muteButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = AudioManager.Instance.IsMuted ? "Unmute" : "Mute";
            }
        }
    }

    // ==================== PANEL MANAGEMENT ====================

    private void ShowExitConfirmation()
    {
        if (exitConfirmationPanel == null) return;

        exitConfirmationPanel.SetActive(true);
        exitConfirmationPanel.transform.localScale = Vector3.zero;
        exitConfirmationPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    private void HideExitConfirmation()
    {
        if (exitConfirmationPanel == null) return;

        exitConfirmationPanel.transform.DOScale(Vector3.zero, 0.2f)
            .OnComplete(() => exitConfirmationPanel.SetActive(false));
    }

    private void ShowSettings()
    {
        if (settingsPanel == null) return;

        settingsPanel.SetActive(true);
        settingsPanel.transform.localScale = Vector3.zero;
        settingsPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    private void HideSettings()
    {
        if (settingsPanel == null) return;

        settingsPanel.transform.DOScale(Vector3.zero, 0.2f)
            .OnComplete(() => settingsPanel.SetActive(false));
    }

    private IEnumerator ExitApplication()
    {
        // Play exit animation
        if (menuAnimationSequence != null)
            menuAnimationSequence.Kill();

        menuAnimationSequence = DOTween.Sequence();
        menuAnimationSequence.Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));

        yield return menuAnimationSequence.WaitForCompletion();

        // Quit application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // ==================== ANDROID BACK BUTTON ====================

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }

    private void HandleBackButton()
    {
        if (settingsPanel != null && settingsPanel.activeInHierarchy)
        {
            HideSettings();
        }
        else if (exitConfirmationPanel != null && exitConfirmationPanel.activeInHierarchy)
        {
            HideExitConfirmation();
        }
        else
        {
            OnExitAppClicked();
        }
    }

    private void OnDestroy()
    {
        menuAnimationSequence?.Kill();

        // Remove all button listeners
        if (playGameButton != null) playGameButton.onClick.RemoveAllListeners();
        if (exitAppButton != null) exitAppButton.onClick.RemoveAllListeners();
        if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
        if (confirmExitButton != null) confirmExitButton.onClick.RemoveAllListeners();
        if (cancelExitButton != null) cancelExitButton.onClick.RemoveAllListeners();
        if (closeSettingsButton != null) closeSettingsButton.onClick.RemoveAllListeners();
        if (muteButton != null) muteButton.onClick.RemoveAllListeners();

        // Remove slider listeners
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.RemoveAllListeners();
        if (bgmVolumeSlider != null) bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.RemoveAllListeners();
    }
}
