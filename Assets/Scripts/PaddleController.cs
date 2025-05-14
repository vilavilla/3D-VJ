using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 10f; // Velocidad de movimiento

    public float bounceStrength = 5f; // Cuánto cambia la dirección
    public float maxBounceAngle = 60f; // Máximo ángulo hacia los lados

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();

            // Punto de impacto
            Vector3 contactPoint = collision.GetContact(0).point;
            float offset = contactPoint.x - transform.position.x;
            float paddleWidth = transform.localScale.x / 2f;

            // Normalizamos el offset (-1 izquierda, 1 derecha)
            float normalizedOffset = Mathf.Clamp(offset / paddleWidth, -1f, 1f);

            // Calcula el ángulo de rebote
            float bounceAngle = normalizedOffset * maxBounceAngle;
            float bounceAngleRad = bounceAngle * Mathf.Deg2Rad;

            // Nueva dirección basada en el ángulo
            Vector3 newDirection = new Vector3(Mathf.Sin(bounceAngleRad), 0f, Mathf.Cos(bounceAngleRad));

            // Mantener la velocidad actual
            float currentSpeed = ballRb.linearVelocity.magnitude; // Cambiado de 'velocity' a 'linearVelocity'
            ballRb.linearVelocity = newDirection.normalized * currentSpeed; // Cambiado de 'velocity' a 'linearVelocity'
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float input = Input.GetAxis("Horizontal"); // -1 a +1
        Vector3 movimiento = Vector3.right * input * speed * Time.deltaTime;
        transform.Translate(movimiento, Space.World);
    }

}
