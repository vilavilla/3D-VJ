using UnityEngine;
using TMPro;                 
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI (Menu Scene)")]
    public TextMeshProUGUI highScoreText;  //  TextMeshProUGUI en lugar de Text

    void Start()
    {
        int hs = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
            highScoreText.text = "High Score: " + hs;
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
