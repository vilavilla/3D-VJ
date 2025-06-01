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
        // Si no has arrastrado desde el Inspector, los buscamos por nombre:
        if (highScoreText == null)
            highScoreText = GameObject.Find("HighScoreText")?.GetComponent<TextMeshProUGUI>();

        if (playButton == null)
            playButton = GameObject.Find("PlayButton")?.GetComponent<Button>();

        if (creditsButton == null)
            creditsButton = GameObject.Find("CreditsButton")?.GetComponent<Button>();

        if (quitButton == null)
            quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();
    }

    void Start()
    {

        // Asignamos los listeners de los botones
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(PlayGame);
        }

        if (creditsButton != null)
        {
            creditsButton.onClick.RemoveAllListeners();
            creditsButton.onClick.AddListener(ShowCredits);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }

        // Hacemos una primera puesta a punto inmediata (por si queremos mostrar algo nada más abrir el menú)
        RefreshHighScoreText();
    }
    void Update()
    {
        
            RefreshHighScoreText();
       
    }
    void RefreshHighScoreText()
    {
        int hs = 0;

        // Si ScoreManager ya existe, uso ese valor actualizado;
        // en caso contrario, leo directamente de PlayerPrefs:
        if (ScoreManager.Instance != null)
        {
            hs = ScoreManager.Instance.HighScore;
        }
        else
        {
            hs = PlayerPrefs.GetInt("HighScore", 0);
        }

        if (highScoreText != null)
            highScoreText.text = $"High Score: {hs}";
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
