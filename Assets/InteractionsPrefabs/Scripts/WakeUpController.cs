using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Fader))]
public class WakeUpController : MonoBehaviour
{
    private Fader screenFader;

    [Header("Waking Up Settings")]
    public float initialDelay = 0.5f;   // Brief pause before opening eyes
    public float fadeDuration = 2.5f;   // Time to fully open eyes
    public bool performBlink = true;    // Optional groggy double-blink

    private void Awake()
    {
        // Automatically grabs the Fader script attached to this exact same GameObject
        screenFader = GetComponent<Fader>();
    }

    private void Start()
    {
        if (screenFader != null)
        {
            // Start completely black every time this scene loads fresh
            screenFader.SetAlpha(1.0f);
            StartCoroutine(WakeUpSequence());
        }
    }

    private IEnumerator WakeUpSequence()
    {
        yield return new WaitForSeconds(initialDelay);

        if (performBlink)
        {
            // First quick, groggy half-open blink
            yield return StartCoroutine(screenFader.FadeRoutine(0.5f, 0.4f));
            yield return StartCoroutine(screenFader.FadeRoutine(1.0f, 0.3f));
            yield return new WaitForSeconds(0.3f);
        }

        // Full eye opening
        yield return StartCoroutine(screenFader.FadeRoutine(0.0f, fadeDuration));
    }
}