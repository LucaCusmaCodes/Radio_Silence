
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [Header("Movement Settings")]
    public GameObject ObjectToMove;
    public GameObject TargetPosition;
    public float SpeedMultiplier = 1f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip movementSound;

    private bool isMoving = false;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null && movementSound != null)
        {
            audioSource.clip = movementSound;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    // Called by StoreEvent2 when the player presses E.
    public void StartMovement()
    {
        if (ObjectToMove == null || TargetPosition == null)
            return;

        isMoving = true;

        if (audioSource != null &&
            movementSound != null &&
            !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (ObjectToMove == null || TargetPosition == null)
        {
            StopMovementSound();
            return;
        }

        if (isMoving)
        {
            ObjectToMove.transform.position = Vector3.MoveTowards(
                ObjectToMove.transform.position,
                TargetPosition.transform.position,
                Time.deltaTime * SpeedMultiplier
            );

            if (Vector3.Distance(
                ObjectToMove.transform.position,
                TargetPosition.transform.position
            ) < 0.001f)
            {
                ObjectToMove.transform.position =
                    TargetPosition.transform.position;

                isMoving = false;
                StopMovementSound();
            }
        }
    }

    private void StopMovementSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}