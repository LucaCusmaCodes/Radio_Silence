using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhoneTimerUI : MonoBehaviour
{
    [Header("Data")]
    public ChoiceEvent currentEvent;

    [Header("UI Components")]
    public GameObject phoneContainer;      // Drag your main Phone Panel / Image container here!
    public TextMeshProUGUI messageDisplay;
    public Button replyButton;
    public TextMeshProUGUI replyButtonText;
    public Image timerFillBar;

    [Header("Settings")]
    public float friendReplyDelay = 1.5f;   // Seconds before friend sends annoyed response
    public float dismissDelay = 2.5f;       // Seconds to read final outcome before phone closes

    private float timer;
    private bool isEventActive = false;

    private void Start()
    {
        // Hide phone at start of game until event triggers
        if (phoneContainer != null) phoneContainer.SetActive(false);

        
    }

    public void StartEvent(ChoiceEvent evt)
    {
        currentEvent = evt;
        timer = evt.timeLimitSeconds;
        isEventActive = true;

        // 1. Show the phone container
        if (phoneContainer != null) phoneContainer.SetActive(true);

        // 2. FREE THE MOUSE CURSOR so it can click the phone UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ensure button is visible and active
        if (replyButton != null)
        {
            replyButton.gameObject.SetActive(true);
            replyButton.interactable = true;
        }

        // Set initial texts
        if (messageDisplay != null) messageDisplay.text = evt.eventPrompt;
        if (replyButtonText != null) replyButtonText.text = evt.replyButtonText;

        // Bind button click programmatically
        replyButton.onClick.RemoveAllListeners();
        replyButton.onClick.AddListener(OnReplyClicked);
    }

    private void Update()
    {
        if (!isEventActive) return;

        // Count down the timer
        timer -= Time.deltaTime;

        // Update timer UI fill bar
        if (timerFillBar != null && currentEvent.timeLimitSeconds > 0)
        {
            timerFillBar.fillAmount = Mathf.Clamp01(timer / currentEvent.timeLimitSeconds);
        }

        // Timer expired (Player outlasted the prompt / chose indifference)
        if (timer <= 0f)
        {
            OnTimerExpired();
        }
    }

    public void OnReplyClicked()
    {
        if (!isEventActive) return;
        isEventActive = false;

        EndEventUI();

        // Start delayed friend reply conversation sequence
        StartCoroutine(ReplySequence());
    }

    private IEnumerator ReplySequence()
    {
        // 1. Display player's sent text first
        if (messageDisplay != null)
        {
            messageDisplay.text = "You'll get better over time, there are other fish in the sea";
        }

        // 2. Wait for friend to type back...
        yield return new WaitForSeconds(friendReplyDelay);

        // 3. Display friend's harsh, punishing response
        string outcomeText = currentEvent.replyOutcomeText;
        if (messageDisplay != null)
        {
            messageDisplay.text = outcomeText;
        }

        // Process score in GameManager (0 points for kindness)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice(outcomeText, currentEvent.replyScoreChange);
        }

        // 4. Hold text visible during the reading delay, then close phone
        float elapsed = 0f;
        while (elapsed < dismissDelay)
        {
            if (messageDisplay != null) messageDisplay.text = outcomeText;
            elapsed += Time.deltaTime;
            yield return null;
        }

        ClosePhone();
    }

    private void OnTimerExpired()
    {
        if (!isEventActive) return;
        isEventActive = false;

        EndEventUI();

        string outcomeText = currentEvent.ignoreOutcomeText;

        // Display outcome of ignoring the notification
        if (messageDisplay != null)
        {
            messageDisplay.text = outcomeText;
        }

        // Process score in GameManager (+1 point for apathetic inaction)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessChoice(outcomeText, currentEvent.ignoreScoreChange);
        }

        // Keep text visible during reading delay before closing phone
        StartCoroutine(ClosePhoneDelayed(outcomeText));
    }

    private IEnumerator ClosePhoneDelayed(string textToHold)
    {
        float elapsed = 0f;
        while (elapsed < dismissDelay)
        {
            if (messageDisplay != null) messageDisplay.text = textToHold;
            elapsed += Time.deltaTime;
            yield return null;
        }

        ClosePhone();
    }

    private void ClosePhone()
    {
        // Hide the phone container
        if (phoneContainer != null) phoneContainer.SetActive(false);

        // Re-lock and hide mouse cursor to return to normal gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void EndEventUI()
    {
        if (replyButton != null) replyButton.gameObject.SetActive(false);
        if (timerFillBar != null) timerFillBar.fillAmount = 0f;
    }
}