using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class LoginScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField phoneNumberField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button showPasswordButton;
    [SerializeField] private Image showPasswordIcon;
    [SerializeField] private TMP_Text phoneValidationText;
    [SerializeField] private TMP_Text passwordValidationText;

    [Header("Visual References")]
    [SerializeField] private Image phoneFieldBackground;
    [SerializeField] private Image passwordFieldBackground;
    [SerializeField] private CanvasGroup formCanvasGroup;

    [Header("Icons")]
    [SerializeField] private Sprite showPasswordSprite;
    [SerializeField] private Sprite hidePasswordSprite;

    [Header("Colors")]
    [SerializeField] private Color validColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;

    private bool isPasswordVisible = false;
    private Sequence validationSequence;

    private void Start()
    {
        InitializeUI();
        SetupValidation();
        PlayEnterAnimation();
    }

    private void InitializeUI()
    {
        // Setup buttons
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
            loginButton.interactable = false;

            // Add button feedback
            ButtonFeedback feedback = loginButton.GetComponent<ButtonFeedback>();
            if (feedback == null)
                feedback = loginButton.gameObject.AddComponent<ButtonFeedback>();
        }

        if (showPasswordButton != null)
        {
            showPasswordButton.onClick.AddListener(TogglePasswordVisibility);
        }

        // Setup input fields
        if (phoneNumberField != null)
        {
            phoneNumberField.characterLimit = 10;
            phoneNumberField.contentType = TMP_InputField.ContentType.IntegerNumber;
            phoneNumberField.onValueChanged.AddListener(OnPhoneNumberChanged);
        }

        if (passwordField != null)
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;
            passwordField.onValueChanged.AddListener(OnPasswordChanged);
        }

        // Initialize validation texts
        if (phoneValidationText != null)
            phoneValidationText.gameObject.SetActive(false);

        if (passwordValidationText != null)
            passwordValidationText.gameObject.SetActive(false);

        // Set initial password visibility
        UpdatePasswordVisibility();
    }

    private void SetupValidation()
    {
        // Add real-time validation
        if (phoneNumberField != null)
        {
            phoneNumberField.onEndEdit.AddListener(ValidatePhoneNumber);
        }

        if (passwordField != null)
        {
            passwordField.onEndEdit.AddListener(ValidatePassword);
        }
    }

    private void PlayEnterAnimation()
    {
        if (formCanvasGroup != null)
        {
            formCanvasGroup.alpha = 0f;
            formCanvasGroup.transform.localScale = Vector3.one * 0.8f;

            Sequence enterSequence = DOTween.Sequence();
            enterSequence.Append(formCanvasGroup.DOFade(1f, 0.5f));
            enterSequence.Join(formCanvasGroup.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        }
    }

    private void OnPhoneNumberChanged(string value)
    {
        ValidatePhoneNumber(value);
        UpdateLoginButtonState();
    }

    private void OnPasswordChanged(string value)
    {
        ValidatePassword(value);
        UpdateLoginButtonState();
    }

    private void ValidatePhoneNumber(string phoneNumber)
    {
        ValidationResult result = InputValidator.ValidatePhoneNumber(phoneNumber);

        if (phoneValidationText != null)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                phoneValidationText.gameObject.SetActive(false);
            }
            else
            {
                phoneValidationText.gameObject.SetActive(true);
                phoneValidationText.text = result.Message;
                phoneValidationText.color = result.IsValid ? validColor : invalidColor;
            }
        }

        // Update field background color
        if (phoneFieldBackground != null)
        {
            Color targetColor = string.IsNullOrEmpty(phoneNumber) ? normalColor :
                               (result.IsValid ? validColor : invalidColor);
            phoneFieldBackground.DOColor(targetColor, 0.2f);
        }
    }

    private void ValidatePassword(string password)
    {
        ValidationResult result = InputValidator.ValidatePassword(password);

        if (passwordValidationText != null)
        {
            if (string.IsNullOrEmpty(password))
            {
                passwordValidationText.gameObject.SetActive(false);
            }
            else
            {
                passwordValidationText.gameObject.SetActive(true);
                passwordValidationText.text = result.Message;
                passwordValidationText.color = result.IsValid ? validColor : invalidColor;
            }
        }

        // Update field background color
        if (passwordFieldBackground != null)
        {
            Color targetColor = string.IsNullOrEmpty(password) ? normalColor :
                               (result.IsValid ? validColor : invalidColor);
            passwordFieldBackground.DOColor(targetColor, 0.2f);
        }
    }

    private void UpdateLoginButtonState()
    {
        if (loginButton == null) return;

        bool isPhoneValid = InputValidator.ValidatePhoneNumber(phoneNumberField?.text ?? "").IsValid;
        bool isPasswordValid = InputValidator.ValidatePassword(passwordField?.text ?? "").IsValid;

        bool shouldEnable = isPhoneValid && isPasswordValid;

        if (loginButton.interactable != shouldEnable)
        {
            loginButton.interactable = shouldEnable;

            // Animate button state change
            loginButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        }
    }

    private void TogglePasswordVisibility()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        isPasswordVisible = !isPasswordVisible;
        UpdatePasswordVisibility();
    }

    private void UpdatePasswordVisibility()
    {
        if (passwordField != null)
        {
            passwordField.contentType = isPasswordVisible ?
                TMP_InputField.ContentType.Standard :
                TMP_InputField.ContentType.Password;
            passwordField.ForceLabelUpdate();
        }

        if (showPasswordIcon != null)
        {
            showPasswordIcon.sprite = isPasswordVisible ? hidePasswordSprite : showPasswordSprite;
        }
    }

    private void OnLoginButtonClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        StartCoroutine(ProcessLogin());
    }

    private IEnumerator ProcessLogin()
    {
        // Disable login button to prevent multiple clicks
        if (loginButton != null)
            loginButton.interactable = false;

        // Simulate login process
        yield return new WaitForSeconds(0.5f);

        // Animate success
        if (validationSequence != null)
            validationSequence.Kill();

        validationSequence = DOTween.Sequence();
        validationSequence.Append(formCanvasGroup.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f));
        validationSequence.AppendCallback(() =>
        {
            SceneLoader.Instance.LoadScene("MainMenuScene");
        });
    }

    private void OnDestroy()
    {
        validationSequence?.Kill();

        if (loginButton != null)
            loginButton.onClick.RemoveAllListeners();

        if (showPasswordButton != null)
            showPasswordButton.onClick.RemoveAllListeners();

        if (phoneNumberField != null)
        {
            phoneNumberField.onValueChanged.RemoveAllListeners();
            phoneNumberField.onEndEdit.RemoveAllListeners();
        }

        if (passwordField != null)
        {
            passwordField.onValueChanged.RemoveAllListeners();
            passwordField.onEndEdit.RemoveAllListeners();
        }
    }
}