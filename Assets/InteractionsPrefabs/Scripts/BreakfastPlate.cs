using UnityEngine;

public class BreakfastPlate : MonoBehaviour
{
    [Header("Plate Config")]
    public int plateID; // 1 for Plate 1, 2 for Plate 2
    public AudioClip interactionSound;

    [Header("Visual Feedback")]
    public GameObject foodVisual; // Drag the food mesh/child object here!

    private bool playerInRange = false;
    private bool hasBeenUsed = false;

    private void OnEnable()
    {
        // Reset plate state on scene reload
        hasBeenUsed = false;
        playerInRange = false;

        // Hide food when entering/reloading the scene
        if (foodVisual != null)
        {
            foodVisual.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && !hasBeenUsed && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        hasBeenUsed = true;

        // Show the cooked food visual on the plate
        if (foodVisual != null)
        {
            foodVisual.SetActive(true);
        }

        // Play sizzle/cooking sound
        if (interactionSound != null)
        {
            AudioSource.PlayClipAtPoint(interactionSound, transform.position);
        }

        // Notify manager of interaction
        if (KitchenBreakfastManager.Instance != null)
        {
            KitchenBreakfastManager.Instance.OnPlateInteracted(plateID);
        }
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