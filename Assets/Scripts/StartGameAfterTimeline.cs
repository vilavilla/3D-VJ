using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;

public class StartGameAfterTimeline : MonoBehaviour
{
    [Header("Timeline & GameController")]
    public PlayableDirector timeline;
    public TimelineAsset[] previewTimelines;   // new: array de TimelineAssets por nivel

    [Header("Ball Spawn")]
    public GameObject ballPrefab;
    public Transform spawnPoint;

    void Start()
    {
        // Arranca la intro del nivel 0
        PlayPreview(0);
    }

    /// <summary>
    /// Reproduce el preview correspondiente al nivel indicado.
    /// </summary>
    public void PlayPreview(int levelIndex)
    {
        Debug.Log($"[DEBUG] Solicitado Preview para nivel {levelIndex}");             // 1
        if (levelIndex < 0 || levelIndex >= previewTimelines.Length)
        {
            Debug.LogError($"[Preview] Nivel inválido {levelIndex}");
            return;
        }

        timeline.Stop();
        StopAllCoroutines();

        // Antes de asignar, imprime qué asset había…
        Debug.Log($"[DEBUG] Asset antiguo: {timeline.playableAsset?.name}");          // 2

        timeline.playableAsset = previewTimelines[levelIndex];
        timeline.time = 0;
        timeline.Evaluate();

        // Y ahora imprime el nuevo
        Debug.Log($"[DEBUG] Asset asignado: {timeline.playableAsset?.name}");         // 3

        StartCoroutine(PreviewRoutine(levelIndex));
    }


    IEnumerator PreviewRoutine(int levelIndex)                         // new: parámetro
    {
        // 1) Inicia la reproducción desde cero
        timeline.time = 0;
        timeline.Evaluate();
        timeline.Play();

        // 2) Espera la duración o un ENTER para skip
        float dur = (float)timeline.duration;
        float elapsed = 0f;
        while (elapsed < dur)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                timeline.Stop();
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3) Finaliza la reproducción y espera un frame
        timeline.Stop();
        yield return null;

        // 4) Al acabar, spawnea la bola para ese nivel
        OnPreviewComplete(levelIndex);                               // new
    }

    void OnPreviewComplete(int levelIndex)                             // new
    {
        Debug.Log($"[Preview] Nivel {levelIndex} terminado, spawneando bola");
        if (ballPrefab != null && spawnPoint != null)
            Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        else
            Debug.LogWarning("[Preview] Falta ballPrefab o spawnPoint");
    }
}
