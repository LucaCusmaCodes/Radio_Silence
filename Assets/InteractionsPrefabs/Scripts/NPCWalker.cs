
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCWalker : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    public GameObject speechBubble;
    public GameObject secondSpeechBubble;
    public GameObject thirdSpeechBubble;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (speechBubble != null)
            speechBubble.SetActive(false);

        if (secondSpeechBubble != null)
            secondSpeechBubble.SetActive(false);

        if (thirdSpeechBubble != null)
            thirdSpeechBubble.SetActive(false);
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

        agent.isStopped = false;
        agent.SetDestination(destination.position);

        while (agent.pathPending ||
               agent.remainingDistance > agent.stoppingDistance + 0.1f ||
               agent.velocity.sqrMagnitude >= 0.05f)
        {
            yield return null;
        }

        if (speechBubble != null)
            speechBubble.SetActive(true);

        Debug.Log(name + " is talking!");

        yield return new WaitForSeconds(dialogueDuration);

        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

    public IEnumerator TalkAgain(float dialogueDuration)
    {
        GameObject bubble = secondSpeechBubble;

        if (bubble == null)
        {
            Debug.LogError("Second Speech Bubble is missing on " + name);
            yield break;
        }

        bubble.SetActive(true);

        Debug.Log("Customer 1 is saying something else!");

        yield return new WaitForSeconds(dialogueDuration);

        if (bubble != null)
            bubble.SetActive(false);
    }

    public IEnumerator TalkAfterE(float dialogueDuration)
    {
        GameObject bubble = thirdSpeechBubble;

        if (bubble == null)
        {
            Debug.LogError("Third Speech Bubble is missing on " + name);
            yield break;
        }

        bubble.SetActive(true);

        Debug.Log("Customer 1 is speaking after E!");

        yield return new WaitForSeconds(dialogueDuration);

        if (bubble != null)
            bubble.SetActive(false);
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

    public void TurnRed()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            if (rend.gameObject.CompareTag("Skin"))
            {
                rend.material.color = Color.red;
            }
        }
    }
}