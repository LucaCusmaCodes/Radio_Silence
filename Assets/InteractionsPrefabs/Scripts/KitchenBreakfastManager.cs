
using UnityEngine;
using UnityEngine.UI;

public class KitchenBreakfastManager : MonoBehaviour
{
    public static KitchenBreakfastManager Instance { get; private set; }

    [Header("UI Timer Settings")]
    public Image timerFillBar;
    public float decisionTimeLimit = 7.0f;

    [Header("UI Spawn Settings")]
    public GameObject dialogueCanvasPrefab;
    public Transform uiSpawnPoint;

    [Header("Breakfast Plates")]
    public BreakfastPlate plate1;
    public BreakfastPlate plate2;

    private bool isCookingActive = false;
    private int firstPlateID = 0;
    private int platesMade = 0;
    private float currentTimer;

    private void Awake()
    {
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
        firstPlateID = 0;
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
        if (!isCookingActive)
            return;

        currentTimer -= Time.deltaTime;

        if (timerFillBar != null && decisionTimeLimit > 0f)
        {
            timerFillBar.fillAmount =
                Mathf.Clamp01(currentTimer / decisionTimeLimit);
        }

        if (currentTimer <= 0f)
        {
            OnTimerExpired();
        }
    }

    public void OnPlateInteracted(int plateID)
    {
        if (platesMade >= 2 ||
            (!isCookingActive && platesMade == 1))
        {
            return;
        }

        if (platesMade == 0)
        {
            firstPlateID = plateID;
            platesMade = 1;
            isCookingActive = true;
            currentTimer = decisionTimeLimit;

            if (timerFillBar != null)
            {
                timerFillBar.gameObject.SetActive(true);
                timerFillBar.fillAmount = 1f;
            }

            Debug.Log(
                "[KitchenBreakfastManager] First plate selected: " +
                plateID + ". Timer started!"
            );
        }
        else if (isCookingActive &&
                 platesMade == 1 &&
                 plateID != firstPlateID)
        {
            platesMade = 2;
            isCookingActive = false;

            if (timerFillBar != null)
            {
                timerFillBar.fillAmount = 0f;
                timerFillBar.gameObject.SetActive(false);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ProcessChoice(
                    "You took the time to make breakfast for both of you.",
                    0
                );

                Debug.Log(
                    "[Breakfast Event Complete] Moral choice. Total Score: " +
                    GameManager.Instance.internalScore
                );
            }

            SpawnDialogueCanvas();
        }
    }

    private void OnTimerExpired()
    {
        isCookingActive = false;

        // Prevent any additional plate interactions.
        platesMade = 2;

        // Disable only the plate that was NOT selected.
        if (firstPlateID == 1 && plate2 != null)
        {
            plate2.DisableInteraction();
        }
        else if (firstPlateID == 2 && plate1 != null)
        {
            plate1.DisableInteraction();
        }

        if (timerFillBar != null)
        {
            timerFillBar.fillAmount = 0f;
            timerFillBar.gameObject.SetActive(false);
        }

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
                "You only made breakfast for yourself.",
                1
            );

            Debug.Log(
                "[Breakfast Event Complete] Selfish choice. Total Score: " +
                GameManager.Instance.internalScore
            );
        }

        SpawnDialogueCanvas();
    }

    private void SpawnDialogueCanvas()
    {
        if (dialogueCanvasPrefab != null && uiSpawnPoint != null)
        {
            GameObject spawnedCanvas = Instantiate(
                dialogueCanvasPrefab,
                uiSpawnPoint.position,
                uiSpawnPoint.rotation
            );

            spawnedCanvas.transform.SetParent(uiSpawnPoint, false);

            Debug.Log(
                "[KitchenBreakfastManager] Dialogue canvas spawned."
            );
        }
        else
        {
            Debug.LogWarning(
                "Dialogue Canvas Prefab or UI Spawn Point is unassigned!"
            );
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
