// LevelManager.cs
using UnityEngine;
using UnityEngine.Playables;

public class LevelManager : MonoBehaviour
{
    [Header("Configuracion de niveles y transicion")]
    public GameObject[] niveles;
    public PlayableDirector transition;

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

        niveles[nivelActual].SetActive(false);
        nivelActual++;
        ActivarNivel(nivelActual);

        // relanza preview correctamente
        var preview = FindObjectOfType<StartGameAfterTimeline>();
        if (preview != null)
            preview.PlayPreview();
        else
            Debug.LogWarning("No se encontro StartGameAfterTimeline en escena.");

        transicionando = false;
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
}
