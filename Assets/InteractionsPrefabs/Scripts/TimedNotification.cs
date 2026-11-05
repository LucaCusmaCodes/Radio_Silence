using UnityEngine;

public class TimedNotification : MonoBehaviour
{
    [Header("Notification Settings")]
    [Tooltip("Time in seconds before the notification is destroyed.")]
    public float lifetime = 3.0f;

    void Start()
    {
        // Automatically destroys this GameObject (and the Canvas) after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }
}
