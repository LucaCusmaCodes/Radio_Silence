using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BusSeatEvent : MonoBehaviour
{
    [Header("Core References")]
    public Transform playerTransform;
    public GameObject foodObject;

    [Header("Trigger & Timer Settings")]
    public float timeLimit = 8.0f;          // How long the player has before timer runs out
    public GameObject walletTimerBar;       // The Wallet Timer UI parent object
    public Image timerFillImage;            // The Image component set to 'Filled'

    [Header("Interaction Settings")]
    public float interactionDistance = 4.0f; // Distance to press E near food
    public KeyCode interactKey = KeyCode.E;

    [Header("Manager Settings")]
    public GameObject managerObject;
    public Transform managerTargetPosition;
    public float managerWalkSpeed = 2.5f;

    [Header("UI Prefabs & Spawning")]
    public Transform uiCanvasParent;             // Main Screen-Space Canvas
    public GameObject helpedCanvasPrefab;        // Spawned when player presses E in time
    public GameObject managerArrivalCanvasPrefab;// Spawned when manager reaches the counter
    public GameObject timeoutObjectPrefab;       // Spawned when timer runs out

    // Internal State
    private bool eventStarted = false;
    private bool eventFinished = false;
    private float currentTime;

    private void Start()
    {
        currentTime = timeLimit;

        // Ensure UI and Manager start hidden
        if (walletTimerBar != null) walletTimerBar.SetActive(false);
        if (managerObject != null) managerObject.SetActive(false);

        // Auto-find player if unassigned
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Trigger the event when the Player enters the zone
        if (!eventStarted && other.CompareTag("Player"))
        {
            eventStarted = true;

            if (walletTimerBar != null)
            {
                walletTimerBar.SetActive(true);
            }

            Debug.Log("[BusSeatEvent] Event started! Wallet timer active.");
        }
    }

    private void Update()
    {
        // Only run if the trigger was entered and the event hasn't finished yet
        if (!eventStarted || eventFinished) return;
        if (playerTransform == null || foodObject == null) return;

        // 1. Countdown Timer
        currentTime -= Time.deltaTime;
        if (timerFillImage != null)
        {
            timerFillImage.fillAmount = Mathf.Clamp01(currentTime / timeLimit);
        }

        // 2. Check Proximity to Food & E Key Press
        float distanceToFood = Vector3.Distance(playerTransform.position, foodObject.transform.position);
        if (distanceToFood <= interactionDistance)
        {
            if (Input.GetKeyDown(interactKey))
            {
                ExecuteHelpedOutcome();
                return;
            }
        }

        // 3. Check Timer Expiration (Timeout)
        if (currentTime <= 0f)
        {
            ExecuteTimeoutOutcome();
        }
    }

    // --- OUTCOME 1: PLAYER HELPS IN TIME ---
    private void ExecuteHelpedOutcome()
    {
        eventFinished = true;
        HideWalletTimer();

        if (foodObject != null) foodObject.SetActive(false);

        // Spawn Helped/Thanks UI
        SpawnUI(helpedCanvasPrefab);

        // Activate Manager and start walking
        if (managerObject != null && managerTargetPosition != null)
        {
            managerObject.SetActive(true);
            StartCoroutine(WalkManagerToCounter());
        }

        Debug.Log("[BusSeatEvent] Player helped the customer!");
    }

    private IEnumerator WalkManagerToCounter()
    {
        while (Vector3.Distance(managerObject.transform.position, managerTargetPosition.position) > 0.1f)
        {
            managerObject.transform.position = Vector3.MoveTowards(
                managerObject.transform.position,
                managerTargetPosition.position,
                Time.deltaTime * managerWalkSpeed
            );
            yield return null;
        }

        managerObject.transform.position = managerTargetPosition.position;

        // Spawn Manager Arrival UI
        SpawnUI(managerArrivalCanvasPrefab);

        Debug.Log("[BusSeatEvent] Manager arrived at counter!");
    }

    // --- OUTCOME 2: TIMER RUNS OUT (TIMEOUT) ---
    private void ExecuteTimeoutOutcome()
    {
        eventFinished = true;
        HideWalletTimer();

        // 1. Spawn timeout object (e.g., sad customer canvas)
        SpawnUI(timeoutObjectPrefab);

        // 2. Penalties & Consequences
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice("Ignored Customer", 1);
        }

        if (VignetteController.Instance != null)
        {
            VignetteController.Instance.IncreaseDarkness();
        }

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ApplyPitchDropStep();
        }

        Debug.Log("[BusSeatEvent] Timer expired! Spawned timeout object and applied penalties.");
    }

    private void HideWalletTimer()
    {
        if (walletTimerBar != null)
        {
            walletTimerBar.SetActive(false);
        }
    }

    private void SpawnUI(GameObject prefab)
    {
        if (prefab != null && uiCanvasParent != null)
        {
            GameObject spawnedObj = Instantiate(prefab, uiCanvasParent);
            RectTransform rect = spawnedObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }
        }
    }
}