using UnityEngine;

public class ColorExample : MonoBehaviour
{
    void Start()
    {
        // Initialize with RGB
        Color redColor = new Color(1f, 0f, 0f);

        // Initialize with RGBA
        Color blueWithAlpha = new Color(0f, 0f, 1f, 0.5f);

        // Initialize with hex
        ColorUtility.TryParseHtmlString("#00FF00", out Color greenFromHex);

        // Initialize with HSV
        float hue = 0.6f;
        float saturation = 0.9f;
        float value = 0.7f;
        Color fromHSV = Color.HSVToRGB(hue, saturation, value);

        // Use some of the preset colors
        Color cornflower = Color.cornflowerBlue;
        Color lawn = Color.lawnGreen;
        Color lemon = Color.lemonChiffon;
        Color violet = Color.violet;

        // Create complementary color
        Color complementary = new Color(
            1f - redColor.r,
            1f - redColor.g,
            1f - redColor.b
        );
        // Lerp between colors
        Color lerpedColor = Color.Lerp(redColor, blueWithAlpha, 0.5f);

        // Darken color
        Color darkercolor = lerpedColor * 0.8f;

        // Lighten color
        Color lighterColor = lerpedColor + Color.white * 0.2f;
    }
}