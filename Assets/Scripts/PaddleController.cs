using System.Runtime.ExceptionServices;
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
 

    [Header("Extensiones para power-up Expand/Shrink")]
    public GameObject leftExt;
    public GameObject rightExt;

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

        // — AUTO-ASIGNACIÓN de extensiones si no están puestas en el Inspector —
        if (leftExt == null)
        {
            var t = transform.Find("LeftExt");
            if (t != null) leftExt = t.gameObject;
        }
        if (rightExt == null)
        {
            var t = transform.Find("RightExt");
            if (t != null) rightExt = t.gameObject;
        }

        // Asegúrate de que empiecen ocultas
        SetExtended(false);
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

    /// <summary>
    /// Muestra u oculta las secciones extra del paddle.
    /// </summary>
    /// <summary>
    /// Activa progresivamente las extensiones: primero la derecha, luego la izquierda.
    /// Al desactivar, se hace al revés: primero la izquierda, luego la derecha.
    /// </summary>
    public void SetExtended(bool on)
    {
        if (on)
        {
            // Si todavía no está la extensión derecha, ponla
            if (rightExt != null && !rightExt.activeSelf)
            {
                rightExt.SetActive(true);
            }
            // Si la derecha ya está y la izquierda no, pon la izquierda
            else if (leftExt != null && !leftExt.activeSelf)
            {
                leftExt.SetActive(true);
            }
        }
        else
        {
            // Al quitar, primero apaga la izquierda si está activa
            if (leftExt != null && leftExt.activeSelf)
            {
                leftExt.SetActive(false);
            }
            // Si la izquierda ya estaba apagada, apaga la derecha
            else if (rightExt != null && rightExt.activeSelf)
            {
                rightExt.SetActive(false);
            }
        }
    }
}
