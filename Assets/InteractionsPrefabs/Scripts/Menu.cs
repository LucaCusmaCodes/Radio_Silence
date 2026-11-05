using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("UI & World Components")]
    public GameObject MenuCanvas;
    public GameObject playerCapsule;
    public GameObject PlayerFollowCamera;

    private bool MenuOpen = false;

    private void Start()
    {
        if (MenuCanvas != null)
        {
            MenuCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause();
        }
    }

    public void SetPause()
    {
        if (!MenuOpen)
        {
            // Open Pause Menu
            if (MenuCanvas != null) MenuCanvas.SetActive(true);
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playerCapsule != null) playerCapsule.SetActive(false);
            if (PlayerFollowCamera != null) PlayerFollowCamera.SetActive(false);
        }
        else
        {
            // Resume Game
            if (MenuCanvas != null) MenuCanvas.SetActive(false);
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (playerCapsule != null) playerCapsule.SetActive(true);
            if (PlayerFollowCamera != null) PlayerFollowCamera.SetActive(true);
        }

        MenuOpen = !MenuOpen;
    }

    public void Restart()
    {
        Debug.Log("Restarting Game");

        // 1. Reset time scale and cursor
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. Reset persistent score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.internalScore = 0;
        }

        // 3. Reset pitch, volume, and playback state for MusicManager without changing its script
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ResetPitch();

            if (MusicManager.Instance.musicAudioSource != null)
            {
                MusicManager.Instance.musicAudioSource.volume = 1.0f; // Restores volume if faded out

                if (!MusicManager.Instance.musicAudioSource.isPlaying)
                {
                    MusicManager.Instance.musicAudioSource.Play(); // Ensures music starts playing again if stopped
                }
            }
        }

        // 4. Load the target scene (0 for Main Menu / Restart)
        SceneManager.LoadScene(0);
    }

    public void exit()
    {
        Application.Quit();
    }
}