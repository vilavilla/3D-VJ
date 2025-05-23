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

        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezePositionY
                       | RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotation;

        // Aseguramos el Collider (no trigger, provides contacts ON)
        col = GetComponent<Collider>();
        if (col == null)
            Debug.LogError("Paddle necesita un Collider para funcionar correctamente.");
    }

    void FixedUpdate()
    {
        // Movimiento horizontal con MovePosition para respetar colisiones
        float h = Input.GetAxis("Horizontal"); // -1..+1
        Vector3 target = rb.position + Vector3.right * h * speed * Time.fixedDeltaTime;
        rb.MovePosition(target);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Solo queremos rebotar contra objetos con tag "Ball"
        if (!collision.gameObject.CompareTag("Ball")) return;

        Rigidbody ballRb = collision.rigidbody;
        if (ballRb == null) return;

        // 1) Calculamos offset X del punto de contacto
        Vector3 contactPt = collision.GetContact(0).point;
        float offset = contactPt.x - transform.position.x;

        // 2) Mitad del ancho real de la pala
        float halfWidth = col.bounds.extents.x;

        // 3) Normalizamos a [-1,1] y calculamos el ángulo en radianes
        float norm = Mathf.Clamp(offset / halfWidth, -1f, 1f);
        float bounceRad = norm * maxBounceAngle * Mathf.Deg2Rad;

        // 4) Nueva dirección en X-Z
        Vector3 newDir = new Vector3(Mathf.Sin(bounceRad), 0f, Mathf.Cos(bounceRad)).normalized;

        // 5) Conservamos la velocidad de la bola usando linearVelocity
        float speedBefore = ballRb.linearVelocity.magnitude;
        ballRb.linearVelocity = newDir * speedBefore;
    }
}
