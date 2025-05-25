using UnityEngine;

public class Reward : MonoBehaviour
{
    private Vector3 destino;
    private float speed = 3f;
    private bool nivelCompletado = false;

    public void Init(Vector3 objetivo)
    {
        destino = objetivo;
        Debug.Log($"[Reward] Init: destino = {destino}");
    }

    void Update()
    {
        // 1) Mover recompensa
        transform.position = Vector3.MoveTowards(transform.position, destino, speed * Time.deltaTime);

        // 2) Informar posición actual y distancia
        float dist = Vector3.Distance(transform.position, destino);
        Debug.Log($"[Reward] Update: pos = {transform.position}, dist al paddle = {dist}");

        // 3) Detectar llegada por distancia
        if (!nivelCompletado && dist < 0.1f)
        {
            nivelCompletado = true;
            Debug.Log("[Reward] Alcanzo el paddle, llamando a CompletarNivel()");
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.CompletarNivel();
                Debug.Log("[Reward] LevelManager.CompletarNivel() invocado.");
            }
            else
            {
                Debug.LogWarning("[Reward] LevelManager.Instance es null!");
            }
            Destroy(gameObject);
            Debug.Log("[Reward] Self-destruct tras completar nivel.");
        }
    }
}
