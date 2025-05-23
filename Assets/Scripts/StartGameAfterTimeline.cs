using UnityEngine;
using UnityEngine.Playables;

public class StartGameAfterTimeline : MonoBehaviour
{
    public PlayableDirector timeline;
    public GameObject gameController;

    void Start()
    {
        timeline.stopped += OnTimelineStopped;
        gameController.SetActive(false); // Desactiva el juego al principio
    }

    void OnTimelineStopped(PlayableDirector obj)
    {
        if (gameController != null)
            gameController.SetActive(true); // Activa el juego cuando termine la animación
    }
}
