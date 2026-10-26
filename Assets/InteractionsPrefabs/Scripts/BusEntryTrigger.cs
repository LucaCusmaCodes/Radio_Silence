using UnityEngine;

public class BusEntryTrigger : MonoBehaviour
{
    [Header("Bus Doors (Will Open)")]
    public DoorController[] busDoors;         // Drag your bus Door objects here

    [Header("House Triggers (Will Unlock / Be Made Openable Again)")]
    public DoorTriggerZone[] houseTriggers;  // Drag your house DoorTriggerZones here

    [Header("Trigger Settings")]
    public bool triggerOnlyOnce = true;       // Ensures this boarding event only fires once

    private bool hasTriggered = false;
    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the bus entry zone
        if (other.CompareTag("Player"))
        {
            if (triggerOnlyOnce && hasTriggered) return;
            hasTriggered = true;

            // 1. Open the bus doors automatically
            if (busDoors != null)
            {
                foreach (DoorController busDoor in busDoors)
                {
                    if (busDoor != null)
                    {
                        busDoor.OpenDoor();
                    }
                }
            }

            // 2. Reset/unlock the house triggers so the player can interact with/open them again
            if (houseTriggers != null)
            {
                foreach (DoorTriggerZone houseTrigger in houseTriggers)
                {
                    if (houseTrigger != null)
                    {
                        houseTrigger.ResetTriggerState();
                    }
                }
            }

            // Disable this trigger if it's set to happen only once
            if (triggerOnlyOnce && triggerCollider != null)
            {
                triggerCollider.enabled = false;
            }

            Debug.Log("[BusEntryTrigger] Player entered the bus: Bus doors opened, house doors rearmed.");
        }
    }
}