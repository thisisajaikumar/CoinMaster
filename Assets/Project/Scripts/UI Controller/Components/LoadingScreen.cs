using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private TMP_Text tipsText;
    [SerializeField] private Image backgroundImage;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float progressAnimationSpeed = 2f;

    [Header("Loading Tips")]
    [SerializeField]
    private string[] loadingTips = {
        "Tap coins quickly to score points!",
        "Coins disappear after 3 seconds!",
        "Try to collect as many coins as possible!",
        "Beat your high score!",
        "Game lasts for 30 seconds!"
    };

    private Coroutine progressCoroutine;
    private Tween fadeTween;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (progressBar == null)
            progressBar = GetComponentInChildren<Slider>();

        if (loadingText == null)
            loadingText = GetComponentInChildren<TMP_Text>();

        // Initialize state
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;

        if (progressBar != null)
            progressBar.value = 0f;

        // Show random tip
        ShowRandomTip();
    }

    public Coroutine Show()
    {
        return StartCoroutine(ShowCoroutine());
    }

    public Coroutine Hide()
    {
        return StartCoroutine(HideCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        gameObject.SetActive(true);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;

        // Fade in
        fadeTween?.Kill();
        fadeTween = canvasGroup.DOFade(1f, fadeInDuration);
        yield return fadeTween.WaitForCompletion();

        // Start fake progress animation
        if (progressCoroutine != null)
            StopCoroutine(progressCoroutine);
        progressCoroutine = StartCoroutine(AnimateProgress());
    }

    private IEnumerator HideCoroutine()
    {
        // Stop progress animation
        if (progressCoroutine != null)
        {
            StopCoroutine(progressCoroutine);
            progressCoroutine = null;
        }

        // Fade out
        fadeTween?.Kill();
        fadeTween = canvasGroup.DOFade(0f, fadeOutDuration);
        yield return fadeTween.WaitForCompletion();

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    private IEnumerator AnimateProgress()
    {
        if (progressBar == null) yield break;

        progressBar.value = 0f;
        float targetProgress = 0f;

        while (progressBar.value < 0.9f) // Don't complete until scene is loaded
        {
            targetProgress = Mathf.Min(0.9f, targetProgress + Random.Range(0.1f, 0.3f));

            while (progressBar.value < targetProgress)
            {
                progressBar.value += Time.deltaTime * progressAnimationSpeed;
                UpdateLoadingText();
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }

    public void UpdateProgress(float progress)
    {
        if (progressBar != null)
        {
            float targetValue = Mathf.Clamp01(progress);
            progressBar.DOValue(targetValue, 0.2f);
        }
    }

    private void UpdateLoadingText()
    {
        if (loadingText != null)
        {
            int dots = Mathf.FloorToInt(Time.time * 2f) % 4;
            loadingText.text = "Loading" + new string('.', dots);
        }
    }

    private void ShowRandomTip()
    {
        if (tipsText != null && loadingTips.Length > 0)
        {
            string randomTip = loadingTips[Random.Range(0, loadingTips.Length)];
            tipsText.text = randomTip;
        }
    }

    private void OnDestroy()
    {
        fadeTween?.Kill();
        if (progressCoroutine != null)
            StopCoroutine(progressCoroutine);
    }
}