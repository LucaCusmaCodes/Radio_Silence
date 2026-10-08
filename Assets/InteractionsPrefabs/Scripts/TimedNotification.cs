using UnityEngine;

public class TimedNotification : MonoBehaviour
{
    [Header("Settings")]
    public float lifetimeSeconds = 5f; // How long the text stays on screen

    void Start()
    {
        // Automatically destroys this text canvas after 3 seconds
        Destroy(gameObject, lifetimeSeconds);
    }
}
