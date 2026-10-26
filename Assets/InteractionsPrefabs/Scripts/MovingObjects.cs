using UnityEngine;

public class MovingObjects : MonoBehaviour
{
    [Header("References")]
    public GameObject ObjectToMove;
    public GameObject TargetPosition;

    [Header("Movement Settings")]
    public float SpeedMultiplier = 5f;

    [Header("Audio Settings")]
    public AudioClip busMovingSound;    // Drag your engine/driving sound here
    [Range(0f, 1f)] public float volume = 1f;

    private bool isMoving = false;
    private bool hasCompleted = false;
    private Transform playerTransform;
    private AudioSource audioSource;

    private void Start()
    {
        if (ObjectToMove != null)
        {
            // Automatically grab or add an AudioSource to the moving bus object
            audioSource = ObjectToMove.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = ObjectToMove.AddComponent<AudioSource>();
            }

            audioSource.clip = busMovingSound;
            audioSource.loop = true;           // Keeps the sound looping while driving
            audioSource.playOnAwake = false;   // Prevents it from playing before the ride starts
            audioSource.volume = volume;
            audioSource.spatialBlend = 1f;     // Set to 3D so it sounds positional in the world
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger if the Player walks in and it hasn't moved yet
        if (other.CompareTag("Player") && !isMoving && !hasCompleted)
        {
            isMoving = true;
            playerTransform = other.transform;

            // Start playing the moving sound loop!
            if (audioSource != null && busMovingSound != null)
            {
                audioSource.Play();
            }
        }
    }

    private void Update()
    {
        if (isMoving && ObjectToMove != null && TargetPosition != null)
        {
            // 1. Record where the bus was before moving this frame
            Vector3 previousBusPos = ObjectToMove.transform.position;

            // 2. Move the bus smoothly toward the target
            ObjectToMove.transform.position = Vector3.MoveTowards(
                ObjectToMove.transform.position,
                TargetPosition.transform.position,
                Time.deltaTime * SpeedMultiplier
            );

            // 3. Calculate exact delta movement
            Vector3 busDelta = ObjectToMove.transform.position - previousBusPos;

            // 4. Move the player with the bus
            if (playerTransform != null)
            {
                CharacterController cc = playerTransform.GetComponent<CharacterController>();
                if (cc != null)
                {
                    cc.Move(busDelta);
                }
                else
                {
                    playerTransform.position += busDelta;
                }
            }

            // 5. Check if the bus has reached its final destination
            if (Vector3.Distance(ObjectToMove.transform.position, TargetPosition.transform.position) < 0.001f)
            {
                ObjectToMove.transform.position = TargetPosition.transform.position;
                isMoving = false;
                hasCompleted = true;
                playerTransform = null;

                // Stop the moving sound when the ride ends
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
            }
        }
    }
}