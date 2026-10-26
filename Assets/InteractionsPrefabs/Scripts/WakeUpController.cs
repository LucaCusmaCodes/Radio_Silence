using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Fader))]
public class WakeUpController : MonoBehaviour
{
    private Fader screenFader;

    [Header("Waking Up Settings")]
    public float initialDelay = 0.5f;
    public float fadeDuration = 2.5f;
    public bool performBlink = true;

    private void Awake()
    {
        screenFader = GetComponent<Fader>();
    }

    private void Start()
    {
        if (screenFader != null)
        {
            screenFader.SetAlpha(1.0f);

            // Ensure mouse is free/visible while waking up in darkness
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            StartCoroutine(WakeUpSequence());
        }
    }

    private IEnumerator WakeUpSequence()
    {
        yield return new WaitForSeconds(initialDelay);

        if (performBlink)
        {
            yield return StartCoroutine(screenFader.FadeRoutine(0.5f, 0.4f));
            yield return StartCoroutine(screenFader.FadeRoutine(1.0f, 0.3f));
            yield return new WaitForSeconds(0.3f); // Fixed space here!
        }

        // Full eye opening
        yield return StartCoroutine(screenFader.FadeRoutine(0.0f, fadeDuration));

        // ---> EYES ARE OPEN: Lock mouse for normal gameplay! <---
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}