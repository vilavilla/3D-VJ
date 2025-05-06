using UnityEngine;

public class Ball : MonoBehaviour
{
    public float baseSpeed = 10f;         // Velocidad inicial
    public float speedIncreaseRate = 0.2f; // Cuánto aumenta por segundo

    private Rigidbody rb;
    private float currentSpeed;
    private float timeElapsed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = baseSpeed;

        // Lanza la bola hacia adelante con un pequeño ángulo aleatorio
        Vector3 direction = new Vector3(Random.Range(-0.5f, 0.5f), 0f, 1f).normalized;
        rb.linearVelocity = direction * currentSpeed; // Usar linearVelocity en lugar de velocity
    }

    // Update is called once per frame
    void Update()
    {
        // Aumentar velocidad con el tiempo
        timeElapsed += Time.deltaTime;
        currentSpeed = baseSpeed + (speedIncreaseRate * timeElapsed);

        // Mantener dirección pero con velocidad fija
        if (rb.linearVelocity.magnitude > 0.01f) // para evitar dividir entre 0
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed; // Usar linearVelocity en lugar de velocity
        }
    }
}

