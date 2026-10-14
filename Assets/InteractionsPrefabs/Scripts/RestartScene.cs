using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    // Change this to KeyCode.Escape if you want to use the Esc key
    [SerializeField] private KeyCode restartKey = KeyCode.R;

    private void Update()
    {
        if (Input.GetKeyDown(restartKey))
        {
            RestartCurrentScene();
        }
    }

    // You can also hook this public method up to a UI Button's OnClick event
    public void RestartCurrentScene()
    {
        // Resets time scale just in case the game was paused
        Time.timeScale = 1.0f;

        // Fixes cursor state if it was unlocked
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        // Reloads the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}