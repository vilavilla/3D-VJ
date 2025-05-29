
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuOnGUI : MonoBehaviour
{
    private bool isGameOver = false;
    private Rect windowRect;

    void Start()
    {
        // Tamaño y posición de la ventana (centrada 250×180)
        windowRect = new Rect(
            (Screen.width - 250) / 2,
            (Screen.height - 140) / 2,
            250, 140
        );
        // Ocultamos cursor hasta el Game Over
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Llama a este método desde tu LevelManager en lugar de LoadScene:
    public void ShowGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        // Mostramos cursor para poder clicar en el menú
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnGUI()
    {
        if (!isGameOver) return;
        windowRect = GUI.Window(99, windowRect, DrawWindow, "GAME OVER");
    }

    void DrawWindow(int id)
    {
        GUILayout.Space(10);

        if (GUILayout.Button("Restart Game", GUILayout.Height(30)))
        {
            RestartLevel();
        }

        if (GUILayout.Button("Main Menu", GUILayout.Height(30)))
        {
            LoadMainMenu();
        }

        if (GUILayout.Button("Quit Game", GUILayout.Height(30)))
        {
            QuitGame();
        }

        GUI.DragWindow();
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
