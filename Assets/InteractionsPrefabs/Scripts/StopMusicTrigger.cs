using System.Collections;
using UnityEngine;

public class StopMusicTrigger : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("If true, music smoothly fades out over time. If false, it cuts off instantly.")]
    public bool fadeOut = true;
    public float fadeDuration = 1.5f;

    [Header("Trigger Settings")]
    public bool triggerOnceOnly = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (triggerOnceOnly)
            {
                hasTriggered = true;
            }

            if (MusicManager.Instance != null && MusicManager.Instance.musicAudioSource != null)
            {
                if (fadeOut)
                {
                    StartCoroutine(FadeOutRoutine(MusicManager.Instance.musicAudioSource, fadeDuration));
                }
                else
                {
                    MusicManager.Instance.musicAudioSource.Stop();
                    Debug.Log("[StopMusicTrigger] Instantly stopped background music.");
                }
            }
            else
            {
                Debug.LogWarning("[StopMusicTrigger] MusicManager or its AudioSource is missing!");
            }

            // Disable this trigger collider so it only runs once
            if (triggerOnceOnly)
            {
                Collider col = GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = false;
                }
            }
        }
    }

    private IEnumerator FadeOutRoutine(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * (Time.deltaTime / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Restore original volume level for future playback
        Debug.Log("[StopMusicTrigger] Music faded out successfully.");
    }
}