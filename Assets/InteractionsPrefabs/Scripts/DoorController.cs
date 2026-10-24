using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public enum DoorMovementType { Swing, Slide }

    [Header("Movement Type")]
    public DoorMovementType movementType = DoorMovementType.Swing;

    [Header("General Settings")]
    public float smoothSpeed = 3f;

    [Header("Swing Settings")]
    public float openAngle = 90f;

    [Header("Slide Settings")]
    public Vector3 slideOffset = new Vector3(2f, 0f, 0f);

    [Header("Audio Settings")]
    public AudioClip doorSound;
    [Range(0f, 1f)] public float soundVolume = 1f;
    private AudioSource audioSource;

    private Quaternion closedLocalRotation;
    private Quaternion openLocalRotation;
    private Vector3 closedLocalPosition;
    private Vector3 openLocalPosition;

    private bool isOpen = false;
    private bool isMoving = false; // Prevents overlapping interruption bugs
    private Coroutine currentRoutine;

    void Awake()
    {
        closedLocalRotation = transform.localRotation;
        openLocalRotation = closedLocalRotation * Quaternion.Euler(0, openAngle, 0);

        closedLocalPosition = transform.localPosition;
        openLocalPosition = closedLocalPosition + slideOffset;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && doorSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OpenDoor()
    {
        if (isOpen) return; // Already open, ignore
        isOpen = true;
        PlaySound();
        StartMovement(movementType == DoorMovementType.Swing ? (object)openLocalRotation : openLocalPosition);
    }

    public void CloseDoor()
    {
        if (!isOpen) return; // Already closed, ignore
        isOpen = false;
        PlaySound();
        StartMovement(movementType == DoorMovementType.Swing ? (object)closedLocalRotation : closedLocalPosition);
    }

    public void ToggleDoor()
    {
        if (isOpen) CloseDoor();
        else OpenDoor();
    }

    private void StartMovement(object target)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        if (movementType == DoorMovementType.Swing)
        {
            currentRoutine = StartCoroutine(SwingRoutine((Quaternion)target));
        }
        else
        {
            currentRoutine = StartCoroutine(SlideRoutine((Vector3)target));
        }
    }

    private IEnumerator SwingRoutine(Quaternion targetLocalRotation)
    {
        isMoving = true;
        float time = 0f;
        Quaternion startLocalRotation = transform.localRotation;

        while (time < 1f)
        {
            time += Time.deltaTime * smoothSpeed;
            transform.localRotation = Quaternion.Slerp(startLocalRotation, targetLocalRotation, time);
            yield return null;
        }

        transform.localRotation = targetLocalRotation;
        isMoving = false;
    }

    private IEnumerator SlideRoutine(Vector3 targetLocalPosition)
    {
        isMoving = true;
        float time = 0f;
        Vector3 startLocalPosition = transform.localPosition;

        while (time < 1f)
        {
            time += Time.deltaTime * smoothSpeed;
            transform.localPosition = Vector3.Lerp(startLocalPosition, targetLocalPosition, time);
            yield return null;
        }

        transform.localPosition = targetLocalPosition;
        isMoving = false;
    }

    private void PlaySound()
    {
        if (audioSource != null && doorSound != null)
        {
            // Prevents audio from overlapping and stuttering if triggered rapidly
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(doorSound, soundVolume);
            }
        }
    }
}