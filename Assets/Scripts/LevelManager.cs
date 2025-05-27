// LevelManager.cs
using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Configuración de niveles y transición")]
    public GameObject[] niveles;
    public PlayableDirector transition;

    [Header("Referencia al Preview Controller")]
    public StartGameAfterTimeline previewController;

    [Header("Recompensa")]
    public GameObject rewardPrefab;

    [Header("Vidas y bola")]
    public int lives = 3;                  // Vidas iniciales
    public Transform spawnPoint;           // Asigna en Inspector
    public GameObject ballPrefab;          // Prefab de la bola

    [Header("UI")]
    public TMP_Text livesText;     // Arrastra aquí tu TMP Text del Canvas


    int nivelActual = 0;
    bool transicionando = false;

    int totalBloques = 0;
    int bloquesDestruidos = 0;
    bool recompensaAparecida = false;
    Transform paddle;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        ActivarNivel(0);
        SpawnBall();                        // Spawnea la primera bola
        UpdateLivesUI();
    }

    void Update()
    {
        // Cambio manual de nivel con 1–5
        if (Input.GetKeyDown(KeyCode.Alpha1)) CambiarNivelDirecto(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) CambiarNivelDirecto(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) CambiarNivelDirecto(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) CambiarNivelDirecto(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) CambiarNivelDirecto(4);
    }

    /// <summary>
    /// Llamar desde Ball cuando z < -18
    /// </summary>
    public void LoseLife()
    {
        lives--;
        UpdateLivesUI();    
        Debug.Log($"[LevelManager] Vida perdida. Vidas restantes: {lives}");
        if (lives > 0)
            SpawnBall();
        else
            GameOver();
    }

    void SpawnBall()
    {
        Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
    }

    void GameOver()
    {
        Debug.Log("[LevelManager] GAME OVER");
        SceneManager.LoadScene("MenuScene");
    }

    /// <summary>
    /// Debes llamar a este método desde tu script de bloque,
    /// pasándole la posición del bloque destruido.
    /// </summary>
    public void BloqueDestruido(Vector3 bloquePos)
    {
        bloquesDestruidos++;
        // Ajusta el porcentaje según lo necesites (0.01f = 1%)
        if (!recompensaAparecida && bloquesDestruidos >= 0.01f * totalBloques)
        {
            recompensaAparecida = true;
            Vector3 spawnPos = new Vector3(bloquePos.x, bloquePos.y + 3f, bloquePos.z);
            Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void CompletarNivel()
    {
        Debug.Log($"[LevelManager] CompletarNivel() llamado (nivelActual={nivelActual})");
        if (transicionando || nivelActual >= niveles.Length - 1) return;

        transicionando = true;
        transition.stopped += OnTransicionTerminada;
        transition.Play();
    }

    void OnTransicionTerminada(PlayableDirector dir)
    {
        transition.stopped -= OnTransicionTerminada;

        if (nivelActual >= 0 && nivelActual < niveles.Length)
            niveles[nivelActual].SetActive(false);

        nivelActual++;

        if (nivelActual >= 0 && nivelActual < niveles.Length)
            niveles[nivelActual].SetActive(true);

        ActivarNivel(nivelActual, skipActivation: true);

        previewController?.PlayPreview();
        transicionando = false;
    }

    void ActivarNivel(int index, bool skipActivation = false)
    {
        if (!skipActivation)
            foreach (var go in niveles)
                if (go != null) go.SetActive(false);

        if (index < 0 || index >= niveles.Length || niveles[index] == null)
            return;

        if (!skipActivation)
            niveles[index].SetActive(true);

        nivelActual = index;

        totalBloques = 0;
        foreach (var t in niveles[index].GetComponentsInChildren<Transform>(true))
            if (t.CompareTag("Block"))
                totalBloques++;

        bloquesDestruidos = 0;
        recompensaAparecida = false;

        var pad = GameObject.FindGameObjectWithTag("Paddle");
        paddle = pad ? pad.transform : null;
    }

    public void CambiarNivelDirecto(int index)
    {
        if (index < niveles.Length)
        {
            transition.Stop();
            ActivarNivel(index);
        }
    }
    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = $"Vidas: {lives}";
    }
}
