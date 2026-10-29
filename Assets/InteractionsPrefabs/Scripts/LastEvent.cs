using UnityEngine;
using UnityEngine.UI;

public class LastEvent : MonoBehaviour
{
    [Header("UI Buttons (No Panel Needed)")]
    public Button askForHelpButton;       // Drag your "Ask for Help" button directly
    public Button doItYourselfButton;     // Drag your "Do It Yourself" button directly

    [Header("Player Movement Control")]
    public MonoBehaviour playerMovementScript; // Drag your Player Movement script here to freeze movement

    [Header("Score Restriction for 'Ask for Help'")]
    public bool restrictHelpByScore = true;
    public int maxScoreToDisableHelp = 3;  // If internalScore >= this, Ask For Help becomes unclickable & grayed out

    [Header("Spawn Objects on 'Ask for Help'")]
    public GameObject firstSpawnPrefab;
    public Transform firstSpawnLocation;
    public GameObject secondSpawnPrefab;
    public Transform secondSpawnLocation;

    [Header("Object Destruction on 'Ask for Help'")]
    public GameObject objectToDestroyOnHelp; // Drag the object you want destroyed when 'Ask for Help' is clicked

    [Header("Ladder / Interactable Activation on 'Do It Yourself'")]
    public GameObject ladderObjectToActivate; // Drag your ladder GameObject here (it can start deactivated in the scene)

    [Header("Consequences & Audio")]
    public int helpScoreChange = 0;
    public int diyScoreChange = 1;
    public AudioClip clickSound;

    private bool eventTriggered = false;

    private void Awake()
    {
        // Instantly hide the individual buttons when the scene loads
        if (askForHelpButton != null) askForHelpButton.gameObject.SetActive(false);
        if (doItYourselfButton != null) doItYourselfButton.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !eventTriggered)
        {
            eventTriggered = true;
            OpenChoiceButtons();
        }
    }

    private void OpenChoiceButtons()
    {
        // 1. FREEZE PLAYER MOVEMENT so they can't walk away while choosing
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        // 2. UNLOCK AND SHOW MOUSE CURSOR
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Check score to see if "Ask for Help" should be disabled and grayed out
        UpdateAskForHelpButtonState();

        // 4. Show the individual buttons
        if (askForHelpButton != null)
        {
            askForHelpButton.gameObject.SetActive(true);
            askForHelpButton.onClick.RemoveAllListeners();
            askForHelpButton.onClick.AddListener(OnAskForHelpClicked);
        }

        if (doItYourselfButton != null)
        {
            doItYourselfButton.gameObject.SetActive(true);
            doItYourselfButton.onClick.RemoveAllListeners();
            doItYourselfButton.onClick.AddListener(OnDoItYourselfClicked);
        }

        Debug.Log("[LastEvent] Choice buttons opened. Player movement frozen.");
    }

    private void UpdateAskForHelpButtonState()
    {
        if (askForHelpButton == null) return;

        bool shouldDisableHelp = false;

        if (restrictHelpByScore && GameManager.Instance != null)
        {
            if (GameManager.Instance.internalScore >= maxScoreToDisableHelp)
            {
                shouldDisableHelp = true;
            }
        }

        // Setting interactable to false automatically grays out the button in Unity UI
        askForHelpButton.interactable = !shouldDisableHelp;

        if (shouldDisableHelp)
        {
            Debug.Log("[LastEvent] Score threshold reached. 'Ask for Help' button disabled and grayed out.");
        }
    }

    private void OnAskForHelpClicked()
    {
        PlayClickSound();

        // 1. Spawn both objects
        SpawnObject(firstSpawnPrefab, firstSpawnLocation);
        SpawnObject(secondSpawnPrefab, secondSpawnLocation);

        // 2. Destroy the designated target object
        if (objectToDestroyOnHelp != null)
        {
            Destroy(objectToDestroyOnHelp);
            Debug.Log($"[LastEvent] Destroyed object: {objectToDestroyOnHelp.name}");
        }

        // 3. Process score in GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice("Asked for Help", helpScoreChange);
        }

        Debug.Log("[LastEvent] 'Ask for Help' chosen.");

        CloseEvent();
    }

    private void OnDoItYourselfClicked()
    {
        PlayClickSound();

        // Activate the ladder GameObject in the scene
        if (ladderObjectToActivate != null)
        {
            ladderObjectToActivate.SetActive(true);
            Debug.Log("[LastEvent] Ladder activated successfully!");
        }

        // Process score in GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice("Did It Yourself", diyScoreChange);
        }

        Debug.Log("[LastEvent] 'Do It Yourself' chosen.");

        CloseEvent();
    }

    private void SpawnObject(GameObject prefab, Transform location)
    {
        if (prefab == null) return;

        Vector3 spawnPos = location != null ? location.position : transform.position;
        Quaternion spawnRot = location != null ? location.rotation : Quaternion.identity;

        Instantiate(prefab, spawnPos, spawnRot);
    }

    private void PlayClickSound()
    {
        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, transform.position);
        }
    }

    private void CloseEvent()
    {
        // Hide the individual buttons
        if (askForHelpButton != null) askForHelpButton.gameObject.SetActive(false);
        if (doItYourselfButton != null) doItYourselfButton.gameObject.SetActive(false);

        // UNFREEZE PLAYER MOVEMENT
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        // Re-lock and hide the mouse cursor to return to normal gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Disable trigger collider so the event can't be re-triggered
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Disable the script
        this.enabled = false;
    }
}