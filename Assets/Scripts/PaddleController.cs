using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PaddleController : MonoBehaviour
{
    private float speed = 25f;

  
    private Rigidbody rb;

    [Header("Colliders del Paddle (estos tres deben vivir en hijos de PaddleRoot)")]
    // Asigna manualmente en el Inspector los 3 BoxCollider que tienes repartidos en "Centro", "LeftExt" y "RightExt"
    public Collider centerCollider;
    public Collider leftExtCollider;
    public Collider rightExtCollider;

    [Header("Extensiones para power-up Expand/Shrink (solo visual)")]
    public GameObject leftExt;   // El hijo que contiene la malla + BoxCollider de la extensión izquierda
    public GameObject rightExt;  // El hijo que contiene la malla + BoxCollider de la extensión derecha
    private float minX = -22.3f; // Límite izquierdo del paddle  
    private float maxX = 24f; // Límite derecho del paddle

    void Awake()
    {
        // 1) Aseguramos el Rigidbody y lo configuramos
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezePositionY
                       | RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotation;

        // 2) Al iniciar, DESACTIVAMOS TODOS los colliders de las extensiones y el central:
        if (centerCollider != null) centerCollider.enabled = true;
        if (leftExtCollider != null) leftExtCollider.enabled = false;
        if (rightExtCollider != null) rightExtCollider.enabled = false;

        // 3) Ocultamos las mallas de las extensiones (porque al inicio no deben verse)
        if (leftExt != null) leftExt.SetActive(false);
        if (rightExt != null) rightExt.SetActive(false);
    }

    void FixedUpdate()
    {
        // 1) Leemos el input horizontal (-1..+1) y calculamos la velocidad deseada en X
        float h = Input.GetAxis("Horizontal");
        Vector3 vel = new Vector3(h * speed, 0f, 0f);
        rb.linearVelocity = vel;

        // 2) Una vez aplicada la velocidad, limitamos la posición X para que no salga de [minX, maxX]
        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        transform.position = clampedPos;
    }

   

    /// <summary>
    /// Activa o desactiva progresivamente las secciones del paddle:
    /// - on == true: primero habilita extensión derecha, luego extensión izquierda, luego zona central
    /// - on == false: primero inhabilita zona central, luego extensión izquierda, luego extensión derecha
    /// </summary>
    public void SetExtended(bool on)
    {
        if (on)
        {
            // 1) Si aún no está activa la sección DERECHA, habilítala
            if (rightExtCollider != null && !rightExtCollider.enabled)
            {
                rightExtCollider.enabled = true;
                if (rightExt != null) rightExt.SetActive(true);
                return;
            }
            // 2) Si la derecha ya está activa, habilita la IZQUIERDA
            if (leftExtCollider != null && !leftExtCollider.enabled)
            {
                leftExtCollider.enabled = true;
                if (leftExt != null) leftExt.SetActive(true);
                return;
            }
            // 3) Si ambas extensiones ya están activas, habilita la zona CENTRAL final
            if (centerCollider != null && !centerCollider.enabled)
            {
                centerCollider.enabled = true;
                return;
            }
        }
        else
        {
            // 1) Si la zona CENTRAL está activa, desactívala primero
            if (centerCollider != null && centerCollider.enabled)
            {
                centerCollider.enabled = false;
                return;
            }
            // 2) Si la central ya estaba inactiva, desactiva la extensión IZQUIERDA
            if (leftExtCollider != null && leftExtCollider.enabled)
            {
                leftExtCollider.enabled = false;
                if (leftExt != null) leftExt.SetActive(false);
                return;
            }
            // 3) Si la izquierda ya estaba inactiva, desactiva la extensión DERECHA
            if (rightExtCollider != null && rightExtCollider.enabled)
            {
                rightExtCollider.enabled = false;
                if (rightExt != null) rightExt.SetActive(false);
                return;
            }
        }
    }
}
