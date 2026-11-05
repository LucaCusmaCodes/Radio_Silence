
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class PortalSequence : MonoBehaviour
{
    [Header("First Customer")]
    public NPCWalker firstNPC;
    public Transform pointB;
    public Highlight_Human firstNPCHighlight;

    [Header("Second Customer")]
    public NPCWalker secondNPC;
    public Transform pointC;

    [Header("Exit Points")]
    public Transform exitPoint1;
    public Transform exitPoint2;

    [Header("Dialogue Settings")]
    public float firstDialogueDuration = 5f;
    public float secondDialogueDuration = 5f;
    public float responseTimeLimit = 10f;

    [Header("Countdown Circle")]
    public Image choiceTimerImage;

    private bool sequenceStarted = false;

    private void Awake()
    {
        // Disable the highlight at the beginning.
        if (firstNPCHighlight != null)
        {
            firstNPCHighlight.SetHighlightEnabled(false);
        }

        // Hide the countdown circle at the beginning.
        if (choiceTimerImage != null)
        {
            choiceTimerImage.gameObject.SetActive(false);
            choiceTimerImage.fillAmount = 1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !sequenceStarted)
        {
            sequenceStarted = true;
            StartCoroutine(PlaySequence());
        }
    }

    private IEnumerator PlaySequence()
    {
        // Customer 1 walks to Point B and speaks.
        yield return StartCoroutine(
            firstNPC.WalkAndTalk(pointB, firstDialogueDuration)
        );

        // Enable highlighting and check whether
        // the player is already inside the detection area.
        if (firstNPCHighlight != null)
        {
            firstNPCHighlight.SetHighlightEnabled(true);
            firstNPCHighlight.CheckPlayerNearby();
        }

        // Show the countdown circle.
        if (choiceTimerImage != null)
        {
            choiceTimerImage.gameObject.SetActive(true);
            choiceTimerImage.fillAmount = 1f;
        }

        // Wait for the player to press E.
        float timer = 0f;
        bool responded = false;

        while (timer < responseTimeLimit)
        {
            // Gradually empty the countdown circle.
            if (choiceTimerImage != null)
            {
                choiceTimerImage.fillAmount =
                    1f - (timer / responseTimeLimit);
            }

            // Check whether the player pressed E.
            if (Keyboard.current != null &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                responded = true;
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Hide the countdown circle.
        if (choiceTimerImage != null)
        {
            choiceTimerImage.gameObject.SetActive(false);
        }

        // Disable highlighting after the response period.
        if (firstNPCHighlight != null)
        {
            firstNPCHighlight.SetHighlightEnabled(false);
        }

        if (responded)
        {
            // Customer 2 appears and turns red.
            secondNPC.gameObject.SetActive(true);
            secondNPC.TurnRed();

            yield return StartCoroutine(
                secondNPC.WalkAndTalk(pointC, secondDialogueDuration)
            );

            // Both customers walk away.
            yield return StartCoroutine(firstNPC.WalkAway(exitPoint1));
            yield return StartCoroutine(secondNPC.WalkAway(exitPoint2));
        }
        else
        {
            // Customer 1 speaks again.
            yield return StartCoroutine(
                firstNPC.TalkAgain(firstDialogueDuration)
            );

            // Increase score.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ProcessChoice("Ignored Customer", 1);
            }

            // Change music.
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.ApplyPitchDropStep();
            }

            // Darken vignette.
            if (VignetteController.Instance != null)
            {
                VignetteController.Instance.IncreaseDarkness();
            }

            // Customer 1 walks away.
            yield return StartCoroutine(firstNPC.WalkAway(exitPoint1));
        }
    }
}