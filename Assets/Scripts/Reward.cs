using UnityEngine;

public class Reward : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad en unidades/segundo hacia -Z")]
    public float speed = 3f;

    private bool nivelCompletado = false;

    void Update()
    {
        // Mover siempre a lo largo del eje Z negativo en espacio mundial
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (nivelCompletado) return;

        // Si el paddle choca con el reward, completamos nivel
        if (other.CompareTag("Paddle"))
        {
            nivelCompletado = true;
            LevelManager.Instance?.CompletarNivel();
            Destroy(gameObject);
        }
    }
}
