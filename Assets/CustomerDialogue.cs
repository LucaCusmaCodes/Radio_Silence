
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CustomerDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public float displayDuration = 10f;

    private NavMeshAgent agent;
    private bool dialogueStarted = false;

    public void Setup(
        GameObject panel,
        TMP_Text text,
        Button button)
    {
        dialoguePanel = panel;
        dialogueText = text;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Floating dialogue Canvas not found.");
        }
    }

    void Update()
    {
        if (dialogueStarted || agent == null ||
            dialoguePanel == null || dialogueText == null)
        {
            return;
        }

        if (!agent.pathPending &&
            agent.hasPath &&
            agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        dialogueStarted = true;
        agent.isStopped = true;

        dialogueText.text = "Hi! Do you have any glasses here? Speaking of glasses my granddaughter just bought a pair and she looks very mature now. Oh they grow up so fast, I miss when I would read stories to them before bed...";
        dialoguePanel.SetActive(true);

        StartCoroutine(HideDialogueAfterDelay());
    }

    IEnumerator HideDialogueAfterDelay()
    {
        Debug.Log("HideDialogueAfterDelay started!");
            // Wait for the customer's dialogue to finish
        yield return new WaitForSeconds(displayDuration);

        // Hide the customer's dialogue
        dialoguePanel.SetActive(false);

        // Find the response event in the scene
        LonelyCustomerEvent choiceEvent =
            FindFirstObjectByType<LonelyCustomerEvent>();

        // Start the response event
        if (choiceEvent != null)
        {
            Debug.Log("Found LonelyCustomerEvent. Starting it now!");
            choiceEvent.StartEvent();
        }
        else
        {
            Debug.LogError("Could not find LonelyCustomerEvent!");
        }
    }
}



