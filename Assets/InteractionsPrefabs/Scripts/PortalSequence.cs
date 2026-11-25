
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
    public GameObject firstCustomerCanvas;

    [Header("Second Customer")]
    public NPCWalker secondNPC;
    public Transform pointC;
    public GameObject secondCustomerCanvas;

    [Header("Exit Points")]
    public Transform exitPoint1;
    public Transform exitPoint2;

    [Header("Dialogue Settings")]
    public float firstDialogueDuration = 5f;
    public float secondDialogueDuration = 5f;
    public float responseTimeLimit = 10f;

    [Header("Countdown Circle")]
    public Image choiceTimerImage;

    [Header("Audio")]
    public AudioClip returnSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    [Header("Score Changes")]
    public int returnScoreChange = 0;
    public int ignoredScoreChange = 1;

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

        if (firstCustomerCanvas != null)
            firstCustomerCanvas.SetActive(false);

        if (secondCustomerCanvas != null)
            secondCustomerCanvas.SetActive(false);

        if (secondNPC != null)
            secondNPC.gameObject.SetActive(false);
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
        // Customer 1 walks to Point B.
        // Their Canvas appears only while they speak.
        if (firstCustomerCanvas != null)
            firstCustomerCanvas.SetActive(true);

        yield return StartCoroutine(
            firstNPC.WalkAndTalk(pointB, firstDialogueDuration)
        );

        if (firstCustomerCanvas != null)
            firstCustomerCanvas.SetActive(false);

        // Show highlighting and countdown.
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
            {
                choiceTimerImage.fillAmount =
                    1f - (timer / responseTimeLimit);
            }

            if (Keyboard.current != null &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                responded = true;
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Hide countdown and highlighting.
        if (choiceTimerImage != null)
            choiceTimerImage.gameObject.SetActive(false);

        if (firstNPCHighlight != null)
            firstNPCHighlight.SetHighlightEnabled(false);

        if (responded)
            yield return StartCoroutine(HandleResponse());
        else
            yield return StartCoroutine(HandleIgnoredChoice());
    }

    private IEnumerator HandleResponse()
    {
        // Customer 1 speaks again.
        if (firstCustomerCanvas != null)
            firstCustomerCanvas.SetActive(true);

        yield return StartCoroutine(
            firstNPC.TalkAgain(firstDialogueDuration)
        );

        if (firstCustomerCanvas != null)
            firstCustomerCanvas.SetActive(false);

        // Play return sound.
        if (returnSound != null)
        {
            AudioSource.PlayClipAtPoint(
                returnSound,
                transform.position,
                soundVolume
            );
        }

        // Returning the wallet gives 0 points.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice(
                "Returned Wallet",
                returnScoreChange
            );
        }

        // Customer 2 appears and turns red.
        if (secondNPC != null)
        {
            secondNPC.gameObject.SetActive(true);
            secondNPC.TurnRed();

            // Customer 2 speaks while walking to Point C.
            if (secondCustomerCanvas != null)
                secondCustomerCanvas.SetActive(true);

            yield return StartCoroutine(
                secondNPC.WalkAndTalk(pointC, secondDialogueDuration)
            );

            if (secondCustomerCanvas != null)
                secondCustomerCanvas.SetActive(false);

            // Both customers walk away.
            yield return StartCoroutine(firstNPC.WalkAway(exitPoint1));
            yield return StartCoroutine(secondNPC.WalkAway(exitPoint2));
        }
        else
        {
            yield return StartCoroutine(firstNPC.WalkAway(exitPoint1));
        }
    }

    private IEnumerator HandleIgnoredChoice()
    {
        // Customer 1 speaks again.
        if (firstCustomerCanvas != null)
            firstCustomerCanvas.SetActive(true);

        yield return StartCoroutine(
            firstNPC.TalkAgain(firstDialogueDuration)
        );

        if (firstCustomerCanvas != null)
            firstCustomerCanvas.SetActive(false);

        // Ignoring the customer gives 1 point.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice(
                "Ignored Customer",
                ignoredScoreChange
            );
        }

        // Apply negative consequences.
        if (MusicManager.Instance != null)
            MusicManager.Instance.ApplyPitchDropStep();

        if (VignetteController.Instance != null)
            VignetteController.Instance.IncreaseDarkness();

        yield return StartCoroutine(firstNPC.WalkAway(exitPoint1));
    }
}
