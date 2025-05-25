using UnityEngine;

public class Reward : MonoBehaviour
{
    private Vector3 destino;
    private float speed = 3f;
    private bool nivelCompletado = false;

    public void Init(Vector3 objetivo)
    {
        destino = objetivo;
    }

    void Update()
    {
        // Mueve la recompensa hacia el paddle
        transform.position = Vector3.MoveTowards(transform.position, destino, speed * Time.deltaTime);

        // Cuando casi alcanza el destino, completa nivel sólo una vez
        if (!nivelCompletado && Vector3.Distance(transform.position, destino) < 0.1f)
        {
            nivelCompletado = true;
            if (LevelManager.Instance != null)
                LevelManager.Instance.CompletarNivel();
            Destroy(gameObject);
        }
    }
}
