
using UnityEngine;
using UnityEngine.AI;

public class WalkToPoint : MonoBehaviour
{
    public Transform pointB;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (agent != null && pointB != null)
        {
            agent.SetDestination(pointB.position);
        }
        else
        {
            Debug.LogWarning("Missing NavMeshAgent or Point B.");
        }

        if (animator == null)
        {
            Debug.LogWarning("Animator not found on character or its children.");
        }
    }

    void Update()
    {
        if (agent != null && animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }
}