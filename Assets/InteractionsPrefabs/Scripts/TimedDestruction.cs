using UnityEngine;

public class TimedDestruction : MonoBehaviour
{
    [Header("Lifetime Settings")]
    [Tooltip("Time in seconds before this object is destroyed.")]
    public float lifetimeSeconds = 60.0f; // Default set to 1 minute

    private void Start()
    {
        // Tells Unity to destroy this GameObject after lifetimeSeconds
        Destroy(gameObject, lifetimeSeconds);
    }
}