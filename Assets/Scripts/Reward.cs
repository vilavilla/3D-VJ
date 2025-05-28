// Reward.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Reward : MonoBehaviour
{
    [Header("Movimiento inicial")]
    [Tooltip("Velocidad inicial en unidades/segundo hacia -Z local")]
    public float speed = 3f;

    [Header("Límites")]
    [Tooltip("Altura mínima antes de destruir el reward")]
    public float destroyY = -10f;

    private Rigidbody rb;
    private Collider col;
    private bool nivelCompletado = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;

        // Asegúrate de que el collider físico no es trigger para chocar con el suelo
        col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    void Start()
    {
        // Impulso inicial hacia atrás (eje Z local negativo)
        rb.linearVelocity = -transform.forward * speed;
    }

    void Update()
    {
        // Destruir si cae demasiado
        if (transform.position.y < destroyY && !nivelCompletado)
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (nivelCompletado) return;

        if (collision.gameObject.CompareTag("Ball"))
        {
            Physics.IgnoreCollision(collision.collider, col);
            return;
        }

        // Si colisiona con el paddle, completar nivel
        if (collision.gameObject.CompareTag("Paddle"))
        {
            nivelCompletado = true;
            LevelManager.Instance?.CompletarNivel();
            Destroy(gameObject);
        }
    }
}
