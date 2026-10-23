using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedInteraction : MonoBehaviour
{
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
        if (playerInRange && !hasSlept && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SleepAndTeleportRoutine());
        }
    }

    private IEnumerator SleepAndTeleportRoutine()
    {
        hasSlept = true;

        // 1. Get all mess objects in the scene
        CleanableMess[] foundMesses = Object.FindObjectsByType<CleanableMess>(FindObjectsSortMode.None);

        List<CleanableMess> activeMesses = new List<CleanableMess>();
        if (foundMesses != null)
        {
            foreach (CleanableMess mess in foundMesses)
            {
                if (mess != null && mess.gameObject.activeInHierarchy)
                {
                    activeMesses.Add(mess);
                }
            }
        }

        // ---> DEBUG LOG: Print out every remaining mess found! <---
        Debug.Log($"[Bed Check] Total active messes found by script: {activeMesses.Count}");
        foreach (var m in activeMesses)
        {
            Debug.Log($"-> Still active mess object name: {m.gameObject.name}");
        }

        // 2. Evaluate remaining messes
        if (activeMesses.Count > 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ProcessChoice("You ignored the mess and went straight to sleep.", selfishPointsForMess);
                Debug.Log($"[Sleep Event] Point given. Remaining messes: {activeMesses.Count}");
            }
        }
        else
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ProcessChoice("You cleaned up the entire house before sleeping.", 0);
                Debug.Log("[Sleep Event] House fully cleaned. 0 points.");
            }
        }

        // 3. Fade screen to black
        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.FadeRoutine(1f, fadeDuration));
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        // 4. Teleport Player safely
        if (playerRef != null && teleportTarget != null)
        {
            CharacterController cc = playerRef.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            playerRef.transform.position = teleportTarget.position;
            playerRef.transform.rotation = teleportTarget.rotation;

            if (cc != null) cc.enabled = true;
        }

        // 5. Trigger the WakeUpController sequence smoothly
        WakeUpController wakeUp = screenFader != null ? screenFader.GetComponent<WakeUpController>() : null;
        if (wakeUp != null)
        {
            wakeUp.StopAllCoroutines();
            wakeUp.StartCoroutine("WakeUpSequence");
        }
        else if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.FadeRoutine(0f, fadeDuration));
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