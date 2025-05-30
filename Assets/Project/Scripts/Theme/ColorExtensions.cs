using UnityEngine;

public static class ColorExtensions
{
    // Create color with alpha
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));
    }

    // Lighten color
    public static Color Lighten(this Color color, float amount)
    {
        return Color.Lerp(color, Color.white, Mathf.Clamp01(amount));
    }

    // Darken color
    public static Color Darken(this Color color, float amount)
    {
        return Color.Lerp(color, Color.black, Mathf.Clamp01(amount));
    }

    // Saturate color
    public static Color Saturate(this Color color, float amount)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        s = Mathf.Clamp01(s + amount);
        return Color.HSVToRGB(h, s, v).WithAlpha(color.a);
    }

    // Desaturate color
    public static Color Desaturate(this Color color, float amount)
    {
        return Saturate(color, -amount);
    }

    // Get complementary color
    public static Color Complementary(this Color color)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        h = (h + 0.5f) % 1f;
        return Color.HSVToRGB(h, s, v).WithAlpha(color.a);
    }

    // Convert to hex string
    public static string ToHex(this Color color)
    {
        Color32 c = color;
        return $"#{c.r:X2}{c.g:X2}{c.b:X2}{c.a:X2}";
    }

    // Create color from hex string
    public static Color FromHex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;

        Debug.LogWarning($"Failed to parse hex color: {hex}");
        return Color.white;
    }

    // Random color
    public static Color Random()
    {
        return new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            1f
        );
    }

    // Random color with fixed saturation and value
    public static Color RandomHue(float saturation = 1f, float value = 1f)
    {
        float hue = UnityEngine.Random.Range(0f, 1f);
        return Color.HSVToRGB(hue, saturation, value);
    }
}
