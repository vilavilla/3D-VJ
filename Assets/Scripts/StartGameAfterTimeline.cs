using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class StartGameAfterTimeline : MonoBehaviour
{
    [Header("Timeline & GameController")]
    public PlayableDirector timeline;
    public GameObject gameController;

    [Header("Ball Spawn")]
    public GameObject ballPrefab;
    public Transform spawnPoint;

    void Start()
    {
        PlayPreview();
    }

    /// <summary>
    /// Lanza el preview de 7s, permite skip con ENTER, y al acabar activa el juego + 1 bola.
    /// </summary>
    public void PlayPreview()
    {
        StopAllCoroutines();
        StartCoroutine(PreviewRoutine());
    }

    IEnumerator PreviewRoutine()
    {
        // 1) Esconde el gameplay
        gameController.SetActive(false);

        // 2) Inicia el timeline desde cero
        timeline.time = 0;
        timeline.Play();

        // 3) Espera la duración o un ENTER
        float dur = (float)timeline.duration;
        float elapsed = 0f;
        while (elapsed < dur)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                // skip
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4) Para el timeline y espera un frame
        timeline.Stop();
        yield return null;

        // 5) Activa el juego y spawnea UNA bola
        gameController.SetActive(true);
        if (ballPrefab != null && spawnPoint != null)
            Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
            //Debug.LogWarning("No saco la bola");
        else
            Debug.LogWarning("Asigna ballPrefab y spawnPoint en StartGameAfterTimeline");
    }
}
