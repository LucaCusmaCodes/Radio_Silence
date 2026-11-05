using UnityEngine;
using UnityEngine.UI;

public class BusSeatChoiceController : MonoBehaviour
{
    [Header("UI Component")]
    public Image timerFillBar;                // Drag your timer bar Image here directly

    [Header("World References")]
    public GameObject personVisualModel;      // The person standing near the seat
    public InteractableHighlight personHighlight; // Drag the person's InteractableHighlight component here

    [Header("Person Visual Swap")]
    public Renderer personRenderer;            // Drag the Person's Renderer here
    public Material angryMaterial;             // (Option A) Drag an angry material here to swap materials
    public bool useColorSwapInstead = false;  // Check this box if you'd rather change the material color directly
    public Color angryColor = Color.red;       // (Option B) Color used if useColorSwapInstead is checked

    [Header("Event Settings")]
    public float timeLimitSeconds = 5.0f;      // Timer duration to decide
    public KeyCode interactKey = KeyCode.E;     // Key to interact with the person

    [Header("Person Speech Object")]
    public GameObject personSpeechPrefab;      // Drag your angry speech Canvas/Object prefab here
    public Transform speechSpawnPoint;         // Optional: Specific spawn position. If empty, uses person's position.

    [Header("Consequences & Scoring")]
    public int giveSeatScoreChange = 0;        // Give seat (Altruistic): 0 points
    public int keepSeatScoreChange = 1;        // Keep seat / Timeout (Selfish): +1 point

    [Header("Audio")]
    public AudioClip interactSound;            // Play when interacting with the person
    [Range(0f, 1f)] public float soundVolume = 1.0f;

    private float timer;
    private bool isEventActive = false;
    private bool playerMadeChoice = false;
    private bool isPlayerInTrigger = false;

    private void Awake()
    {
        // Instantly hide the timer bar when the scene loads
        if (timerFillBar != null)
        {
            timerFillBar.gameObject.SetActive(false);
        }

        // Auto-assign renderer if not set manually in Inspector
        if (personRenderer == null && personVisualModel != null)
        {
            personRenderer = personVisualModel.GetComponentInChildren<Renderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerMadeChoice)
        {
            isPlayerInTrigger = true;

            if (!isEventActive)
            {
                StartEvent();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
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

        Debug.Log("[BusSeatChoice] Event started! Timer running.");
    }

    private void Update()
    {
        if (!isEventActive || playerMadeChoice) return;

        // Tick countdown timer
        timer -= Time.deltaTime;

        // Update fill bar UI
        if (timerFillBar != null && timeLimitSeconds > 0)
        {
            timerFillBar.fillAmount = Mathf.Clamp01(timer / timeLimitSeconds);
        }

        // Choice 1: Interacted with the person in range -> Gave Seat
        if (isPlayerInTrigger && Input.GetKeyDown(interactKey))
        {
            OnGiveUpSeat();
        }
        // Choice 2: Timer expired -> Kept Seat
        else if (timer <= 0f)
        {
            OnKeepSeat();
        }
    }

    private void OnGiveUpSeat()
    {
        isEventActive = false;
        playerMadeChoice = true;

        CloseUI();

        // 1. Disable highlight FIRST so it doesn't overwrite our material changes on shutdown
        if (personHighlight != null)
        {
            personHighlight.enabled = false;
        }

        // 2. --- PERSON VISUAL / MATERIAL SWAP (All Slots) ---
        if (personRenderer != null)
        {
            if (useColorSwapInstead)
            {
                Material[] mats = personRenderer.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i].HasProperty("_BaseColor"))
                    {
                        mats[i].SetColor("_BaseColor", angryColor);
                    }
                    else if (mats[i].HasProperty("_Color"))
                    {
                        mats[i].color = angryColor;
                    }
                }
                personRenderer.materials = mats;
            }
            else if (angryMaterial != null)
            {
                Material[] angryMats = new Material[personRenderer.materials.Length];
                for (int i = 0; i < angryMats.Length; i++)
                {
                    angryMats[i] = angryMaterial;
                }
                personRenderer.materials = angryMats;
            }
        }

        // 3. Spawn angry speech bubble/object
        if (personSpeechPrefab != null)
        {
            Vector3 spawnPos = (speechSpawnPoint != null)
                ? speechSpawnPoint.position
                : ((personVisualModel != null) ? personVisualModel.transform.position : transform.position);

            Quaternion spawnRot = (speechSpawnPoint != null)
                ? speechSpawnPoint.rotation
                : Quaternion.identity;

            Instantiate(personSpeechPrefab, spawnPos, spawnRot);
        }

        // 4. Play sound
        PlayAudio();

        // 5. Process score (0 points)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice("Gave Up Bus Seat", giveSeatScoreChange);
        }

        Debug.Log("[BusSeatChoice] Choice made: Gave up seat. Person material swapped, angry speech spawned.");

        DisableEventInteraction();
    }

    private void OnKeepSeat()
    {
        isEventActive = false;
        playerMadeChoice = true;

        CloseUI();

        // 1. Trigger Music Pitch Drop
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ApplyPitchDropStep();
        }

        // 2. Trigger Screen Darkening
        if (VignetteController.Instance != null)
        {
            VignetteController.Instance.IncreaseDarkness();
        }

        // 3. Process score (+1 point)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice("Kept Bus Seat (Timer Expired)", keepSeatScoreChange);
        }

        Debug.Log("[BusSeatChoice] Time expired: Kept seat. Pitch dropped, screen darkened, +1 point.");

        DisableEventInteraction();
    }

    private void DisableEventInteraction()
    {
        // Disable trigger collider so it can't re-trigger
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Ensure highlight is off
        if (personHighlight != null)
        {
            personHighlight.enabled = false;
        }

        // Disable script execution
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

    private void PlayAudio()
    {
        if (interactSound != null)
        {
            AudioSource.PlayClipAtPoint(interactSound, transform.position, soundVolume);
        }
    }
}