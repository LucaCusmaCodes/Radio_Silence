
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

    [Header("Manager")]
    public GameObject manager;
    public float afterEManagerDelay = 2f;

    [Header("Exit Points")]
    public Transform exitPoint1;
    public Transform exitPoint2;

    [Header("Dialogue Settings")]
    public float firstDialogueDuration = 5f;
    public float secondDialogueDuration = 5f;
    public float thirdDialogueDuration = 5f;
    public float responseTimeLimit = 10f;

    [Header("Countdown Circle")]
    public Image choiceTimerImage;

    private bool sequenceStarted = false;

    private void Awake()
    {
        if (firstNPCHighlight != null)
            firstNPCHighlight.SetHighlightEnabled(false);

        if (choiceTimerImage != null)
        {
            choiceTimerImage.gameObject.SetActive(false);
            choiceTimerImage.fillAmount = 1f;
        }

        if (manager != null)
            manager.SetActive(false);
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

        if (firstNPCHighlight != null)
        {
            firstNPCHighlight.SetHighlightEnabled(true);
            firstNPCHighlight.CheckPlayerNearby();
        }

        if (choiceTimerImage != null)
        {
            choiceTimerImage.gameObject.SetActive(true);
            choiceTimerImage.fillAmount = 1f;
        }

        float timer = 0f;
        bool responded = false;

        while (timer < responseTimeLimit)
        {
            if (choiceTimerImage != null)
                choiceTimerImage.fillAmount = 1f - (timer / responseTimeLimit);

            if (Keyboard.current != null &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                responded = true;
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (choiceTimerImage != null)
            choiceTimerImage.gameObject.SetActive(false);

        if (firstNPCHighlight != null)
            firstNPCHighlight.SetHighlightEnabled(false);

        if (responded)
        {
            // Customer 1 speaks after E.
            yield return StartCoroutine(
                firstNPC.TalkAfterE(thirdDialogueDuration)
            );

            // Customer 2 (the manager) appears and turns red.
            secondNPC.gameObject.SetActive(true);
            secondNPC.TurnRed();

            // Customer 2 walks to Point C and speaks.
            yield return StartCoroutine(
                secondNPC.WalkAndTalk(pointC, secondDialogueDuration)
            );

            // Both customers walk away.
            yield return StartCoroutine(firstNPC.WalkAway(exitPoint1));
            yield return StartCoroutine(secondNPC.WalkAway(exitPoint2));
        }
        else
        {
            // Customer 1 speaks if E was NOT pressed.
            yield return StartCoroutine(
                firstNPC.TalkAgain(secondDialogueDuration)
            );

            if (GameManager.Instance != null)
                GameManager.Instance.ProcessChoice("Ignored Customer", 1);

            if (MusicManager.Instance != null)
                MusicManager.Instance.ApplyPitchDropStep();

            if (VignetteController.Instance != null)
                VignetteController.Instance.IncreaseDarkness();

            yield return StartCoroutine(firstNPC.WalkAway(exitPoint1));
        }
    }
}