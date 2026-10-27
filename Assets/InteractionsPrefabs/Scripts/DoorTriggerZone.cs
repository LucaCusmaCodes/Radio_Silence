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

            ExecuteDoorAction();

            hasTriggered = true;

            // Turn off the collider so it stops firing while inside/moving
            if (triggerOnlyOnce && triggerCollider != null)
            {
                triggerCollider.enabled = false;
            }
        }
    }

    public void ExecuteDoorAction()
    {
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
    }

    // --- CALL THIS WHEN THE BUS STOPS MOVING ---
    public void ResetTriggerState()
    {
        hasTriggered = false;

        // Re-enable the collider so it can trigger the exit
        if (triggerCollider != null)
        {
            triggerCollider.enabled = true;
        }

        Debug.Log("[DoorTriggerZone] Trigger reset! Ready for door to open.");
    }
}