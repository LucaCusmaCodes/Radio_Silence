using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Renderer targetRenderer;

    [ColorUsage(true, true)]
    public Color glowColor = Color.yellow;

    [Header("Pulse Animation")]
    public float pulseSpeed = 3.0f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 3.0f;

    [Header("Fade Settings")]
    public float fadeSpeed = 5.0f;

    private Color originalColor;
    private Material matInstance;
    private bool isNearPlayer = false;
    private float transitionFactor = 0f;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null)
        {
            // Creates a unique material instance so we don't modify the project asset
            matInstance = targetRenderer.material;

            // Store the original base color of the untextured material
            if (matInstance.HasProperty("_Color"))
            {
                originalColor = matInstance.color;
            }
            else if (matInstance.HasProperty("_BaseColor"))
            {
                originalColor = matInstance.GetColor("_BaseColor");
            }
        }
    }

    private void Update()
    {
        if (matInstance == null) return;

        // Smoothly move transition factor towards 1 (near) or 0 (away)
        float targetFactor = isNearPlayer ? 1f : 0f;
        transitionFactor = Mathf.MoveTowards(transitionFactor, targetFactor, fadeSpeed * Time.deltaTime);

        if (transitionFactor > 0f)
        {
            // Generate smooth pulse wave between 0 and 1
            float wave = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            float currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, wave);

            // Scale intensity by how faded in the player is
            float finalFactor = currentIntensity * transitionFactor;

            // 1. Blend base color toward glow color based on the pulse wave (Guarantees visual change!)
            Color blendedColor = Color.Lerp(originalColor, glowColor, Mathf.Clamp01(finalFactor * 0.5f));

            if (matInstance.HasProperty("_Color"))
            {
                matInstance.color = blendedColor;
            }
            else if (matInstance.HasProperty("_BaseColor"))
            {
                matInstance.SetColor("_BaseColor", blendedColor);
            }

            // 2. Also apply standard emission if the shader supports it
            if (matInstance.HasProperty("_EmissionColor"))
            {
                matInstance.EnableKeyword("_EMISSION");
                matInstance.SetColor("_EmissionColor", glowColor * finalFactor);
            }
        }
        else
        {
            // Reset to original appearance when away
            if (matInstance.HasProperty("_Color"))
            {
                matInstance.color = originalColor;
            }
            else if (matInstance.HasProperty("_BaseColor"))
            {
                matInstance.SetColor("_BaseColor", originalColor);
            }

            if (matInstance.HasProperty("_EmissionColor"))
            {
                matInstance.SetColor("_EmissionColor", Color.black);
                matInstance.DisableKeyword("_EMISSION");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPlayer = false;
        }
    }

    private void OnDisable()
    {
        isNearPlayer = false;
        transitionFactor = 0f;
        if (matInstance != null)
        {
            if (matInstance.HasProperty("_Color")) matInstance.color = originalColor;
            if (matInstance.HasProperty("_EmissionColor"))
            {
                matInstance.SetColor("_EmissionColor", Color.black);
                matInstance.DisableKeyword("_EMISSION");
            }
        }
    }
}