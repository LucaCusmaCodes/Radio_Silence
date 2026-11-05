using UnityEngine;

public class InteractablePhone : MonoBehaviour
{
    [Header("Event Data")]
    public ChoiceEvent choiceEvent;
    public PhoneTimerUI phoneUI;
    public KeyCode interactKey = KeyCode.E;

    [Header("3D Model Settings")]
    public GameObject phoneDeskModel; // Drag the 3D model or graphic here!
    public bool destroyOnPickUp = false; // Choose whether to hide or completely destroy it

    [Header("UI Prompt (Optional)")]
    public GameObject interactPromptUI; // e.g., "Press E to check phone" text

    private bool isPlayerInArea = false;
    private bool hasBeenPickedUp = false;

    private void Start()
    {
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        // If no separate model is assigned, default to this GameObject itself
        if (phoneDeskModel == null)
        {
            phoneDeskModel = this.gameObject;
        }
    }

    private void Update()
    {
        if (isPlayerInArea && !hasBeenPickedUp && Input.GetKeyDown(interactKey))
        {
            PickUpPhone();
        }
    }

    private void PickUpPhone()
    {
        hasBeenPickedUp = true;

        // Hide interaction UI prompt
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        // Trigger the UI event on screen
        if (phoneUI != null && choiceEvent != null)
        {
            phoneUI.StartEvent(choiceEvent);
        }

        // Hide the physical phone on the desk
        if (destroyOnPickUp)
        {
            Destroy(phoneDeskModel);
        }
        else
        {
            // Disables the mesh/object so it disappears cleanly from the world
            phoneDeskModel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenPickedUp) return;

        if (other.CompareTag("Player"))
        {
            isPlayerInArea = true;
            if (interactPromptUI != null) interactPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInArea = false;
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }
    }
}