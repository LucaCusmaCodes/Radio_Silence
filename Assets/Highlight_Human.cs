
using UnityEngine;
using System.Collections;

public class Highlight_Human : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Renderer targetRenderer;
    public Color highlightColor = new Color(0.1f, 0.85f, 0.8f);
    public float pulseSpeed = 2f;
    public float emissionIntensity = 2f;

    private Material highlightMaterial;
    private Color originalColor;
    private bool highlightAllowed = false;
    private bool isNearPlayer = false;

    private Coroutine highlightCoroutine;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        if (targetRenderer != null)
        {
            highlightMaterial = targetRenderer.material;
            originalColor = highlightMaterial.color;
        }
        else
        {
            Debug.LogError("No Renderer assigned to Highlight_Human!");
        }
    }

    public void SetHighlightEnabled(bool enabled)
    {
        highlightAllowed = enabled;

        Debug.Log("Highlight allowed: " + enabled);

        if (!enabled)
        {
            isNearPlayer = false;
        }

        UpdateHighlight();
    }

    public void CheckPlayerNearby()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        if (box == null)
        {
            Debug.LogError("Detection Cube has no Box Collider!");
            return;
        }

        Collider[] colliders = Physics.OverlapBox(
            transform.TransformPoint(box.center),
            Vector3.Scale(box.size, transform.lossyScale) / 2f,
            transform.rotation
        );

        isNearPlayer = false;

        foreach (Collider col in colliders)
        {
            Debug.Log("Overlap detected: " + col.name);

            if (col.transform.root.CompareTag("Player"))
            {
                isNearPlayer = true;
                Debug.Log("PLAYER ALREADY INSIDE DETECTION CUBE!");
                break;
            }
        }

        UpdateHighlight();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the highlight zone: " + other.name);

        if (other.transform.root.CompareTag("Player"))
        {
            isNearPlayer = true;
            Debug.Log("PLAYER ENTERED HIGHLIGHT ZONE!");
            UpdateHighlight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Something exited the highlight zone: " + other.name);

        if (other.transform.root.CompareTag("Player"))
        {
            isNearPlayer = false;
            Debug.Log("PLAYER LEFT HIGHLIGHT ZONE!");
            UpdateHighlight();
        }
    }

    private void UpdateHighlight()
    {
        if (highlightMaterial == null)
            return;

        bool shouldHighlight = highlightAllowed && isNearPlayer;

        if (shouldHighlight)
        {
            if (highlightCoroutine == null)
            {
                highlightCoroutine = StartCoroutine(PulseHighlight());
            }
        }
        else
        {
            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
                highlightCoroutine = null;
            }

            highlightMaterial.DisableKeyword("_EMISSION");
            highlightMaterial.color = originalColor;
        }
    }

    private IEnumerator PulseHighlight()
    {
        highlightMaterial.EnableKeyword("_EMISSION");

        while (highlightAllowed && isNearPlayer)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

            highlightMaterial.color = Color.Lerp(
                originalColor,
                highlightColor,
                pulse
            );

            highlightMaterial.SetColor(
                "_EmissionColor",
                highlightColor * (pulse * emissionIntensity)
            );

            yield return null;
        }

        highlightMaterial.DisableKeyword("_EMISSION");
        highlightMaterial.color = originalColor;
        highlightCoroutine = null;
    }

    private void OnDisable()
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
            highlightCoroutine = null;
        }

        if (highlightMaterial != null)
        {
            highlightMaterial.DisableKeyword("_EMISSION");
            highlightMaterial.color = originalColor;
        }
    }
}