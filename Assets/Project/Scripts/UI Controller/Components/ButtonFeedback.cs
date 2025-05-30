using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Animation")]
    [SerializeField] private float pressedScale = 0.95f;
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private Ease animationEase = Ease.OutQuad;

    [Header("Color Animation")]
    [SerializeField] private bool useColorAnimation = true;
    [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1f);

    private Vector3 originalScale;
    private Color originalColor;
    private Image buttonImage;
    private Button button;
    private Tween scaleTween;
    private Tween colorTween;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        originalScale = transform.localScale;

        if (buttonImage != null)
            originalColor = buttonImage.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        AnimateScale(originalScale * pressedScale);

        if (useColorAnimation && buttonImage != null)
            AnimateColor(pressedColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        AnimateScale(originalScale);

        if (useColorAnimation && buttonImage != null)
            AnimateColor(originalColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        if (useColorAnimation && buttonImage != null)
            AnimateColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        AnimateScale(originalScale);

        if (useColorAnimation && buttonImage != null)
            AnimateColor(originalColor);
    }

    private void AnimateScale(Vector3 targetScale)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(targetScale, animationDuration).SetEase(animationEase);
    }

    private void AnimateColor(Color targetColor)
    {
        colorTween?.Kill();
        colorTween = buttonImage.DOColor(targetColor, animationDuration);
    }

    private void OnDestroy()
    {
        scaleTween?.Kill();
        colorTween?.Kill();
    }
}