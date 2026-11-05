
using UnityEngine;
using UnityEngine.AI;

public class CustomerMovement : MonoBehaviour
{
    public Transform pointB;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (agent == null)
        {
            Debug.LogError("Customer is missing a NavMeshAgent.");
            return;
        }

        if (pointB == null)
        {
            Debug.LogError("Point B has not been assigned.");
            return;
        }

        agent.SetDestination(pointB.position);
    }

    void Update()
    {
        if (agent != null && animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }
}