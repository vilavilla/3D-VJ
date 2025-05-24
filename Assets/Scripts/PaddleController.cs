using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PaddleController : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad de desplazamiento en unidades/segundo")]
    public float speed = 10f;

    [Header("Rebote pelota")]
    [Tooltip("Ángulo máximo de rebote hacia los lados (grados)")]
    public float maxBounceAngle = 60f;

    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        // Aseguramos el Rigidbody y lo configuramos
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        // Dinámico para que la física lo frene
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        // Sólo libre en X, lo demás congelado
        rb.constraints = RigidbodyConstraints.FreezePositionY
                       | RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotation;

        // Aseguramos el Collider (no trigger, para que rechace)
        col = GetComponent<Collider>();
        if (col == null)
            Debug.LogError("Paddle necesita un Collider para funcionar correctamente.");
    }

    void FixedUpdate()
    {
        // Movemos por linearVelocity en vez de MovePosition
        float h = Input.GetAxis("Horizontal"); // -1..+1
        Vector3 vel = new Vector3(h * speed, 0f, 0f);
        rb.linearVelocity = vel;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Rebote de la bola (sin cambios)
        if (!collision.gameObject.CompareTag("Ball")) return;
        Rigidbody ballRb = collision.rigidbody;
        if (ballRb == null) return;

        Vector3 contactPt = collision.GetContact(0).point;
        float offset = contactPt.x - transform.position.x;
        float halfWidth = col.bounds.extents.x;

        float norm = Mathf.Clamp(offset / halfWidth, -1f, 1f);
        float bounceRad = norm * maxBounceAngle * Mathf.Deg2Rad;

        Vector3 newDir = new Vector3(Mathf.Sin(bounceRad), 0f, Mathf.Cos(bounceRad)).normalized;
        float speedBefore = ballRb.linearVelocity.magnitude;
        ballRb.linearVelocity = newDir * speedBefore;
    }
}
