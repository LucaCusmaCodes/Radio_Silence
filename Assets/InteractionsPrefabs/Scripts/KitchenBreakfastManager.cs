using UnityEngine;
using UnityEngine.UI;

public class KitchenBreakfastManager : MonoBehaviour
{
    public static KitchenBreakfastManager Instance { get; private set; }

    [Header("UI Timer Settings")]
    public Image timerFillBar;
    public float decisionTimeLimit = 7.0f;

    [Header("UI Spawn Settings")]
    public GameObject dialogueCanvasPrefab; // Drag your Dialogue Canvas Prefab here
    public Transform uiSpawnPoint;          // Drag your uiSpawnPoint GameObject here

    [Header("State")]
    private bool isCookingActive = false;
    private int platesMade = 0;
    private float currentTimer;

    private void Awake()
    {
        // Enforce singular Instance for this scene load
        Instance = this;
        ResetState();
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        isCookingActive = false;
        platesMade = 0;
        currentTimer = 0f;

        if (timerFillBar != null)
        {
            timerFillBar.fillAmount = 0f;
            timerFillBar.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isCookingActive) return;

        currentTimer -= Time.deltaTime;

        if (timerFillBar != null && decisionTimeLimit > 0)
        {
            timerFillBar.fillAmount = Mathf.Clamp01(currentTimer / decisionTimeLimit);
        }

        if (currentTimer <= 0f)
        {
            OnTimerExpired();
        }
    }

    public void OnPlateInteracted(int plateID)
    {
        if (platesMade == 0)
        {
            // First plate made! Start timer
            platesMade = 1;
            isCookingActive = true;
            currentTimer = decisionTimeLimit;

            if (timerFillBar != null)
            {
                timerFillBar.gameObject.SetActive(true);
                timerFillBar.fillAmount = 1f;
            }
        }
        else if (isCookingActive && platesMade == 1 && plateID == 2)
        {
            // Second plate made before timer expires
            platesMade = 2;
            isCookingActive = false;

            if (timerFillBar != null)
            {
                timerFillBar.gameObject.SetActive(false);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ProcessChoice("You took the time to make breakfast for both of you.", 0);
                Debug.Log($"[Breakfast Event Complete] Choice: Moral (2 Plates) | Total Score: {GameManager.Instance.internalScore}");
            }

            // Spawn dialogue canvas for successful choice
            SpawnDialogueCanvas();
        }
    }

    private void OnTimerExpired()
    {
        isCookingActive = false;

        if (timerFillBar != null)
        {
            timerFillBar.gameObject.SetActive(false);
        }

        // Apply permanent darkness & pitch shift
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
            GameManager.Instance.ProcessChoice("You only made breakfast for yourself.", 1);
            Debug.Log($"[Breakfast Event Complete] Choice: Selfish (1 Plate) | Total Score: {GameManager.Instance.internalScore}");
        }

        // Spawn dialogue canvas when timer runs out (selfish path)
        SpawnDialogueCanvas();
    }

    private void SpawnDialogueCanvas()
    {
        if (dialogueCanvasPrefab != null && uiSpawnPoint != null)
        {
            GameObject spawnedCanvas = Instantiate(dialogueCanvasPrefab, uiSpawnPoint.position, uiSpawnPoint.rotation);
            spawnedCanvas.transform.SetParent(uiSpawnPoint, false);
            Debug.Log("[KitchenBreakfastManager] Dialogue canvas spawned successfully at uiSpawnPoint.");
        }
        else
        {
            Debug.LogWarning("⚠️ Dialogue Canvas Prefab or UI Spawn Point is unassigned in KitchenBreakfastManager!");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}