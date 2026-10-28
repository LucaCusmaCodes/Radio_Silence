using UnityEngine;

public class BreakfastPlate : MonoBehaviour
{
    [Header("Plate Config")]
    public int plateID; // 1 for Plate 1, 2 for Plate 2
    public AudioClip interactionSound;

    [Header("Visual Feedback")]
    public GameObject foodVisual;
    public InteractableHighlight interactHighlight; // Drag the object's InteractableHighlight component here

    private bool playerInRange = false;
    private bool hasBeenUsed = false;
    private bool interactionDisabled = false;

    private void OnEnable()
    {
        hasBeenUsed = false;
        playerInRange = false;
        interactionDisabled = false;

        if (foodVisual != null)
        {
            foodVisual.SetActive(false);
        }

        // Re-enable highlight when enabled
        if (interactHighlight != null)
        {
            interactHighlight.enabled = true;
        }
    }

    private void Update()
    {
        if (interactionDisabled || hasBeenUsed)
            return;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (interactionDisabled || hasBeenUsed)
            return;

        hasBeenUsed = true;

        // Disable highlight component so it turns off and stays off
        if (interactHighlight != null)
        {
            interactHighlight.enabled = false;
        }

        if (foodVisual != null)
        {
            foodVisual.SetActive(true);
        }

        if (interactionSound != null)
        {
            AudioSource.PlayClipAtPoint(
                interactionSound,
                transform.position
            );
        }

        if (KitchenBreakfastManager.Instance != null)
        {
            KitchenBreakfastManager.Instance.OnPlateInteracted(plateID);
        }
    }

    // Called by KitchenBreakfastManager when time expires.
    public void DisableInteraction()
    {
        interactionDisabled = true;
        playerInRange = false;

        // Disable highlight component when time runs out
        if (interactHighlight != null)
        {
            interactHighlight.enabled = false;
        }

        Debug.Log("[BreakfastPlate] Plate " + plateID +
                  " interaction disabled.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}