using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CreditsController : MonoBehaviour
{
    [Header("Botones")]
    public Button menuButton;
    public Button quitButton;

    void Start()
    {
        menuButton.onClick.RemoveAllListeners();
        menuButton.onClick.AddListener(BackToMenu);
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(QuitGame);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
