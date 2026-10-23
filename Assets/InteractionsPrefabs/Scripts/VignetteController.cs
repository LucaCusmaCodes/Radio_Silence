using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class VignetteController : MonoBehaviour
{
    public static VignetteController Instance;

    [Header("Vignette Settings")]
    [Range(0f, 1f)] public float maxAlpha = 0.85f; // Maximum darkness cap
    public int totalEventsToMax = 5;               // Number of ignored events to reach max darkness
    public float fadeDuration = 1.2f;              // Transition speed when darkening

    private Image vignetteImage;
    private int currentIgnoredCount = 0;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        vignetteImage = GetComponent<Image>();

        // If no sprite is assigned, procedurally generate a smooth radial vignette texture!
        if (vignetteImage.sprite == null)
        {
            vignetteImage.sprite = GenerateVignetteSprite();
        }
    }

    private void Start()
    {
        // Start completely transparent
        if (vignetteImage != null)
        {
            Color c = vignetteImage.color;
            c.a = 0f;
            vignetteImage.color = c;
        }
    }

    /// <summary>
    /// Call this function every time an event is ignored/time runs out.
    /// </summary>
    public void IncreaseDarkness()
    {
        currentIgnoredCount++;

        // Calculate target alpha based on how many events have been ignored
        float targetAlpha = Mathf.Clamp01((float)currentIgnoredCount / totalEventsToMax) * maxAlpha;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeVignetteRoutine(targetAlpha));
    }

    private IEnumerator FadeVignetteRoutine(float targetAlpha)
    {
        if (vignetteImage == null) yield break;

        Color startColor = vignetteImage.color;
        float startAlpha = startColor.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);

            Color current = vignetteImage.color;
            current.a = newAlpha;
            vignetteImage.color = current;

            yield return null;
        }

        Color finalColor = vignetteImage.color;
        finalColor.a = targetAlpha;
        vignetteImage.color = finalColor;
    }

    private Sprite GenerateVignetteSprite()
    {
        int size = 256;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;

        float center = size / 2f;
        float maxRadius = center;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Mathf.Sqrt((x - center) * (x - center) + (y - center) * (y - center));
                float t = Mathf.Clamp01(distance / maxRadius);

                // Quadratic curve for a smooth falloff (clear center, dark edges)
                float alpha = Mathf.SmoothStep(0f, 1f, t);
                alpha = Mathf.Pow(alpha, 1.5f); // Adjust curve steepness if needed

                texture.SetPixel(x, y, new Color(0, 0, 0, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}