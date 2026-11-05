
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCWalker : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    public GameObject speechBubble;
    public GameObject secondSpeechBubble;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (speechBubble != null)
        {
            speechBubble.SetActive(false);
        }
        if (secondSpeechBubble != null)
        {
            secondSpeechBubble.SetActive(false);
        }
    }

    void Update()
    {
        if (animator != null && agent != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    public IEnumerator WalkAndTalk(Transform destination, float dialogueDuration)
    {
        if (agent == null || destination == null)
        {
            Debug.LogError("Missing NavMeshAgent or destination on " + name);
            yield break;
        }

        // Walk to the destination
        agent.isStopped = false;
        agent.SetDestination(destination.position);

        // Wait until the customer arrives
        while (agent.pathPending ||
               agent.remainingDistance > agent.stoppingDistance + 0.1f ||
               agent.velocity.sqrMagnitude >= 0.05f)
        {
            yield return null;
        }

        // Display dialogue
        if (speechBubble != null)
        {
            speechBubble.SetActive(true);
        }

        Debug.Log(name + " is talking!");

        // Keep dialogue visible for the specified duration
        yield return new WaitForSeconds(dialogueDuration);

        // Hide dialogue
        if (speechBubble != null)
        {
            speechBubble.SetActive(false);
        }
    }
    public IEnumerator TalkAgain(float dialogueDuration)
    {
        if (secondSpeechBubble != null)
        {
            secondSpeechBubble.SetActive(true);

            Debug.Log("Customer 1 is saying something else!");

            yield return new WaitForSeconds(dialogueDuration);

            secondSpeechBubble.SetActive(false);
        }
    }
 
    public IEnumerator WalkAway(Transform exitDestination)
    {
        if (agent == null || exitDestination == null)
        {
            Debug.LogError("Missing agent or exit destination on " + name);
            yield break;
        }

        agent.isStopped = false;
        agent.SetDestination(exitDestination.position);

        while (agent.pathPending ||
               agent.remainingDistance > agent.stoppingDistance + 0.1f ||
               agent.velocity.sqrMagnitude >= 0.05f)
        {
            yield return null;
        }

        gameObject.SetActive(false);
    }

    // Your existing TurnRed method
    
    public void TurnRed()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            rend.material.color = Color.red;
        }
    }

}