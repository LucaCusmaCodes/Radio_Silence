using UnityEngine;

public class ActivateLastEventTrigger : MonoBehaviour
{
    [Header("Target Controller / Event")]
    [Tooltip("Drag your deactivated LastEvent GameObject here.")]
    public GameObject lastEventObjectToActivate;

    [Header("Trigger Settings")]
    public bool triggerOnceOnly = true;

    [Header("Optional Audio")]
    public AudioClip activationSound;
    [Range(0f, 1f)]
    public float soundVolume = 1.0f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (triggerOnceOnly)
            {
                hasTriggered = true;
            }

            // Activate the LastEvent controller object
            if (lastEventObjectToActivate != null)
            {
                lastEventObjectToActivate.SetActive(true);
                Debug.Log($"[Trigger] Successfully activated LastEvent GameObject: {lastEventObjectToActivate.name}");
            }
            else
            {
                Debug.LogWarning("[Trigger] No LastEvent GameObject assigned in Inspector!");
            }

            // Play optional feedback sound
            if (activationSound != null)
            {
                AudioSource.PlayClipAtPoint(activationSound, transform.position, soundVolume);
            }

            // Disable this trigger zone so it doesn't re-fire
            if (triggerOnceOnly)
            {
                Collider col = GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = false;
                }
            }
        }
    }
}