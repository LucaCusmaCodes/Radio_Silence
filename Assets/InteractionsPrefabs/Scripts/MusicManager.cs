using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioSource musicAudioSource;
    public float maxPitch = 1.0f;
    public float minPitch = 0.5f;
    public float pitchStep = 0.05f;

    private float currentPitch;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentPitch = maxPitch;
    }

    private void Start()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.pitch = currentPitch;
        }
    }

    /// <summary>
    /// Call this on immoral/apathetic choices to drop the music pitch permanently.
    /// </summary>
    public void ApplyPitchDropStep()
    {
        currentPitch = Mathf.Clamp(currentPitch - pitchStep, minPitch, maxPitch);

        if (musicAudioSource != null)
        {
            musicAudioSource.pitch = currentPitch;
        }
    }

    /// <summary>
    /// Call this on game restart/menu load to restore normal audio pitch.
    /// </summary>
    public void ResetPitch()
    {
        currentPitch = maxPitch;

        if (musicAudioSource != null)
        {
            musicAudioSource.pitch = maxPitch;
        }
    }
}