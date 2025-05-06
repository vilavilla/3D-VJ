using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 10f; // Velocidad de movimiento


    public float bounceStrength = 5f; // Cuánto cambia la dirección

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();

            // Calcular el punto de contacto en X relativo al centro del paddle
            Vector3 contactPoint = collision.GetContact(0).point;
            float offset = contactPoint.x - transform.position.x;
            float paddleWidth = transform.localScale.x / 2f;

            float directionX = offset / paddleWidth; // Normalizamos -1 (izq) a 1 (der)

            // Mantener velocidad actual pero ajustar dirección
            Vector3 newDirection = new Vector3(directionX * bounceStrength, 0f, 1f).normalized;

            float currentSpeed = ballRb.linearVelocity.magnitude; // Usar linearVelocity en lugar de velocity
            ballRb.linearVelocity = newDirection * currentSpeed; // Usar linearVelocity en lugar de velocity
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float input = Input.GetAxis("Horizontal"); // Flechas izquierda/derecha o A/D
        transform.Translate(Vector3.right * input * speed * Time.deltaTime);
    }
}
