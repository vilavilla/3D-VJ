using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    [Header("UI (Game Scene)")]
    public Text scoreText;
    public Text highScoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScore();
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddPoints(int points)
    {
        CurrentScore += points;
        UpdateUI();
    }

    public void EndGame()
    {
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.Save();
        }
    }

    void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + CurrentScore;
        if (highScoreText != null)
            highScoreText.text = "High Score: " + HighScore;
    }
}
