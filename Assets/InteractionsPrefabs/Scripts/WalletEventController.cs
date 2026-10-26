using UnityEngine;
using UnityEngine.UI;

public class WalletEventController : MonoBehaviour
{
    [Header("UI Component")]
    public Image timerFillBar;                // Drag your timer bar Image here directly

    [Header("World References")]
    public GameObject walletVisualModel;      // The physical wallet mesh on the ground

    [Header("Event Settings")]
    public float timeLimitSeconds = 5.0f;      // Total countdown time to decide
    public KeyCode returnKey = KeyCode.E;       // Key to give the wallet back

    [Header("Man Speech Object")]
    public GameObject manSpeechPrefab;         // Drag your timed speech Canvas/Object prefab here
    public Transform spawnLocation;            // Optional: Drag a specific spawn point. If empty, spawns at wallet.

    [Header("Player Visual Swap")]
    public Renderer playerRenderer;            // Drag your Player capsule/mesh Renderer here
    public Material angryMaterial;             // (Option A) Drag an angry material here to swap materials
    public bool useColorSwapInstead = false;  // Check this box if you'd rather change the material color
    public Color angryColor = Color.red;       // (Option B) Color used if useColorSwapInstead is checked

    [Header("Consequences")]
    public int returnScoreChange = 0;           // Altruistic choice: 0 points
    public int keepScoreChange = 1;             // Selfish choice: +1 point

    [Header("Audio")]
    public AudioClip returnSound;               // Drag your return sound clip here
    [Range(0f, 1f)] public float soundVolume = 1.0f;

    private float timer;
    private bool isEventActive = false;
    private bool playerMadeChoice = false;

    private void Awake()
    {
        // Instantly hide the timer bar when the scene loads
        if (timerFillBar != null)
        {
            timerFillBar.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Show timer bar and start countdown only when entering trigger area
        if (other.CompareTag("Player") && !playerMadeChoice && !isEventActive)
        {
            StartEvent();
        }
    }

    public void StartEvent()
    {
        isEventActive = true;
        timer = timeLimitSeconds;

        // Turn on the timer bar image and fill it
        if (timerFillBar != null)
        {
            timerFillBar.gameObject.SetActive(true);
            timerFillBar.fillAmount = 1f;
        }

        Debug.Log("[WalletEvent] Trigger activated. Timer started!");
    }

    private void Update()
    {
        if (!isEventActive) return;

        // Tick timer down
        timer -= Time.deltaTime;

        // Update fill bar
        if (timerFillBar != null && timeLimitSeconds > 0)
        {
            timerFillBar.fillAmount = Mathf.Clamp01(timer / timeLimitSeconds);
        }

        // Choice 1: Player presses E -> Return wallet
        if (Input.GetKeyDown(returnKey))
        {
            OnWalletReturned();
        }
        // Choice 2: Timer expires -> Selfish choice
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

        // Hide wallet mesh on return
        if (walletVisualModel != null)
        {
            walletVisualModel.SetActive(false);
        }

        // --- PLAYER MATERIAL/COLOR SWAP ---
        if (playerRenderer != null)
        {
            if (useColorSwapInstead)
            {
                // Option B: Change the existing material's tint color
                playerRenderer.material.color = angryColor;
            }
            else if (angryMaterial != null)
            {
                // Option A: Assign a whole new angry Material
                playerRenderer.material = angryMaterial;
            }
        }

        // Spawn the man's angry speech object / canvas
        if (manSpeechPrefab != null)
        {
            Vector3 spawnPos = (spawnLocation != null) ? spawnLocation.position : transform.position;
            Quaternion spawnRot = (spawnLocation != null) ? spawnLocation.rotation : Quaternion.identity;

            Instantiate(manSpeechPrefab, spawnPos, spawnRot);
        }

        // Play return clip at world position
        if (returnSound != null)
        {
            AudioSource.PlayClipAtPoint(returnSound, transform.position, soundVolume);
        }

        // Process score in GameManager (0 points)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice("Returned Wallet", returnScoreChange);
        }

        Debug.Log("[WalletEvent] Wallet returned. Player material swapped and man's speech spawned.");

        DisableEventInteraction();
    }

    private void OnTimerExpired()
    {
        isEventActive = false;
        playerMadeChoice = true;

        CloseUI();

        // NOTE: walletVisualModel is kept active so the wallet stays on the ground!

        // Atmospheric penalties
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

        Debug.Log("[WalletEvent] Time expired. Wallet left on ground, atmospheric penalties applied.");

        DisableEventInteraction();
    }

    private void DisableEventInteraction()
    {
        // Disable trigger collider so the player cannot re-trigger the event
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Disable script execution without destroying the wallet GameObject
        this.enabled = false;
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