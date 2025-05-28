using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;
using UnityEngine.Splines; // new: para usar SplineContainer
using Unity.Mathematics; // new: para usar Vector3 y otras matemáticas
using Unity.Cinemachine; // new: para usar CinemachineSplineDolly

public class StartGameAfterTimeline : MonoBehaviour
{
    public CinemachineSplineDolly splineDolly; // new: referencia al CinemachineSplineDolly
    [Header("Ball Spawn")]
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public float previewDuration = 7f; // new: duración de la 


    public void PlayPreview(int levelIndex) 
    {
        StopAllCoroutines(); // new: detiene cualquier rutina previa
        StartCoroutine(PreviewRoutine(levelIndex)); // new: inicia la rutina de preview con el índice del nivel

    }

    private void Evaluate(float t)
    {
        //splineContainer.Evaluate(t, out Vector3 position, out Vector3 tangent, out Vector3 upVector);
        splineDolly.CameraPosition = t;
    }

    IEnumerator PreviewRoutine(int levelIndex)                         // new: parámetro
    {
        

        // 2) Espera la duración o un ENTER para skip
        float dur = previewDuration;
        float elapsed = 0f;
        while (elapsed < dur)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                //timeline.Stop();
                break;
            }
            elapsed += Time.deltaTime;
            Evaluate(elapsed / dur); // new: evalúa el spline
            yield return null;
        }

        // 3) Finaliza la reproducción y espera un frame
        //timeline.Stop();
        Evaluate(1f); // new: evalúa el spline al final
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
