using UnityEngine;
using UnityEngine.Playables;

public class LevelManager : MonoBehaviour
{
    [Header("Configuración de niveles y transición")]
    public GameObject[] niveles;             // Todos los root de cada nivel
    public PlayableDirector transition;      // Tu Timeline de transición

    [Header("Recompensa")]
    public GameObject rewardPrefab;          // Prefab de la recompensa

    private int nivelActual = 0;
    private bool transicionando = false;

    private int totalBloques = 0;
    private int bloquesDestruidos = 0;
    private bool recompensaAparecida = false;
    private Transform paddle;

    void Start()
    {
        ActivarNivel(0);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (transition.state == PlayState.Playing)
            {
                Debug.Log("Enter detectado: saltando transición");
                //transition.stopped -= OnTransicionTerminada;
                transition.Stop();
                transicionando = false;
                Camera.main.transform.position = new Vector3(0f, 14.3f, -31.5f);
                return;
            }
        }

        // Cambio manual de nivel con 1–5
        if (Input.GetKeyDown(KeyCode.Alpha1)) CambiarNivelDirecto(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) CambiarNivelDirecto(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) CambiarNivelDirecto(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) CambiarNivelDirecto(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) CambiarNivelDirecto(4);

    }

    /// <summary>
    /// Debe llamarse desde cada bloque cuando sea destruido.
    /// </summary>
    public void BloqueDestruido()
    {
        bloquesDestruidos++;
        Debug.Log($"[Level {nivelActual + 1}] Destroyed {bloquesDestruidos} / {totalBloques}");

        // Al 95% aparece la recompensa
        if (!recompensaAparecida && bloquesDestruidos >= 0.15f * totalBloques)
        {
            recompensaAparecida = true;
            // Aparece en (0,8,0) y se dirige al paddle:
            GameObject reward = Instantiate(rewardPrefab, new Vector3(0, 8, 0), Quaternion.identity);
            reward.GetComponent<Reward>().Init(paddle.position);
        }
    }

    /// <summary>
    /// Lanza la transición de Timeline y al terminar avanza de nivel.
    /// </summary>
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
        transicionando = false;
    }

 

    /// <summary>
    /// Desactiva todos los niveles y activa el índice solicitado.
    /// Además cuenta los bloques hijos con tag "Block" y obtiene el paddle.
    /// </summary>
    void ActivarNivel(int index)
    {
        // Desactiva todos los niveles
        foreach (var go in niveles) go.SetActive(false);

        if (index < niveles.Length)
        {
            var root = niveles[index];
            root.SetActive(true);
            nivelActual = index;

            // — Cuenta total de bloques manualmente —
            totalBloques = 0;
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                if (t.CompareTag("Block"))
                    totalBloques++;

            bloquesDestruidos = 0;
            recompensaAparecida = false;

            Debug.Log($"[Level {nivelActual + 1}] Total blocks: {totalBloques}");

            paddle = GameObject.FindGameObjectWithTag("Paddle")?.transform;
        }


    }


    /// <summary>
    /// Cambio inmediato, cancelando cualquier Timeline en curso.
    /// </summary>
    void CambiarNivelDirecto(int index)
    {
        if (index < niveles.Length)
        {
            transition.Stop();
            ActivarNivel(index);
        }
    }
}
