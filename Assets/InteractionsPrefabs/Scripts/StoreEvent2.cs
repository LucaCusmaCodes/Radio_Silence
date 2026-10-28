using UnityEngine;
using UnityEngine.UI;

public class StoreEvent2 : MonoBehaviour
{
    [Header("UI Component")]
    public Image timerFillBar;

    [Header("World References")]
    public GameObject walletVisualModel;
    public InteractableHighlight walletHighlight; // Drag the wallet's InteractableHighlight component here

    [Header("Event Settings")]
    public float timeLimitSeconds = 5.0f;
    public KeyCode returnKey = KeyCode.E;

    [Header("Score Restriction (Force Timeout)")]
    public bool restrictByScore = true;           // Enable score check
    public int maxScoreToInteract = 3;            // If internalScore >= this, player can't press E (timer forces timeout)

    [Header("Man Speech Object")]
    public GameObject manSpeechPrefab;
    public Transform spawnLocation;

    [Header("Spawn Objects On Interaction")]
    public GameObject objectToSpawn;
    public Transform objectSpawnLocation;

    public GameObject secondObjectToSpawn;
    public Transform secondObjectSpawnLocation;

    [Header("End of Event Object Destruction")]
    public GameObject objectToDestroyOnEnd;

    [Header("Spawn Objects On Timeout")]
    public GameObject objectToSpawnOnTimeout;
    public Transform timeoutSpawnLocation;

    // Second object spawned when timer expires.
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
    public float soundVolume = 1.0f;

    private float timer;
    private bool isEventActive = false;
    private bool playerMadeChoice = false;
    private bool blockPlayerInteraction = false; // Tracks if pressing E is blocked

    private void Awake()
    {
        if (timerFillBar != null)
        {
            timerFillBar.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") &&
            !playerMadeChoice &&
            !isEventActive)
        {
            // Check if player's internal score blocks the "E" key choice
            blockPlayerInteraction = IsPlayerScoreTooHigh();

            if (blockPlayerInteraction)
            {
                Debug.Log("[StoreEvent2] Player score reached threshold. E key interaction blocked; forcing timer to run out.");
            }

            StartEvent();
        }
    }

    private bool IsPlayerScoreTooHigh()
    {
        if (!restrictByScore) return false;

        if (GameManager.Instance != null)
        {
            return GameManager.Instance.internalScore >= maxScoreToInteract;
        }

        return false;
    }

    public void StartEvent()
    {
        isEventActive = true;
        timer = timeLimitSeconds;

        if (timerFillBar != null)
        {
            timerFillBar.gameObject.SetActive(true);
            timerFillBar.fillAmount = 1f;
        }

        Debug.Log("[StoreEvent2] Timer started!");
    }

    private void Update()
    {
        if (!isEventActive)
            return;

        timer -= Time.deltaTime;

        if (timerFillBar != null && timeLimitSeconds > 0f)
        {
            timerFillBar.fillAmount =
                Mathf.Clamp01(timer / timeLimitSeconds);
        }

        // Only allow pressing E if interaction is NOT blocked by score
        if (!blockPlayerInteraction && Input.GetKeyDown(returnKey))
        {
            OnWalletReturned();
        }
        else if (timer <= 0f)
        {
            OnTimerExpired();
        }
    }

    private void OnWalletReturned()
    {
        isEventActive = false;
        playerMadeChoice = true;

        CloseUI();

        // Destroy the designated object when the event ends.
        DestroyEndObject();

        // Hide the wallet model.
        if (walletVisualModel != null)
        {
            walletVisualModel.SetActive(false);
        }

        // Change the player's material or color.
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
            Vector3 speechPos = spawnLocation != null
                ? spawnLocation.position
                : transform.position;

            Quaternion speechRot = spawnLocation != null
                ? spawnLocation.rotation
                : Quaternion.identity;

            Instantiate(manSpeechPrefab, speechPos, speechRot);
        }

        // Spawn both interaction objects.
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
        isEventActive = false;
        playerMadeChoice = true;

        CloseUI();

        // Play sound only when the timer expires.
        if (returnSound != null)
        {
            AudioSource.PlayClipAtPoint(
                returnSound,
                transform.position,
                soundVolume
            );
        }

        // Destroy the designated object.
        DestroyEndObject();

        // Spawn the first timeout object.
        SpawnObject(
            objectToSpawnOnTimeout,
            timeoutSpawnLocation
        );

        // Spawn the second timeout object.
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

        Debug.Log("[StoreEvent2] Timer expired. Timeout objects spawned.");

        DisableEventInteraction();
    }

    private void SpawnObject(GameObject prefab, Transform location)
    {
        if (prefab == null)
            return;

        Vector3 spawnPos = location != null
            ? location.position
            : transform.position;

        Quaternion spawnRot = location != null
            ? location.rotation
            : Quaternion.identity;

        Instantiate(prefab, spawnPos, spawnRot);

        Debug.Log("[StoreEvent2] Spawned object: " + prefab.name);
    }

    private void DestroyEndObject()
    {
        if (objectToDestroyOnEnd != null)
        {
            Destroy(objectToDestroyOnEnd);
            objectToDestroyOnEnd = null;

            Debug.Log("[StoreEvent2] End object destroyed!");
        }
    }

    private void DisableEventInteraction()
    {
        // Disable trigger collider so it can't re-trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Turn off the highlight script permanently when the event concludes
        if (walletHighlight != null)
        {
            walletHighlight.enabled = false;
        }

        enabled = false;
    }

    private void CloseUI()
    {
        if (timerFillBar != null)
        {
            timerFillBar.fillAmount = 0f;
            timerFillBar.gameObject.SetActive(false);
        }
    }
}