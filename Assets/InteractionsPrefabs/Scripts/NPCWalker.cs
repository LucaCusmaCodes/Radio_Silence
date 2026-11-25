using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCWalker : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Dialogue")]
    public GameObject speechBubble;
    public GameObject secondSpeechBubble;
    public GameObject ignoredSpeechBubble;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (speechBubble != null)
            speechBubble.SetActive(false);

        if (secondSpeechBubble != null)
            secondSpeechBubble.SetActive(false);

        if (ignoredSpeechBubble != null)
            ignoredSpeechBubble.SetActive(false);
    }

    private void Update()
    {
        if (animator != null && agent != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    public IEnumerator WalkAndTalk(
        Transform destination,
        float dialogueDuration)
    {
        if (agent == null || destination == null)
        {
            Debug.LogError("Missing NavMeshAgent or destination on " + name);
            yield break;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(name + " is not on a NavMesh!");
            yield break;
        }

        agent.isStopped = false;
        agent.SetDestination(destination.position);

        while (agent.pathPending)
            yield return null;

        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogError(name + " has an invalid path!");
            yield break;
        }

        while (agent.remainingDistance > agent.stoppingDistance + 0.1f ||
               agent.velocity.sqrMagnitude >= 0.05f)
        {
            if (!agent.pathPending &&
                agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                Debug.LogError(name + " cannot reach destination!");
                yield break;
            }

            yield return null;
        }

        // Show initial dialogue.
        if (speechBubble != null)
            speechBubble.SetActive(true);

        Debug.Log(name + " is talking!");

        yield return new WaitForSeconds(dialogueDuration);

        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

    public IEnumerator TalkAgain(
        bool returnedWallet,
        float dialogueDuration)
    {
        GameObject dialogueToShow = returnedWallet
            ? secondSpeechBubble
            : ignoredSpeechBubble;

        if (dialogueToShow != null)
        {
            dialogueToShow.SetActive(true);

            Debug.Log(name + " is speaking again!");

            yield return new WaitForSeconds(dialogueDuration);

            dialogueToShow.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "The required second dialogue Canvas is missing on " + name
            );
        }
    }

    public IEnumerator WalkAway(Transform exitDestination)
    {
        if (agent == null || exitDestination == null)
        {
            Debug.LogError("Missing agent or exit destination on " + name);
            yield break;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(name + " is not on a NavMesh!");
            yield break;
        }

        agent.isStopped = false;
        agent.SetDestination(exitDestination.position);

        while (agent.pathPending)
            yield return null;

        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogError(name + " has an invalid exit path!");
            yield break;
        }

        while (agent.remainingDistance > agent.stoppingDistance + 0.1f ||
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
            rend.material.color = Color.red;
        }
    }
}