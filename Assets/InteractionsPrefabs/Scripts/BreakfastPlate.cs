
using UnityEngine;

public class BreakfastPlate : MonoBehaviour
{
    [Header("Plate Config")]
    public int plateID; // 1 for Plate 1, 2 for Plate 2
    public AudioClip interactionSound;

    [Header("Visual Feedback")]
    public GameObject foodVisual;

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
