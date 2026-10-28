using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoreEvent2 : MonoBehaviour
{
    [Header("UI Component")]
    public Image timerFillBar;

    [Header("World References")]
    public GameObject walletVisualModel;

    [Header("Event Settings")]
    public float timeLimitSeconds = 5.0f;
    public KeyCode returnKey = KeyCode.E;

    [Header("Man Speech Object")]
    public GameObject manSpeechPrefab;
    public Transform spawnLocation;

    [Header("Spawn Object On Interaction")]
    public GameObject objectToSpawn;
    public Transform objectSpawnLocation;

    [Header("Manager Walk Settings")]
    public GameObject managerPrefab;              // Manager prefab spawned on interaction
    public Transform managerSpawnLocation;        // Where the manager spawns
    public Transform managerTargetPosition;       // Where the manager walks to
    public float managerWalkSpeed = 3.0f;         // Walking speed

    [Header("Manager Arrival Settings")]
    public GameObject objectToSpawnOnArrival;     // Spawned when manager reaches destination
    public Transform arrivalSpawnLocation;        // Where the arrival object spawns

    [Header("End of Event Object Destruction")]
    public GameObject objectToDestroyOnEnd;       // Destroyed when event finishes (either way)

    [Header("Spawn Object On Timeout")]
    public GameObject objectToSpawnOnTimeout;     // Spawned when timer runs out
    public Transform timeoutSpawnLocation;        // Where timeout object spawns

    [Header("Player Visual Swap")]
    public Renderer playerRenderer;
    public Material angryMaterial;
    public bool useColorSwapInstead = false;
    public Color angryColor = Color.red;

    [Header("Consequences")]
    public int returnScoreChange = 0;
    public int keepScoreChange = 1;

    [Header("Audio")]
    public AudioClip returnSound;
    [Range(0f, 1f)]
    public float soundVolume = 1.0f;

    private float timer;
    private bool isEventActive = false;
    private bool playerMadeChoice = false;

    private void Awake()
    {
        if (timerFillBar != null)
        {
            timerFillBar.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerMadeChoice && !isEventActive)
        {
            StartEvent();
        }
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

        Debug.Log("[StoreEvent2] Trigger activated. Timer started!");
    }

    private void Update()
    {
        if (!isEventActive) return;

        timer -= Time.deltaTime;

        if (timerFillBar != null && timeLimitSeconds > 0f)
        {
            timerFillBar.fillAmount = Mathf.Clamp01(timer / timeLimitSeconds);
        }

        if (Input.GetKeyDown(returnKey))
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
        DestroyEndObject();

        if (walletVisualModel != null)
        {
            walletVisualModel.SetActive(false);
        }

        // Player visual swap
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

        // Spawn man speech
        if (manSpeechPrefab != null)
        {
            Vector3 speechPos = spawnLocation != null ? spawnLocation.position : transform.position;
            Quaternion speechRot = spawnLocation != null ? spawnLocation.rotation : Quaternion.identity;
            Instantiate(manSpeechPrefab, speechPos, speechRot);
        }

        // Spawn interaction object
        SpawnObject();

        // Spawn and start walking manager
        SpawnAndWalkManager();

        // Audio
        if (returnSound != null)
        {
            AudioSource.PlayClipAtPoint(returnSound, transform.position, soundVolume);
        }

        // Score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice("Returned Wallet", returnScoreChange);
        }

        Debug.Log("[StoreEvent2] Wallet returned.");
        DisableEventInteraction();
    }

    private void SpawnObject()
    {
        if (objectToSpawn == null) return;

        Vector3 spawnPos = objectSpawnLocation != null ? objectSpawnLocation.position : transform.position;
        Quaternion spawnRot = objectSpawnLocation != null ? objectSpawnLocation.rotation : Quaternion.identity;

        Instantiate(objectToSpawn, spawnPos, spawnRot);
    }

    private void SpawnAndWalkManager()
    {
        if (managerPrefab == null) return;

        Vector3 spawnPos = managerSpawnLocation != null ? managerSpawnLocation.position : transform.position;
        Quaternion spawnRot = managerSpawnLocation != null ? managerSpawnLocation.rotation : Quaternion.identity;

        GameObject spawnedManager = Instantiate(managerPrefab, spawnPos, spawnRot);

        if (managerTargetPosition != null)
        {
            StartCoroutine(WalkManagerRoutine(spawnedManager, managerTargetPosition));
        }
    }

    private IEnumerator WalkManagerRoutine(GameObject manager, Transform target)
    {
        // Make sure we ignore any physics and move purely via transform
        while (manager != null && target != null)
        {
            float distance = Vector3.Distance(manager.transform.position, target.position);

            if (distance <= 0.1f) break;

            manager.transform.position = Vector3.MoveTowards(
                manager.transform.position,
                target.position,
                managerWalkSpeed * Time.deltaTime
            );

            yield return null;
        }

        if (manager != null && target != null)
        {
            manager.transform.position = target.position;
            SpawnArrivalObject();
            Debug.Log("[StoreEvent2] Manager reached destination and spawned arrival object.");
        }
    }

    private void SpawnArrivalObject()
    {
        if (objectToSpawnOnArrival == null) return;

        Vector3 spawnPos = arrivalSpawnLocation != null ? arrivalSpawnLocation.position : managerTargetPosition.position;
        Quaternion spawnRot = arrivalSpawnLocation != null ? arrivalSpawnLocation.rotation : Quaternion.identity;

        Instantiate(objectToSpawnOnArrival, spawnPos, spawnRot);
    }

    private void OnTimerExpired()
    {
        isEventActive = false;
        playerMadeChoice = true;

        CloseUI();
        DestroyEndObject();
        SpawnTimeoutObject();

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
            GameManager.Instance.ProcessChoice("Ignored Wallet", keepScoreChange);
        }

        Debug.Log("[StoreEvent2] Time expired.");
        DisableEventInteraction();
    }

    private void SpawnTimeoutObject()
    {
        if (objectToSpawnOnTimeout == null) return;

        Vector3 timeoutPos = timeoutSpawnLocation != null ? timeoutSpawnLocation.position : transform.position;
        Quaternion timeoutRot = timeoutSpawnLocation != null ? timeoutSpawnLocation.rotation : Quaternion.identity;

        Instantiate(objectToSpawnOnTimeout, timeoutPos, timeoutRot);
    }

    private void DestroyEndObject()
    {
        if (objectToDestroyOnEnd != null)
        {
            Destroy(objectToDestroyOnEnd);
        }
    }

    private void DisableEventInteraction()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
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