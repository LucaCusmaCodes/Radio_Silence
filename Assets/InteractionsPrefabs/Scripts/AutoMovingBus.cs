using UnityEngine;

public class AutoMovingBus : MonoBehaviour
{
    [Header("Destination")]
    public Transform targetPosition; // Drag your target destination empty object here

    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    [Header("Audio Settings")]
    public AudioClip drivingSound;   // Drag your engine sound here
    [Range(0f, 1f)] public float soundVolume = 1f;
    public float minDistance = 2f;   // Distance where the sound is at full volume
    public float maxDistance = 30f;  // Distance where the sound fades out completely

    private AudioSource audioSource;
    private bool isMoving = true;

    void Start()
    {
        // Get or add the AudioSource on the bus object itself so it tracks its movement
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (drivingSound != null)
        {
            audioSource.clip = drivingSound;
            audioSource.loop = true;
            audioSource.volume = soundVolume;

            // --- 3D Spatial Settings (Makes the sound move with the bus) ---
            audioSource.spatialBlend = 1f; // 1.0 = Full 3D sound in the world
            audioSource.rolloffMode = AudioRolloffMode.Linear; // Or Logarithmic
            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;
            // -------------------------------------------------------------

            audioSource.Play();
        }
    }

    void Update()
    {
        if (!isMoving || targetPosition == null) return;

        // Move the bus smoothly toward the target position
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition.position,
            moveSpeed * Time.deltaTime
        );

        // Check if the bus has reached its final destination
        if (Vector3.Distance(transform.position, targetPosition.position) < 0.01f)
        {
            transform.position = targetPosition.position;
            isMoving = false;

            // Stop the engine sound when it arrives
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            Debug.Log("[AutoMovingBus] Bus reached destination.");
        }
    }
}