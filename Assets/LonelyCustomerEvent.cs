
using System.Collections;
using UnityEngine;
using TMPro;

public class LonelyCustomerEvent : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text promptText;
    public TMP_Text outcomeText;

    [Header("Event Settings")]
    public float timeLimit = 5f;
    public float outcomeDisplayDuration = 4f;

    [TextArea]
    public string promptMessage =
        "He seems lonely maybe you should start a conversation";

    [TextArea]
    public string replyMessage =
        "Stop chit chatting on the job. Don't you see we have buying customers.";

    [Header("Score")]
    public int ignoreScoreChange = 1;

    private bool eventActive = false;
    private bool eventFinished = false;
    private float timeRemaining;

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        if (outcomeText != null)
            outcomeText.gameObject.SetActive(false);
    }

    public void StartEvent()
    {
        if (eventActive || eventFinished)
            return;

        eventActive = true;
        timeRemaining = timeLimit;

        if (promptText != null)
        {
            promptText.text = promptMessage;
            promptText.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!eventActive)
            return;

        timeRemaining -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E))
        {
            FinishEvent(true);
            return;
        }

        if (timeRemaining <= 0f)
        {
            FinishEvent(false);
        }
    }

    private void FinishEvent(bool responded)
    {
        eventActive = false;
        eventFinished = true;

        if (promptText != null)
            promptText.gameObject.SetActive(false);

        if (responded)
        {
            ShowOutcome(replyMessage);
        }
        else
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ProcessChoice(
                    "Ignored the lonely customer",
                    ignoreScoreChange
                );
            }

            if (VignetteController.Instance != null)
            {
                VignetteController.Instance.IncreaseDarkness();
            }

            ShowOutcome("You ignored the customer.");
        }
    }

    private void ShowOutcome(string message)
    {
        if (outcomeText == null)
            return;

        outcomeText.text = message;
        outcomeText.gameObject.SetActive(true);

        StartCoroutine(HideOutcomeAfterDelay());
    }

    private IEnumerator HideOutcomeAfterDelay()
    {
        yield return new WaitForSeconds(outcomeDisplayDuration);

        if (outcomeText != null)
            outcomeText.gameObject.SetActive(false);
    }
}