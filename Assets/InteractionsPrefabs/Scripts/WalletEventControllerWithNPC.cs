
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

    [Header("Player Visual Swap")]
    public Renderer playerRenderer;
    public Material angryMaterial;
    public bool useColorSwapInstead = false;
    public Color angryColor = Color.red;

    [Header("NPC Walking")]
    public Transform npcTransform;
    public Animator npcAnimator;
    public Transform walkDestination;
    public float walkSpeed = 2f;
    public float turnSpeed = 180f;

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
        if (other.CompareTag("Player") &&
            !playerMadeChoice && !isEventActive)
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
            timerFillBar.fillAmount =
                Mathf.Clamp01(timer / timeLimitSeconds);
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

        // Start NPC walking.
        if (npcTransform != null && walkDestination != null)
        {
            StartCoroutine(NPCWalk());
        }
        else
        {
            Debug.LogError(
                "[WalletEvent] NPC Transform or Walk Destination is missing!"
            );
        }

        // Hide wallet mesh on return.
        if (walletVisualModel != null)
        {
            walletVisualModel.SetActive(false);
        }

        // Player material/color swap.
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

        // Spawn existing angry speech object / canvas.
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
                Instantiate(
                    manSpeechPrefab,
                    spawnPos,
                    spawnRot,
                    spawnParentTransform
                );
            }
            else
            {
                Instantiate(manSpeechPrefab, spawnPos, spawnRot);
            }
        }

        // Play return clip.
        if (returnSound != null)
        {
            AudioSource.PlayClipAtPoint(
                returnSound,
                transform.position,
                soundVolume
            );
        }

        // Process score.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice(
                "Returned Wallet",
                returnScoreChange
            );
        }

        Debug.Log("[WalletEvent] Wallet returned. Speech spawned.");

        // Disable interaction without disabling this script.
        DisableEventInteraction();
    }

    private IEnumerator NPCWalk()
    {
        // Start walking animation.
        if (npcAnimator != null)
        {
            npcAnimator.SetBool("IsWalking", true);
        }

        while (Vector3.Distance(
            npcTransform.position,
            walkDestination.position) > 0.1f)
        {
            Vector3 direction =
                walkDestination.position - npcTransform.position;

            direction.y = 0f;

            // Rotate NPC toward destination.
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                npcTransform.rotation = Quaternion.RotateTowards(
                    npcTransform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                );
            }

            // Move NPC toward destination.
            npcTransform.position = Vector3.MoveTowards(
                npcTransform.position,
                walkDestination.position,
                walkSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Stop walking animation.
        if (npcAnimator != null)
        {
            npcAnimator.SetBool("IsWalking", false);
        }

        Debug.Log("[WalletEvent] NPC reached destination!");
    }

    private void OnTimerExpired()
    {
        isEventActive = false;
        playerMadeChoice = true;

        CloseUI();

        // Wallet stays on the ground.

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

        Debug.Log("[WalletEvent] Time expired. Wallet left on ground.");

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

        // IMPORTANT:
        // Do not disable this script here.
        // Disabling it would stop the NPC walking coroutine.
    }

    private void CloseUI()
    {
        if (timerFillBar != null)
        {
            timerFillBar.fillAmount = 0f;

            Debug.Log("Timer: " + timer +
                      " | Fill: " + timerFillBar.fillAmount);

            timerFillBar.gameObject.SetActive(false);
        }
    }
}