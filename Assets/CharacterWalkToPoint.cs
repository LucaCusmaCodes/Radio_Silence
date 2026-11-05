
using UnityEngine;

public class CharacterWalkToPoint : MonoBehaviour
{
    public Animator animator;
    public Transform destination;
    public float walkSpeed = 2f;

    private bool isWalking = false;

    public void StartWalking()
    {
        isWalking = true;

        if (animator != null)
        {
            animator.Play("Walk");
        }
    }

    private void Update()
    {
        if (!isWalking || destination == null)
            return;

        Vector3 direction = destination.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction.normalized * walkSpeed * Time.deltaTime;
        }
        else
        {
            isWalking = false;

            if (animator != null)
            {
                animator.Play("Idle");
            }
        }
    }
}


