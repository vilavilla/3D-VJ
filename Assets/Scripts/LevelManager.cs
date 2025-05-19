/*using UnityEngine;
public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject[] niveles; // Array para meter tus niveles
    private void Start()
    {
        ActivarNivel(0); // Activa el primer nivel al inicio
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivarNivel(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivarNivel(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivarNivel(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ActivarNivel(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ActivarNivel(4);
    }

    void ActivarNivel(int index)
    {
        for (int i = 0; i < niveles.Length; i++)
        {
            niveles[i].SetActive(i == index); // Solo activa el nivel seleccionado
        }
    }
}*/
using UnityEngine;
using UnityEngine.Playables;

public class LevelManager : MonoBehaviour
{
    public GameObject[] niveles;             // Array con todos los niveles
    public PlayableDirector transition;      // Timeline con la animación
    private int nivelActual = 0;
    private bool transicionando = false;

    void Start()
    {
        ActivarNivel(0);
    }

    void Update()
    {
        // Cambiar de nivel manualmente con teclas numéricas (1–5)
        if (Input.GetKeyDown(KeyCode.Alpha1)) CambiarNivelDirecto(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) CambiarNivelDirecto(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) CambiarNivelDirecto(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) CambiarNivelDirecto(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) CambiarNivelDirecto(4);
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
        transicionando = false;
    }

    void ActivarNivel(int index)
    {
        // Desactivar todos los niveles primero
        for (int i = 0; i < niveles.Length; i++)
        {
            niveles[i].SetActive(false);
        }

        if (index < niveles.Length)
        {
            niveles[index].SetActive(true);
            nivelActual = index; // actualiza también el número actual
        }
    }

    void CambiarNivelDirecto(int index)
    {
        if (index < niveles.Length)
        {
            transition.Stop(); // parar cualquier animación en curso
            ActivarNivel(index);
        }
    }
}

