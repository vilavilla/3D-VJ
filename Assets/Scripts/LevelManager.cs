using UnityEngine;
using UnityEngine.Playables;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance { get; private set; }


    [Header("Configuracion de niveles y transicion")]
    public GameObject[] niveles;
    public PlayableDirector transition;

    [Header("Referencia al Preview Controller")]
    public StartGameAfterTimeline previewController;  // Asignar en el Inspector

    [Header("Recompensa")]
    public GameObject rewardPrefab;

    int nivelActual = 0;
    bool transicionando = false;

    int totalBloques = 0;
    int bloquesDestruidos = 0;
    bool recompensaAparecida = false;
    Transform paddle;

    void Start()
    {
        ActivarNivel(0);
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

    public void BloqueDestruido()
    {
        bloquesDestruidos++;
        if (!recompensaAparecida && bloquesDestruidos >= 0.05f * totalBloques)
        {
            recompensaAparecida = true;
            var reward = Instantiate(rewardPrefab, new Vector3(0, 8, 0), Quaternion.identity);
            reward.GetComponent<Reward>().Init(paddle.position);
        }
    }

    public void CompletarNivel()
    {
        if (transicionando || nivelActual >= niveles.Length - 1) return;
        transicionando = true;
        transition.stopped += OnTransicionTerminada;
        transition.Play();
    }

    void OnTransicionTerminada(PlayableDirector dir)
    {
        transition.stopped -= OnTransicionTerminada;

        // 1) Desactiva el nivel viejo (si existe)
        if (nivelActual >= 0 && nivelActual < niveles.Length && niveles[nivelActual] != null)
            niveles[nivelActual].SetActive(false);

        // 2) Avanza el índice
        nivelActual++;

        // 3) Activa el nivel nuevo (si existe)
        if (nivelActual >= 0 && nivelActual < niveles.Length && niveles[nivelActual] != null)
            niveles[nivelActual].SetActive(true);

        // 4) Refresca datos internos
        ActivarNivel(nivelActual, skipActivation: true);

        // 5) Relanza el preview
        if (previewController != null)
            previewController.PlayPreview();
        else
            Debug.LogWarning("LevelManager: previewController no asignado en el Inspector");

        transicionando = false;
    }

    // Y modificamos ActivarNivel para que no vuelva a tocar el SetActive
    // si hemos hecho ya la activación manual arriba.
    void ActivarNivel(int index, bool skipActivation = false)
    {
        // Solo resetear datos, no tocar la jerarquía si skipActivation==true
        if (!skipActivation)
            foreach (var go in niveles)
                if (go != null) go.SetActive(false);

        if (index < 0 || index >= niveles.Length || niveles[index] == null)
            return;

        if (!skipActivation)
            niveles[index].SetActive(true);

        nivelActual = index;

        // Cuenta los bloques, resetea flags, busca paddle...
        totalBloques = 0;
        foreach (var t in niveles[index].GetComponentsInChildren<Transform>(true))
            if (t.CompareTag("Block"))
                totalBloques++;

        bloquesDestruidos = 0;
        recompensaAparecida = false;

        var pad = GameObject.FindGameObjectWithTag("Paddle");
        paddle = pad ? pad.transform : null;
    }


    void ActivarNivel(int index)
    {
        foreach (var go in niveles) go.SetActive(false);

        if (index < niveles.Length)
        {
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
    }

    void CambiarNivelDirecto(int index)
    {
        if (index < niveles.Length)
        {
            transition.Stop();
            ActivarNivel(index);
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

}
