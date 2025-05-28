using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    public Transform spawnPoint;
    public GameObject ballPrefab;

    [Header("UI")]
    public TMP_Text livesText;

    int nivelActual = 0;
    int totalBloques = 0;
    int bloquesDestruidos = 0;
    bool recompensaAparecida = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
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
        if (lives > 0) SpawnBall();
        else GameOver();
    }

    public void SpawnBall()
    {
        Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
    }

    void GameOver()
    {
        SceneManager.LoadScene("MenuScene");
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
        if (nivelActual >= niveles.Length - 1) return;

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

        if (!skipActivation)
            foreach (var go in niveles)
                if (go) go.SetActive(false);

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
    }

    public void CambiarNivelDirecto(int index)
    {
        if (index < niveles.Length)
        {
            ActivarNivel(index);
            previewController.PlayPreview(index);
        }
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = $"Vidas: {lives}";
    }
}
