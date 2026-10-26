using UnityEngine;

public class CleanableMess : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip cleanSound; // Optional: Sound effect when picked up/cleaned

    private bool playerInRange = false;

    private void Update()
    {
        // If the player is close and presses [E], clean up the mess
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            CleanUp();
        }
    }

    private void CleanUp()
    {
        // Play cleanup sound if assigned
        if (cleanSound != null)
        {
            AudioSource.PlayClipAtPoint(cleanSound, transform.position);
        }

        // 1. Hide the object immediately so BedInteraction knows it's cleaned right away
        gameObject.SetActive(false);

        // 2. Destroy the mess object from the scene
        Destroy(gameObject);
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