using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderInteractable : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip interactionSound;
    [Range(0f, 1f)]
    public float soundVolume = 1.0f;

    [Header("Fader Reference")]
    public Fader screenFader;

    [Header("Teleportation")]
    public Transform teleportTarget;

    [Header("Highlight Reference")]
    public InteractableHighlight ladderHighlight; // Drag your ladder's InteractableHighlight component here

    private GameObject playerRef;

    [Header("Settings")]
    public float fadeDuration = 1.0f;
    public int ladderScoreChange = 1;

    private bool playerInRange = false;
    private bool hasInteracted = false;

    private void Awake()
    {
        // Ensure highlight is off when the ladder appears
        if (ladderHighlight != null)
        {
            ladderHighlight.enabled = false;
        }
    }

    private void Update()
    {
        if (playerInRange &&
            !hasInteracted &&
            Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(InteractAndTeleportRoutine());
        }
    }

    private IEnumerator InteractAndTeleportRoutine()
    {
        hasInteracted = true;

        // Turn off highlight immediately upon interacting
        if (ladderHighlight != null)
        {
            ladderHighlight.enabled = false;
        }

        // Play the interaction sound independently
        if (interactionSound != null)
        {
            GameObject soundObject = new GameObject("LadderInteractionSound");
            soundObject.transform.position = transform.position;

            AudioSource source = soundObject.AddComponent<AudioSource>();
            source.clip = interactionSound;
            source.volume = soundVolume;
            source.spatialBlend = 0f;
            source.playOnAwake = false;
            source.loop = false;

            source.Play();

            Destroy(soundObject, interactionSound.length + 0.5f);
        }
        else
        {
            Debug.LogWarning("[LadderInteractable] No interaction sound assigned!");
        }

        // Process score in GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice("Climbed Ladder", ladderScoreChange);
        }

        // Fade the screen to black
        if (screenFader != null)
        {
            yield return StartCoroutine(
                screenFader.FadeRoutine(1f, fadeDuration)
            );
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        // Teleport the player safely
        if (playerRef != null && teleportTarget != null)
        {
            CharacterController cc = playerRef.GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.enabled = false;
            }

            playerRef.transform.position = teleportTarget.position;
            playerRef.transform.rotation = teleportTarget.rotation;

            if (cc != null)
            {
                cc.enabled = true;
            }
        }

        // Fade screen back in
        if (screenFader != null)
        {
            yield return StartCoroutine(
                screenFader.FadeRoutine(0f, fadeDuration)
            );
        }

        // Disable trigger collider so it can't be used again
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            playerInRange = true;
            playerRef = other.gameObject;

            // Turn on the highlight when in range
            if (ladderHighlight != null)
            {
                ladderHighlight.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // Turn off the highlight when walking away
            if (ladderHighlight != null)
            {
                ladderHighlight.enabled = false;
            }
        }
    }
}