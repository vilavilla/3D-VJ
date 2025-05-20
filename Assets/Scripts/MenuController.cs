using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;        //  Necesario para Button
using TMPro;                 // para TextMeshProUGUI

public class MenuController : MonoBehaviour
{
    [Header("UI (Menu Scene)")]
    public TextMeshProUGUI highScoreText;

    [Header("Botones")]
    public Button playButton;
    public Button creditsButton;
    public Button quitButton;

    void Awake()
    {
        // Asegúrate de que estos campos están asignados en el Inspector:
        //  - highScoreText  tu TMP en la escena
        //  - playButton     tu Button “Play”
        //  - creditsButton  tu Button “Credits”
        //  - quitButton     tu Button “Quit”
    }

    void Start()
    {
        // 1) Mostrar High Score
        int hs = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
            highScoreText.text = "High Score: " + hs;

        // 2) Conectar los botones a sus métodos
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(PlayGame);

        creditsButton.onClick.RemoveAllListeners();
        creditsButton.onClick.AddListener(ShowCredits);

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(QuitGame);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
