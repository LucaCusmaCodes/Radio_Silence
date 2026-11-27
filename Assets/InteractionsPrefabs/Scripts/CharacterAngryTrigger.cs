using UnityEngine;

public class CharacterAngryTrigger : MonoBehaviour
{
    [Header("Character Reference")]
    public InteractableHighlight highlightScript;

    [Header("Input")]
    public KeyCode returnKey = KeyCode.E;

    [Header("Visual Target")]
    public Renderer playerRenderer;
    public Material angryMaterial;
    public bool useColorSwapInstead = false;
    public Color angryColor = Color.red;

    private bool wasHighlightEnabled = false;
    private bool pressedReturnKeyDuringEvent = false;
    private bool triggeredAngryState = false;
    private Material enforcedAngryMat;

    private void Awake()
    {
        if (highlightScript == null)
        {
            highlightScript = GetComponent<InteractableHighlight>();
        }

        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<Renderer>();
        }

        if (highlightScript != null)
        {
            wasHighlightEnabled = highlightScript.enabled;
        }
    }

    private void Update()
    {
        if (highlightScript == null) return;

        // Only record the E press while the highlight/event is still active
        if (highlightScript.enabled && Input.GetKeyDown(returnKey))
        {
            pressedReturnKeyDuringEvent = true;
        }

        // Trigger angry state ONLY if highlight just disabled AND E was pressed during the event
        if (wasHighlightEnabled && !highlightScript.enabled && pressedReturnKeyDuringEvent)
        {
            triggeredAngryState = true;
        }

        wasHighlightEnabled = highlightScript.enabled;
    }

    private void LateUpdate()
    {
        if (!triggeredAngryState || playerRenderer == null) return;

        if (useColorSwapInstead)
        {
            Material mat = playerRenderer.material;
            if (mat.HasProperty("_Color") && mat.color != angryColor)
            {
                mat.color = angryColor;
            }
            else if (mat.HasProperty("_BaseColor") && mat.GetColor("_BaseColor") != angryColor)
            {
                mat.SetColor("_BaseColor", angryColor);
            }
        }
        else if (angryMaterial != null)
        {
            if (enforcedAngryMat == null || playerRenderer.sharedMaterial != enforcedAngryMat)
            {
                playerRenderer.material = angryMaterial;
                enforcedAngryMat = playerRenderer.sharedMaterial;
            }
        }
    }
}