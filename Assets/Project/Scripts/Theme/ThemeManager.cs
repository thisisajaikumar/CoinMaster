using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private ThemeData themeData;

    [Header("Theme Toggle")]
    [SerializeField] private Button themeToggleButton;
    [SerializeField] private Image themeToggleIcon;
    [SerializeField] private Sprite lightModeIcon;
    [SerializeField] private Sprite darkModeIcon;

    // Theme state
    private bool isDarkMode = false;
    private ThemeColors currentTheme;

    // Registered UI elements
    private List<Image> registeredBackgrounds = new List<Image>();
    private List<TMP_Text> registeredTexts = new List<TMP_Text>();
    private List<Button> registeredButtons = new List<Button>();
    private List<TMP_InputField> registeredInputFields = new List<TMP_InputField>();

    // Animation sequences
    private Sequence themeTransitionSequence;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTheme();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetupThemeToggle();
        LoadThemePreference();
        ApplyTheme(isDarkMode, false); // Apply without animation on start
    }

    private void InitializeTheme()
    {
        if (themeData == null)
        {
            Debug.LogError("ThemeData not assigned to ThemeManager!");
            return;
        }

        currentTheme = isDarkMode ? themeData.darkTheme : themeData.lightTheme;
    }

    private void SetupThemeToggle()
    {
        if (themeToggleButton != null)
        {
            themeToggleButton.onClick.AddListener(ToggleTheme);

            // Add button feedback if not present
            ButtonFeedback feedback = themeToggleButton.GetComponent<ButtonFeedback>();
            if (feedback == null)
                feedback = themeToggleButton.gameObject.AddComponent<ButtonFeedback>();
        }
    }

    // ==================== PUBLIC API ====================

    public void ToggleTheme()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        isDarkMode = !isDarkMode;
        ApplyTheme(isDarkMode, true);
        SaveThemePreference();
    }

    public void SetDarkMode(bool darkMode, bool animated = true)
    {
        if (isDarkMode == darkMode) return;

        isDarkMode = darkMode;
        ApplyTheme(isDarkMode, animated);
        SaveThemePreference();
    }

    public void RegisterBackground(Image background)
    {
        if (background != null && !registeredBackgrounds.Contains(background))
        {
            registeredBackgrounds.Add(background);
            ApplyBackgroundColor(background);
        }
    }

    public void RegisterText(TMP_Text text)
    {
        if (text != null && !registeredTexts.Contains(text))
        {
            registeredTexts.Add(text);
            ApplyTextColor(text);
        }
    }

    public void RegisterButton(Button button)
    {
        if (button != null && !registeredButtons.Contains(button))
        {
            registeredButtons.Add(button);
            ApplyButtonColors(button);
        }
    }

    public void RegisterInputField(TMP_InputField inputField)
    {
        if (inputField != null && !registeredInputFields.Contains(inputField))
        {
            registeredInputFields.Add(inputField);
            ApplyInputFieldColor(inputField);
        }
    }

    public void UnregisterAll()
    {
        registeredBackgrounds.Clear();
        registeredTexts.Clear();
        registeredButtons.Clear();
        registeredInputFields.Clear();
    }

    // ==================== PRIVATE METHODS ====================

    private void ApplyTheme(bool darkMode, bool animated)
    {
        currentTheme = darkMode ? themeData.darkTheme : themeData.lightTheme;

        if (animated)
        {
            AnimateThemeTransition();
        }
        else
        {
            ApplyThemeImmediate();
        }

        UpdateThemeToggleIcon();
    }

    private void ApplyThemeImmediate()
    {
        // Apply to all registered elements
        foreach (var background in registeredBackgrounds)
            ApplyBackgroundColor(background);

        foreach (var text in registeredTexts)
            ApplyTextColor(text);

        foreach (var button in registeredButtons)
            ApplyButtonColors(button);

        foreach (var inputField in registeredInputFields)
            ApplyInputFieldColor(inputField);
    }

    private void AnimateThemeTransition()
    {
        if (themeTransitionSequence != null)
            themeTransitionSequence.Kill();

        themeTransitionSequence = DOTween.Sequence();

        // Animate backgrounds
        foreach (var background in registeredBackgrounds)
        {
            if (background != null)
            {
                Color targetColor = GetBackgroundColor(background);
                themeTransitionSequence.Join(background.DOColor(targetColor, themeData.transitionDuration)
                    .SetEase(themeData.transitionEase));
            }
        }

        // Animate text colors
        foreach (var text in registeredTexts)
        {
            if (text != null)
            {
                Color targetColor = GetTextColor(text);
                themeTransitionSequence.Join(text.DOColor(targetColor, themeData.transitionDuration)
                    .SetEase(themeData.transitionEase));
            }
        }

        // Animate button colors
        foreach (var button in registeredButtons)
        {
            if (button != null)
            {
                var colors = button.colors;
                colors.normalColor = currentTheme.buttonNormalColor;
                colors.highlightedColor = currentTheme.buttonHighlightColor;
                colors.pressedColor = currentTheme.buttonPressedColor;
                button.colors = colors;
            }
        }

        // Animate input field colors
        foreach (var inputField in registeredInputFields)
        {
            if (inputField != null)
            {
                Image fieldImage = inputField.GetComponent<Image>();
                if (fieldImage != null)
                {
                    themeTransitionSequence.Join(fieldImage.DOColor(currentTheme.inputFieldColor,
                        themeData.transitionDuration).SetEase(themeData.transitionEase));
                }
            }
        }
    }

    private void ApplyBackgroundColor(Image background)
    {
        if (background == null) return;

        Color targetColor = GetBackgroundColor(background);
        background.color = targetColor;
    }

    private void ApplyTextColor(TMP_Text text)
    {
        if (text == null) return;

        Color targetColor = GetTextColor(text);
        text.color = targetColor;
    }

    private void ApplyButtonColors(Button button)
    {
        if (button == null) return;

        var colors = button.colors;
        colors.normalColor = currentTheme.buttonNormalColor;
        colors.highlightedColor = currentTheme.buttonHighlightColor;
        colors.pressedColor = currentTheme.buttonPressedColor;
        button.colors = colors;
    }

    private void ApplyInputFieldColor(TMP_InputField inputField)
    {
        if (inputField == null) return;

        Image fieldImage = inputField.GetComponent<Image>();
        if (fieldImage != null)
        {
            fieldImage.color = currentTheme.inputFieldColor;
        }

        // Also update text color
        if (inputField.textComponent != null)
        {
            inputField.textComponent.color = currentTheme.primaryTextColor;
        }
    }

    private Color GetBackgroundColor(Image background)
    {
        // You can add logic here to determine which background color to use
        // based on the background's tag, name, or component
        if (background.CompareTag("PrimaryBackground"))
            return currentTheme.primaryBackground;
        else if (background.CompareTag("Panel"))
            return currentTheme.panelBackground;
        else
            return currentTheme.secondaryBackground;
    }

    private Color GetTextColor(TMP_Text text)
    {
        // Determine text color based on text type
        if (text.CompareTag("AccentText"))
            return currentTheme.accentTextColor;
        else if (text.CompareTag("SecondaryText"))
            return currentTheme.secondaryTextColor;
        else
            return currentTheme.primaryTextColor;
    }

    private void UpdateThemeToggleIcon()
    {
        if (themeToggleIcon != null)
        {
            themeToggleIcon.sprite = isDarkMode ? lightModeIcon : darkModeIcon;

            // Animate icon change
            themeToggleIcon.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        }
    }

    private void SaveThemePreference()
    {
        PlayerPrefs.SetInt("IsDarkMode", isDarkMode ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadThemePreference()
    {
        isDarkMode = PlayerPrefs.GetInt("IsDarkMode", 0) == 1;
    }

    private void OnDestroy()
    {
        themeTransitionSequence?.Kill();

        if (themeToggleButton != null)
            themeToggleButton.onClick.RemoveAllListeners();
    }

    // ==================== GETTERS ====================

    public bool IsDarkMode => isDarkMode;
    public ThemeColors CurrentTheme => currentTheme;
    public string CurrentThemeName => isDarkMode ? "Dark" : "Light";
}