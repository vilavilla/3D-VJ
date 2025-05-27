using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuOnGUI : MonoBehaviour
{
    private bool isPaused = false;
    private Rect windowRect;

    void Start()
    {
        // Centrar la ventana (200×200) en pantalla
        windowRect = new Rect(
            (Screen.width - 200) / 2,
            (Screen.height - 200) / 2,
            200, 200
        );
        // Empieza ocultando cursor y bloqueándolo
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            TogglePause();
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }

    void OnGUI()
    {
        if (!isPaused) return;
        windowRect = GUI.Window(0, windowRect, PauseWindow, "PAUSED");
    }

    void PauseWindow(int id)
    {
        GUILayout.Space(20);

        if (GUILayout.Button("Resume", GUILayout.Height(30)))
            Resume();

        if (GUILayout.Button("Restart Game", GUILayout.Height(30)))
            RestartLevel();

        if (GUILayout.Button("Main Menu", GUILayout.Height(30)))
            LoadMainMenu();

        if (GUILayout.Button("Quit", GUILayout.Height(30)))
            QuitGame();

        GUI.DragWindow();
    }

    private void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void RestartLevel()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadMainMenu()
    {
        // Guarda el highscore si es necesario
        ScoreManager.Instance.EndGame();
        // Aseguramos que el juego está "despausado"
        isPaused = false;
        Time.timeScale = 1f;

        // Restauramos cursor para poder clicar en el menú
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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
