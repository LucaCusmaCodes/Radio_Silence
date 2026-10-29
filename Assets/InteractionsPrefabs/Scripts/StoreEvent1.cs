
using UnityEngine;
using UnityEngine.UI;

public class StoreEvent1 : MonoBehaviour
{
    [Header("UI Components")]
    public Image timerFillBar;
    public Button returnButton;
    public GameObject buttonContainer;

    [Header("World References")]
    public GameObject walletVisualModel;
    public InteractableHighlight walletHighlight;

    [Header("Event Settings")]
    public float timeLimitSeconds = 5f;

    [Header("Score Restriction (Force Timeout)")]
    public bool restrictByScore = true;
    public int maxScoreToInteract = 3;

    [Header("Man Speech Object")]
    public GameObject manSpeechPrefab;
    public Transform spawnLocation;

    [Header("Spawn Objects On Return")]
    public GameObject objectToSpawn;
    public Transform objectSpawnLocation;

    public GameObject secondObjectToSpawn;
    public Transform secondObjectSpawnLocation;

    [Header("End of Event Object Destruction")]
    public GameObject objectToDestroyOnEnd;

    [Header("Spawn Objects On Timeout")]
    public GameObject objectToSpawnOnTimeout;
    public Transform timeoutSpawnLocation;

    public GameObject secondObjectToSpawnOnTimeout;
    public Transform secondTimeoutSpawnLocation;

    [Header("Player Visual Swap")]
    public Renderer playerRenderer;
    public Material angryMaterial;
    public bool useColorSwapInstead = false;
    public Color angryColor = Color.red;

    [Header("Consequences")]
    public int returnScoreChange = 0;
    public int keepScoreChange = 1;

    [Header("Audio - Plays Only On Timeout")]
    public AudioClip returnSound;

    [Range(0f, 1f)]
    public float soundVolume = 1f;

    [Header("Disable Camera Look During Event")]
    [Tooltip("Drag your camera-look script here to prevent mouse movement from rotating the camera during the event.")]
    public Behaviour cameraLookScript;

    private float timer;
    private bool isEventActive;
    private bool playerMadeChoice;
    private bool blockPlayerInteraction;

    private bool previousCursorVisible;
    private CursorLockMode previousCursorLockMode;
    private bool previousCameraLookEnabled;

    private void Awake()
    {
        HideEventUI();

        // Connect the button automatically.
        // You do not need to configure On Click() manually.
        if (returnButton != null)
        {
            returnButton.onClick.RemoveListener(OnReturnButtonPressed);
            returnButton.onClick.AddListener(OnReturnButtonPressed);
        }
    }

