
using UnityEngine;
using TMPro;

public class SpawnCharacterOnTrigger : MonoBehaviour
{
    public GameObject spawnPoint;
    public GameObject characterPrefab;
    public Transform pointB;
    public string triggeringTag = "Player";

    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasSpawned || !other.CompareTag(triggeringTag))
            return;

        GameObject customer = Instantiate(
            characterPrefab,
            spawnPoint.transform.position,
            spawnPoint.transform.rotation
        );

        CustomerMovement walkingScript =
         customer.GetComponent<CustomerMovement>();

        if (walkingScript != null)
        {
            walkingScript.pointB = pointB;
        }
        else
        {
            Debug.LogWarning(
                "WalkToPoint script not found on customer root."
            );
        }

        // Connect the floating dialogue box.
        CustomerDialogue dialogue =
            customer.GetComponent<CustomerDialogue>();

        if (dialogue != null)
        {
            Canvas floatingCanvas =
                customer.GetComponentInChildren<Canvas>(true);

            TMP_Text floatingText =
                customer.GetComponentInChildren<TMP_Text>(true);

            if (floatingCanvas != null && floatingText != null)
            {
                dialogue.Setup(
                     floatingCanvas.gameObject,
                     floatingText,
                     null
            );
            }
            else
            {
                Debug.LogWarning(
                    "Floating Canvas or TextMeshPro text not found."
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "CustomerDialogue script not found on customer root."
            );
        }

        hasSpawned = true;
    }
}
