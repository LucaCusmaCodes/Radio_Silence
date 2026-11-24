
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WalletEventControllerWithNPC : MonoBehaviour
{
    [Header("UI Component")]
    public Image timerFillBar;

    [Header("World References")]
    public GameObject walletVisualModel;
    public InteractableHighlight walletHighlight;

    [Header("Event Settings")]
    public float timeLimitSeconds = 5.0f;
    public KeyCode returnKey = KeyCode.E;

    [Header("Man Speech Object")]
    public GameObject manSpeechPrefab;
    public Transform spawnLocation;
    public Transform spawnParentTransform;

    [Header("NPC Rotation")]
    public Transform npcToRotate;
    public Transform playerTransform;
    public float rotationSpeed = 3f;

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
    [Range(0f, 1f)] public float soundVolume = 1.0f;

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

        Debug.Log("[WalletEvent] Trigger activated. Timer started!");
    }

    private void Update()
    {
        if (!isEventActive) return;

        timer -= Time.deltaTime;

        if (timerFillBar != null && timeLimitSeconds > 0)
        {
            timerFillBar.fillAmount = Mathf.Clamp01(timer / timeLimitSeconds);
        }

        // Player presses E: Return wallet
        if (Input.GetKeyDown(returnKey))
        {
            OnWalletReturned();
        }
        // Timer expires: Selfish choice
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

        // NEW: Turn NPC toward player
        if (npcToRotate != null && playerTransform != null)
        {
            StartCoroutine(RotateNPCToPlayer());
        }

        // Hide wallet mesh on return
        if (walletVisualModel != null)
        {
            walletVisualModel.SetActive(false);
        }

        // PLAYER MATERIAL/COLOR SWAP
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

        // Spawn existing angry speech object / canvas
        if (manSpeechPrefab != null)
        {
            Vector3 spawnPos = (spawnLocation != null)
                ? spawnLocation.position
                : transform.position;

            Quaternion spawnRot = (spawnLocation != null)
                ? spawnLocation.rotation
                : Quaternion.identity;

            if (spawnParentTransform != null)
            {
                Instantiate(manSpeechPrefab, spawnPos, spawnRot, spawnParentTransform);
            }
            else
            {
                Instantiate(manSpeechPrefab, spawnPos, spawnRot);
            }
        }

        // Play return clip
        if (returnSound != null)
        {
            AudioSource.PlayClipAtPoint(returnSound, transform.position, soundVolume);
        }

        // Process score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice("Returned Wallet", returnScoreChange);
        }

        Debug.Log("[WalletEvent] Wallet returned. Player material swapped and man's speech spawned.");

        DisableEventInteraction();
    }

    // NEW: Smoothly rotate NPC toward player
    private IEnumerator RotateNPCToPlayer()
    {
        Vector3 direction = playerTransform.position - npcToRotate.position;

        // Prevent NPC from tilting vertically
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            yield break;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(npcToRotate.rotation, targetRotation) > 0.5f)
        {
            npcToRotate.rotation = Quaternion.Slerp(
                npcToRotate.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Keep NPC facing player
        npcToRotate.rotation = targetRotation;
    }

    private void OnTimerExpired()
    {
        isEventActive = false;
        playerMadeChoice = true;

        CloseUI();

        // Wallet stays on the ground!

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
        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        if (walletHighlight != null)
        {
            walletHighlight.enabled = false;
        }

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
