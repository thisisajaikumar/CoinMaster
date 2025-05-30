using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

[System.Serializable]
public class ThemeColors
{
    [Header("Background Colors")]
    public Color primaryBackground;
    public Color secondaryBackground;
    public Color panelBackground;

    [Header("Text Colors")]
    public Color primaryTextColor;
    public Color secondaryTextColor;
    public Color accentTextColor;

    [Header("UI Colors")]
    public Color buttonNormalColor;
    public Color buttonHighlightColor;
    public Color buttonPressedColor;
    public Color inputFieldColor;
    public Color validationSuccessColor;
    public Color validationErrorColor;
}

[CreateAssetMenu(fileName = "ThemeData", menuName = "Game/Theme Data")]
public class ThemeData : ScriptableObject
{
    [Header("Theme Configuration")]
    public string themeName;
    public ThemeColors lightTheme;
    public ThemeColors darkTheme;

    [Header("Animation Settings")]
    public float transitionDuration = 0.3f;
    public Ease transitionEase = Ease.OutQuad;
}