    private void OnDestroy()
    {
        if (returnButton != null)
        {
            returnButton.onClick.RemoveListener(OnReturnButtonPressed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (isEventActive || playerMadeChoice)
            return;

        StartEvent();
    }

    private bool IsPlayerScoreTooHigh()
    {
        if (!restrictByScore)
            return false;

        if (GameManager.Instance != null)
        {
            return GameManager.Instance.internalScore >= maxScoreToInteract;
        }

        return false;
    }

    public void StartEvent()
    {
        if (isEventActive || playerMadeChoice)
            return;

        isEventActive = true;
        timer = Mathf.Max(0f, timeLimitSeconds);

        blockPlayerInteraction = IsPlayerScoreTooHigh();

        // Remember the existing cursor state.
        previousCursorVisible = Cursor.visible;
        previousCursorLockMode = Cursor.lockState;

        // Unlock the cursor so the UI button can be clicked.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable camera-look input while the player uses the UI.
        if (cameraLookScript != null)
        {
            previousCameraLookEnabled = cameraLookScript.enabled;
            cameraLookScript.enabled = false;
        }

        // Show the button container first.
        if (buttonContainer != null)
        {
            buttonContainer.SetActive(true);
        }

        // Show the timer.
        if (timerFillBar != null)
        {
            timerFillBar.gameObject.SetActive(true);
            timerFillBar.fillAmount = 1f;
        }

        // Prepare the button.
        if (returnButton != null)
        {
            returnButton.gameObject.SetActive(true);
            returnButton.interactable = !blockPlayerInteraction;
        }

        Debug.Log(
            "[StoreEvent2] Event started. Countdown: " + timer
        );
    }

    private void Update()
    {
        if (!isEventActive || playerMadeChoice)
            return;

        // Countdown runs independently of button clicks.
        timer -= Time.deltaTime;

        // Update the timer bar.
        if (timerFillBar != null)
        {
            timerFillBar.fillAmount = timeLimitSeconds > 0f
                ? Mathf.Clamp01(timer / timeLimitSeconds)
                : 0f;
        }

        // If time runs out, trigger the timeout outcome.
        if (timer <= 0f)
        {
            timer = 0f;
            OnTimerExpired();
        }
    }

    public void OnReturnButtonPressed()
    {
        if (!isEventActive || playerMadeChoice)
            return;

        if (blockPlayerInteraction)
            return;

        OnWalletReturned();
    }

    private void OnWalletReturned()
    {
        if (!isEventActive || playerMadeChoice)
            return;

        isEventActive = false;
        playerMadeChoice = true;

        CloseUI();
        RestoreGameplayControls();
        DestroyEndObject();

        // Hide the wallet visual model.
        if (walletVisualModel != null)
        {
            walletVisualModel.SetActive(false);
        }

        // Change the player's appearance.
        if (playerRenderer != null)
        {
            if (useColorSwapInstead)
            {
                playerRenderer.material.color = angryColor;
            }
            else if (angryMaterial != null)
            {
                playerRenderer.material = angryMaterial;
            }
        }

        // Spawn the man's speech object.
        if (manSpeechPrefab != null)
        {
            Vector3 pos = spawnLocation != null
                ? spawnLocation.position
                : transform.position;

            Quaternion rot = spawnLocation != null
                ? spawnLocation.rotation
                : Quaternion.identity;

            Instantiate(manSpeechPrefab, pos, rot);
        }

        // Spawn the return outcome objects.
        SpawnObject(objectToSpawn, objectSpawnLocation);
        SpawnObject(secondObjectToSpawn, secondObjectSpawnLocation);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice(
                "Returned Wallet",
                returnScoreChange
            );
        }

        Debug.Log("[StoreEvent2] Wallet returned.");

        DisableEventInteraction();
    }

    private void OnTimerExpired()
    {
        if (!isEventActive || playerMadeChoice)
            return;

        isEventActive = false;
        playerMadeChoice = true;

        CloseUI();
        RestoreGameplayControls();

        // Play audio only when the timer expires.
        if (returnSound != null)
        {
            AudioSource.PlayClipAtPoint(
                returnSound,
                transform.position,
                soundVolume
            );
        }

        DestroyEndObject();

        // Spawn the timeout outcome objects.
        SpawnObject(
            objectToSpawnOnTimeout,
            timeoutSpawnLocation
        );

        SpawnObject(
            secondObjectToSpawnOnTimeout,
            secondTimeoutSpawnLocation
        );

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ApplyPitchDropStep();
        }

        if (VignetteController.Instance != null)
        {
            VignetteController.Instance.IncreaseDarkness();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice(
                "Ignored Wallet",
                keepScoreChange
            );
        }

        Debug.Log("[StoreEvent2] Timer expired. Timeout outcome triggered.");

        DisableEventInteraction();
    }

    private void SpawnObject(GameObject prefab, Transform location)
    {
        if (prefab == null)
            return;

        Vector3 pos = location != null
            ? location.position
            : transform.position;

        Quaternion rot = location != null
            ? location.rotation
            : Quaternion.identity;

        Instantiate(prefab, pos, rot);

        Debug.Log("[StoreEvent2] Spawned object: " + prefab.name);
    }

    private void DestroyEndObject()
    {
        if (objectToDestroyOnEnd != null)
        {
            Destroy(objectToDestroyOnEnd);
            objectToDestroyOnEnd = null;
        }
    }

    private void HideEventUI()
    {
        if (buttonContainer != null)
        {
            buttonContainer.SetActive(false);
        }

        if (timerFillBar != null)
        {
            timerFillBar.fillAmount = 0f;
            timerFillBar.gameObject.SetActive(false);
        }

        if (returnButton != null)
        {
            returnButton.gameObject.SetActive(false);
        }
    }

    private void CloseUI()
    {
        HideEventUI();
    }

    private void RestoreGameplayControls()
    {
        // Restore the camera-look script.
        if (cameraLookScript != null)
        {
            cameraLookScript.enabled = previousCameraLookEnabled;
        }

        // Restore the original cursor state.
        Cursor.lockState = previousCursorLockMode;
        Cursor.visible = previousCursorVisible;
    }

    private void DisableEventInteraction()
    {
        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        if (walletHighlight != null)
        {
            walletHighlight.enabled = false;
        }

        enabled = false;
    }
}
