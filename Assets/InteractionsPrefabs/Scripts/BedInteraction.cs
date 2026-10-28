
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip interactionSound;
    [Range(0f, 1f)]
    public float soundVolume = 1.0f;

    [Header("Fader Reference")]
    public Fader screenFader;

    [Header("Teleportation")]
    public Transform teleportTarget;

    private GameObject playerRef;

    [Header("Settings")]
    public float fadeDuration = 1.0f;
    public int selfishPointsForMess = 1;

    private bool playerInRange = false;
    private bool hasSlept = false;

    private void Update()
    {
        if (playerInRange &&
            !hasSlept &&
            Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SleepAndTeleportRoutine());
        }
    }

    private IEnumerator SleepAndTeleportRoutine()
    {
        hasSlept = true;

        // Play the bed interaction sound independently
        // so it can continue during the fade and teleport.
        if (interactionSound != null)
        {
            GameObject soundObject =
                new GameObject("BedInteractionSound");

            soundObject.transform.position = transform.position;

            AudioSource source =
                soundObject.AddComponent<AudioSource>();

            source.clip = interactionSound;
            source.volume = soundVolume;
            source.spatialBlend = 0f;
            source.playOnAwake = false;
            source.loop = false;

            source.Play();

            // Destroy the temporary object after playback finishes.
            Destroy(soundObject, interactionSound.length + 0.5f);
        }
        else
        {
            Debug.LogWarning(
                "[BedInteraction] No interaction sound assigned!"
            );
        }

        // Find all remaining mess objects in the scene.
        CleanableMess[] foundMesses =
            Object.FindObjectsByType<CleanableMess>(
                FindObjectsSortMode.None
            );

        List<CleanableMess> activeMesses =
            new List<CleanableMess>();

        if (foundMesses != null)
        {
            foreach (CleanableMess mess in foundMesses)
            {
                if (mess != null &&
                    mess.gameObject.activeInHierarchy)
                {
                    activeMesses.Add(mess);
                }
            }
        }

        Debug.Log(
            $"[Bed Check] Active messes found: {activeMesses.Count}"
        );

        foreach (CleanableMess mess in activeMesses)
        {
            Debug.Log(
                "Still active mess: " + mess.gameObject.name
            );
        }

        // Evaluate remaining messes.
        if (activeMesses.Count > 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ProcessChoice(
                    "You ignored the mess and went straight to sleep.",
                    selfishPointsForMess
                );
            }

            Debug.Log(
                $"[Sleep Event] Awarded {selfishPointsForMess} point(s)."
            );

            // Apply atmosphere penalties when points are awarded.
            if (selfishPointsForMess > 0)
            {
                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.ApplyPitchDropStep();
                }

                if (VignetteController.Instance != null)
                {
                    VignetteController.Instance.IncreaseDarkness();
                }

                Debug.Log(
                    "[Sleep Event] Music pitch dropped and vignette darkness increased."
                );
            }
        }
        else
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ProcessChoice(
                    "You cleaned up the entire house before sleeping.",
                    0
                );
            }

            Debug.Log(
                "[Sleep Event] House fully cleaned. No points awarded."
            );
        }

        // Fade the screen to black.
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

        // Teleport the player safely.
        if (playerRef != null && teleportTarget != null)
        {
            CharacterController cc =
                playerRef.GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.enabled = false;
            }

            playerRef.transform.position =
                teleportTarget.position;

            playerRef.transform.rotation =
                teleportTarget.rotation;

            if (cc != null)
            {
                cc.enabled = true;
            }
        }

        // Start the wake-up sequence.
        WakeUpController wakeUp = screenFader != null
            ? screenFader.GetComponent<WakeUpController>()
            : null;

        if (wakeUp != null)
        {
            wakeUp.StopAllCoroutines();
            wakeUp.StartCoroutine("WakeUpSequence");
        }
        else if (screenFader != null)
        {
            yield return StartCoroutine(
                screenFader.FadeRoutine(0f, fadeDuration)
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerRef = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
