using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Configuración de niveles")]
    public GameObject[] niveles;

    [Header("Referencia al Preview Controller")]
    public StartGameAfterTimeline previewController;

    [Header("Recompensa")]
    public GameObject rewardPrefab;

    [Header("Vidas y bola")]
    public int lives = 3;
    private int maxlives = 3;
    public Transform spawnPoint;
    public GameObject ballPrefab;

    [Header("UI")]
    public TMP_Text livesText;

    [Header("UI")]
    public TMP_Text levelText;


    int nivelActual = 0;
    int totalBloques = 0;
    int bloquesDestruidos = 0;
    bool recompensaAparecida = false;

    // Referencias a todas las palas en el nivel y sus posiciones iniciales
    private List<GameObject> paddleObjects = new List<GameObject>();
    private List<Vector3> paddleStartPositions = new List<Vector3>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        lives = maxlives;
    }

    void Start()
    {

        ActivarNivel(0);
        UpdateLivesUI();
        previewController.PlayPreview(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) CambiarNivelDirecto(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) CambiarNivelDirecto(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) CambiarNivelDirecto(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) CambiarNivelDirecto(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) CambiarNivelDirecto(4);
    }

    public void LoseLife()
    {
        lives--;
        UpdateLivesUI();
        if (lives > 0) StartCoroutine(SpawnBallAndResetPaddles()); 
        else GameOver();
    }

    
  


    void GameOver()
    {
        // Guarda el highscore si es necesario
        ScoreManager.Instance.EndGame();
        // Busca el componente GameOverMenuOnGUI en escena y muestra el menú  
        var gameOverMenu = Object.FindFirstObjectByType<GameOverMenuOnGUI>();
        if (gameOverMenu != null)
            gameOverMenu.ShowGameOver();
        else
            Debug.LogError("No se encontró GameOverMenuOnGUI en la escena");
    }

    public void BloqueDestruido(Vector3 bloquePos)
    {
        ++bloquesDestruidos;
        Debug.Log($"[LevelManager] Bloque destruido {bloquesDestruidos} / {totalBloques-1}");
        if (bloquesDestruidos == totalBloques-1)
        {
            CompletarNivel();
        }
        if (!recompensaAparecida && bloquesDestruidos >= 0.01f * (totalBloques-1))
        {
            recompensaAparecida = true;
            Vector3 spawnPos = bloquePos + Vector3.up * 3f;
            Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void CompletarNivel()
    {
        if (nivelActual >= niveles.Length - 1)
        {
            // Guarda el highscore si es necesario
            ScoreManager.Instance.EndGame();    
            SceneManager.LoadScene("CreditsScene");

        }

        niveles[nivelActual].SetActive(false);

        nivelActual++;
        niveles[nivelActual].SetActive(true);

        ActivarNivel(nivelActual, skipActivation: true);

        previewController.PlayPreview(nivelActual);

    }

    void ActivarNivel(int index, bool skipActivation = false)
    {
        foreach (var b in GameObject.FindGameObjectsWithTag("Ball"))
            Destroy(b);

        foreach (var rb in GameObject.FindGameObjectsWithTag("RewardBall"))
            Destroy(rb);

        foreach (var pu in GameObject.FindGameObjectsWithTag("PowerUp"))
            Destroy(pu);

        if (!skipActivation)
            foreach (var go in niveles)
                if (go) go.SetActive(false);

        if (index < 0 || index >= niveles.Length || niveles[index] == null)
            return;

        if (!skipActivation)
            niveles[index].SetActive(true);

        // Captura todas las palas (incluso inactivas) del nivel y guarda sus posiciones
        paddleObjects.Clear();
        paddleStartPositions.Clear();
        foreach (var t in niveles[index].GetComponentsInChildren<Transform>(true))
        {
            if (t.CompareTag("Paddle"))
            {
                paddleObjects.Add(t.gameObject);
                paddleStartPositions.Add(t.position);
            }
        }

        nivelActual = index;

        totalBloques = 0;
        foreach (var t in niveles[index].GetComponentsInChildren<Transform>(true))
            if (t.CompareTag("Block"))
                totalBloques++;

        bloquesDestruidos = 0;
        recompensaAparecida = false;
        UpdateLevelUI();
    }

    public void CambiarNivelDirecto(int index)
    {
        if (index < niveles.Length)
        {
            ActivarNivel(index);
            previewController.PlayPreview(index);
        }
    }
    public void AddLife()
    {
        lives = Mathf.Min(lives + 1, maxlives);  // no pase del máximo
        UpdateLivesUI();
        Debug.Log($"[LevelManager] Vida extra recogida  Vidas: {lives}");
    }
    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = $"LIVES: {lives}";
    }

    void UpdateLevelUI()
    {
        if (levelText != null)
            levelText.text = $"LEVEL: {nivelActual+1}";
    }


    private IEnumerator SpawnBallAndResetPaddles()
    {
        // Espera 1 segundo antes de reaparecer
        yield return new WaitForSeconds(1f);

        // Resetea la posición de todas las palas a las posiciones iniciales
        for (int i = 0; i < paddleObjects.Count; i++)
        {
            if (paddleObjects[i] != null)
                paddleObjects[i].transform.position = paddleStartPositions[i];
        }

        // Genera la bola en el punto de spawn
        Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
    }

}
