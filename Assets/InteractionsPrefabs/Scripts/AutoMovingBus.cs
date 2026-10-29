
using UnityEngine;

public class AutoMovingBus : MonoBehaviour
{
    [Header("Destination")]
    public Transform targetPosition;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    [Tooltip("Maximum distance the bus can travel from its starting position.")]
    public float maxTravelDistance = 50f;

    [Header("Audio Settings")]
    public AudioClip drivingSound;

    [Range(0f, 1f)]
    public float soundVolume = 1f;

    public float minDistance = 2f;
    public float maxDistance = 30f;

    private AudioSource audioSource;
    private bool isMoving = true;

    private Vector3 startingPosition;
    private float distanceTravelled;

    void Start()
    {
        // Remember where the bus started.
        startingPosition = transform.position;
        distanceTravelled = 0f;

        // Get or add an AudioSource on the bus.
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

            // 3D sound settings.
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;

            audioSource.Play();
        }
    }

    void Update()
    {
        if (!isMoving)
            return;

        if (targetPosition == null)
        {
            StopBus();
            return;
        }

        // Stop if the maximum travel distance has been reached.
        if (distanceTravelled >= maxTravelDistance)
        {
            StopBus();
            return;
        }

        // Calculate how far the bus is allowed to move this frame.
        float remainingDistance = maxTravelDistance - distanceTravelled;
        float step = Mathf.Min(moveSpeed * Time.deltaTime, remainingDistance);

        // Move toward the destination without exceeding the travel limit.
        Vector3 previousPosition = transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition.position,
            step
        );

        // Measure actual distance travelled.
        distanceTravelled += Vector3.Distance(
            previousPosition,
            transform.position
        );

        // Stop if the destination or travel limit is reached.
        if (Vector3.Distance(transform.position, targetPosition.position) < 0.01f ||
            distanceTravelled >= maxTravelDistance)
        {
            StopBus();
        }
    }

    private void StopBus()
    {
        if (!isMoving)
            return;

        isMoving = false;

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        Debug.Log(
            "[AutoMovingBus] Bus stopped after travelling " +
            distanceTravelled.ToString("F2") + " units."
        );
    }
}
