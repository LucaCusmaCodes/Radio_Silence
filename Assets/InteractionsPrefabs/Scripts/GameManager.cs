using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton Instance so other scripts can access it easily
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    public int internalScore = 0;

    private void Awake()
    {
        // Ensure there's only one GameManager in the scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keeps score across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ProcessChoice(string outcomeMessage, int scoreDelta)
    {
        internalScore += scoreDelta;
        Debug.Log($"[GameManager] Outcome: {outcomeMessage} | Score: {internalScore}");
    }
}
