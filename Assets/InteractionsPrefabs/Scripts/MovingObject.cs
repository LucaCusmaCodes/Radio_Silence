
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
        // Get an AudioSource if one is not assigned.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Assign the movement sound.
        if (audioSource != null && movementSound != null)
        {
            audioSource.clip = movementSound;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isMoving = true;

        // Start playing the sound while moving.
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

            // Check whether the target has been reached.
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
