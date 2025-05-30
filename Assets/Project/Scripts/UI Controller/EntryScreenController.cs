using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class EntryScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button loginButton;
    [SerializeField] private CanvasGroup mainCanvasGroup;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RectTransform logoTransform;
    [SerializeField] private RectTransform buttonTransform;

    [Header("Animation Settings")]
    [SerializeField] private float logoAnimationDuration = 1f;
    [SerializeField] private float buttonAnimationDuration = 0.5f;
    [SerializeField] private float buttonAnimationDelay = 0.5f;

    private Sequence introSequence;

    private void Start()
    {
        InitializeUI();
        PlayIntroAnimation();
        SetupAudio();
    }

    private void InitializeUI()
    {
        // Setup button
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);

            // Add button feedback
            ButtonFeedback feedback = loginButton.gameObject.GetComponent<ButtonFeedback>();
            if (feedback == null)
                feedback = loginButton.gameObject.AddComponent<ButtonFeedback>();
        }

        // Initialize animation states
        if (logoTransform != null)
        {
            logoTransform.localScale = Vector3.zero;
            logoTransform.anchoredPosition += Vector2.up * 100f;
        }

        if (buttonTransform != null)
        {
            buttonTransform.localScale = Vector3.zero;
        }
    }

    private void PlayIntroAnimation()
    {
        introSequence = DOTween.Sequence();

        // Logo animation
        if (logoTransform != null)
        {
            introSequence.Append(logoTransform.DOScale(Vector3.one, logoAnimationDuration)
                .SetEase(Ease.OutBack));
            introSequence.Join(logoTransform.DOAnchorPosY(logoTransform.anchoredPosition.y - 100f, logoAnimationDuration)
                .SetEase(Ease.OutQuad));
        }

        // Button animation
        if (buttonTransform != null)
        {
            introSequence.AppendInterval(buttonAnimationDelay);
            introSequence.Append(buttonTransform.DOScale(Vector3.one, buttonAnimationDuration)
                .SetEase(Ease.OutBack));
        }

        introSequence.Play();
    }

    private void SetupAudio()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(BGMType.MainMenu);
        }
    }

    private void OnLoginButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
        }

        // Animate out and load login scene
        StartCoroutine(TransitionToLogin());
    }

    private IEnumerator TransitionToLogin()
    {
        // Disable button to prevent multiple clicks
        if (loginButton != null)
            loginButton.interactable = false;

        // Animate out
        if (mainCanvasGroup != null)
        {
            yield return mainCanvasGroup.DOFade(0f, 0.3f).WaitForCompletion();
        }

        // Load login scene
        SceneLoader.Instance.LoadScene("LoginScene");
    }

    private void OnDestroy()
    {
        introSequence?.Kill();
        if (loginButton != null)
            loginButton.onClick.RemoveAllListeners();
    }
}