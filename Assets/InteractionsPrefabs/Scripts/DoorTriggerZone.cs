using UnityEngine;

public class DoorTriggerZone : MonoBehaviour
{
    [Header("Target Door")]
    public DoorController targetDoor; // Auto-finds if left empty

    public enum DoorAction { Open, Close, Toggle }

    [Header("Trigger Settings")]
    public DoorAction actionOnEnter = DoorAction.Close;
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;
    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();

        if (targetDoor == null)
        {
            targetDoor = Object.FindFirstObjectByType<DoorController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnlyOnce && hasTriggered) return;

            if (targetDoor != null)
            {
                switch (actionOnEnter)
                {
                    case DoorAction.Open:
                        targetDoor.OpenDoor();
                        break;
                    case DoorAction.Close:
                        targetDoor.CloseDoor();
                        break;
                    case DoorAction.Toggle:
                        targetDoor.ToggleDoor();
                        break;
                }
            }

            hasTriggered = true;

            // Turn off the collider so it stops firing immediately
            if (triggerOnlyOnce && triggerCollider != null)
            {
                triggerCollider.enabled = false;
            }
        }
    }

    // --- CALL THIS WHEN THE PLAYER ENTERS THE BUS ---
    public void ResetTriggerState()
    {
        hasTriggered = false;

        // Re-enable the collider so it can be triggered again later
        if (triggerCollider != null)
        {
            triggerCollider.enabled = true;
        }

        Debug.Log("[DoorTriggerZone] Trigger has been reset by the bus entry event.");
    }
}