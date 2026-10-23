using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // 1. Reset persistent game score for a new run
        if (GameManager.Instance != null)
        {
            GameManager.Instance.internalScore = 0;
        }

        // 2. Reset music pitch back to normal
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ResetPitch();
        }

        // 3. Load the gameplay scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}