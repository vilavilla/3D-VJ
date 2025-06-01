using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CreditsController : MonoBehaviour
{
    [Header("Botones")]
    public Button menuButton;
    public Button quitButton;

    void Awake()
    {
        // Si no arrastraste los botones en el Inspector, los buscamos aquí por nombre:
        if (menuButton == null)
            menuButton = GameObject.Find("MenuButton")?.GetComponent<Button>();
        if (quitButton == null)
            quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();
    }

    void OnEnable()
    {
        // 1) Forzamos que el juego NO esté en pausa
        Time.timeScale = 1f;

        // 2) Restauramos el cursor para poder clicar en UI
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Start()
    {
        // 3) Asignamos los listeners a ambos botones
        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(BackToMenu);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void BackToMenu()
    {
        // Antes de cambiar de escena, nos aseguramos de guardar highscore si procede:
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.EndGame();

        SceneManager.LoadScene("MenuScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
