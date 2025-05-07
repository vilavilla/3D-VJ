using UnityEngine;

public class Ball : MonoBehaviour
{
    public float baseSpeed = 10f;
    public float speedIncreaseRate = 0.2f;

    private Rigidbody rb;
    private float currentSpeed;
    private float timeElapsed = 0f;

    public float minForwardComponent = 0.3f; // M�nima proporci�n de velocidad hacia adelante (Z)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = baseSpeed;

        Vector3 direction = new Vector3(Random.Range(-0.5f, 0.5f), 0f, 1f).normalized;
        rb.linearVelocity = direction * currentSpeed;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        currentSpeed = baseSpeed + (speedIncreaseRate * timeElapsed);

        Vector3 velocity = rb.linearVelocity;

        // --- LIMITAR COMPONENTE Z M�NIMA ---
        float zComponent = Mathf.Abs(velocity.normalized.z);

        // Si la velocidad hacia adelante o atr�s es muy baja, corregimos
        if (zComponent < minForwardComponent)
        {
            float signX = Mathf.Sign(velocity.x);
            float signZ = Mathf.Sign(velocity.z);

            // Forzar nueva direcci�n con suficiente Z
            Vector3 newDirection = new Vector3(0.7f * signX, 0f, 0.7f * signZ).normalized;

            velocity = newDirection * currentSpeed;
        }
        else
        {
            velocity = velocity.normalized * currentSpeed;
        }

        rb.linearVelocity = velocity;
    }
}
