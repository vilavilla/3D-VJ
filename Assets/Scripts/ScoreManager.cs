using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;  // <- Importante

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    [Header("UI (TextMeshPro)")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScore();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // En caso de que la SampleScene ya esté activa al iniciar:
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
     
        // Escena de juego (“SampleScene”)
         if (scene.name == "SampleScene")
        {
            CurrentScore = 0;
            scoreText = GameObject.Find("ScoreText")
                        .GetComponent<TMP_Text>();
            // Si también quieres mostrar el HighScore en juego:
            var hsGO = GameObject.Find("HighScoreText");
            highScoreText = hsGO != null
                ? hsGO.GetComponent<TMP_Text>()
                : null;
            UpdateUI();
        }
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
        UpdateUI();
    }

    void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"SCORE: {CurrentScore}";
        if (highScoreText != null)
            highScoreText.text = $"High Score: {HighScore}";
    }
}
