using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;
using UnityEngine.Splines; 
using Unity.Mathematics;
using Unity.Cinemachine;

public class StartGameAfterTimeline : MonoBehaviour
{
    public CinemachineSplineDolly splineDolly; 
    [Header("Ball Spawn")]
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public float previewDuration = 7f; 


    public void PlayPreview(int levelIndex) 
    {
        StopAllCoroutines(); 
        StartCoroutine(PreviewRoutine(levelIndex));

    }

    private void Evaluate(float t)
    {
        splineDolly.CameraPosition = t;
    }

    IEnumerator PreviewRoutine(int levelIndex)                         
    {
        float dur = previewDuration;
        float elapsed = 0f;
        while (elapsed < dur)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                break;
            }
            elapsed += Time.deltaTime;
            Evaluate(elapsed / dur);
            yield return null;
        }
        Evaluate(1f); 
        yield return null;

        OnPreviewComplete(levelIndex);                               
    }

    void OnPreviewComplete(int levelIndex)                             
    {
        Debug.Log($"[Preview] Nivel {levelIndex} terminado, spawneando bola");
        if (ballPrefab != null && spawnPoint != null)
            Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        else
            Debug.LogWarning("[Preview] Falta ballPrefab o spawnPoint");
    }
}
