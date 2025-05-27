using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class Ball : MonoBehaviour
{
    [Header("Movimiento")]
    public float baseSpeed = 5f;
    public float speedIncreaseRate = 0.0f;
    public float minForwardComponent = 0.3f;

    [Header("PowerBall")]
    public bool isPowerBall = false;
    public float powerBallDuration = 5f;

    private Rigidbody rb;
    private Collider ballCollider;
    private float currentSpeed;
    private float timeElapsed;
    private float powerBallTimer;

    private bool pendingMagnet = false;
    private bool isStuckToPaddle = false;
    private Transform paddle;
    private Vector3 magnetOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ballCollider = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
        if (ballCollider == null)
            Debug.LogError("Ball necesita un Collider.");
    }

    void Start()
    {
        currentSpeed = baseSpeed;
        // lanzamiento inicial hacia adelante
        Vector3 dir = new Vector3(Random.Range(-0.5f, 0.5f), 0f, 1f).normalized;
        rb.linearVelocity = dir * currentSpeed;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        currentSpeed = baseSpeed + speedIncreaseRate * timeElapsed;

        if (isStuckToPaddle && paddle != null)
        {
            transform.position = paddle.position + magnetOffset;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Bola liberada del paddle");
                isStuckToPaddle = false;
                rb.linearVelocity = Vector3.forward * currentSpeed; // o la dirección que quieras
            }

            return; // no actualizar velocidad ni movimiento
        }

        // 1) velocidad progresiva (ya calculada más arriba)

        // 2) PowerBall timer
        if (isPowerBall)
        {
            powerBallTimer -= Time.deltaTime;

            // fuerza que el collider esté siempre como trigger mientras dura PowerBall
            if (ballCollider != null && !ballCollider.isTrigger)
            {
                Debug.Log(" Reforzando isTrigger = true");
                ballCollider.isTrigger = true;
            }

            if (powerBallTimer <= 0f)
                DeactivatePowerBall();
        }

        // 3) asegurar componente Z mínimo
        Vector3 vel = rb.linearVelocity;
        float zComp = Mathf.Abs(vel.normalized.z);
        if (zComp < minForwardComponent)
        {
            float sx = Mathf.Sign(vel.x), sz = Mathf.Sign(vel.z);
            Vector3 newDir = new Vector3(0.7f * sx, 0f, 0.7f * sz).normalized;
            vel = newDir * currentSpeed;
        }
        else
        {
            vel = vel.normalized * currentSpeed;
        }
        rb.linearVelocity = vel;

        // ——> AÑADIDO: si cae por detrás de Z = -18, restar vida y destruir
        if (transform.position.z < -18f)
        {
            LevelManager.Instance.LoseLife();
            Destroy(gameObject);
            return;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (isPowerBall)
        {
            Debug.Log($" Ignorando colisión con {col.gameObject.name} porque está activo PowerBall.");
            return; // Previene llamadas indebidas que puedan causar reset
        }

        if (col.gameObject.CompareTag("Paddle"))
        {
            if (pendingMagnet)
            {
                Debug.Log(" Bola se pega al paddle");
                pendingMagnet = false;
                isStuckToPaddle = true;
                paddle = col.transform;
                magnetOffset = transform.position - paddle.position;
                rb.linearVelocity = Vector3.zero;
                return;
            }

            HandlePaddleBounce(col);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isPowerBall) return;

        switch (other.tag)
        {
            // 2) Paddle: rebote angular personalizado
            case "Paddle":
                HandlePaddleTriggerBounce(other);
                break;

            // 3) Walls: cualquiera de las 4 etiquetas "Wall" (izq, der, frente, atrás)
            case "Wall":
                {
                    // Punto más cercano sobre el collider de la pared
                    Vector3 closest = other.ClosestPoint(transform.position);

                    // Normal orientada hacia fuera del muro (de la pared hacia la bola)
                    Vector3 normal = (transform.position - closest).normalized;

                    // Rebote con reflexión vectorial y mantenimiento de velocidad
                    Vector3 newDirection = Vector3.Reflect(rb.linearVelocity.normalized, normal);
                    rb.linearVelocity = newDirection * currentSpeed;
                }
                break;

            // 4) Suelo/destrucción: no se rebota, atraviesa
            default:
                // otros tags: sin acción
                break;
        }
    }

    void HandlePaddleBounce(Collision col)
    {
        Vector3 contact = col.GetContact(0).point;
        BounceFromPoint(contact);
    }

    void HandlePaddleTriggerBounce(Collider paddleCol)
    {
        // aproximamos punto de contacto en el plano Z del paddle
        Vector3 contact = new Vector3(transform.position.x, 0f, paddleCol.transform.position.z);
        BounceFromPoint(contact);
    }

    void BounceFromPoint(Vector3 contactPt)
    {
        float halfW = ballCollider.bounds.extents.x;
        float offset = contactPt.x - transform.position.x;
        float norm = Mathf.Clamp(offset / halfW, -1f, 1f);
        float angleRad = norm * 60f * Mathf.Deg2Rad;

        Vector3 dir = new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad)).normalized;
        rb.linearVelocity = dir * currentSpeed;
    }

    /// <summary>Activa PowerBall durante 'duration' segundos.</summary>
    public void ActivatePowerBall(float duration)
    {
        Debug.Log($" activar, PowerBall: {isPowerBall}");

        if (isPowerBall) return;
        isPowerBall = true;
        powerBallDuration = duration;
        powerBallTimer = duration;
        if (ballCollider != null)
            ballCollider.isTrigger = true;
    }

    /// <summary>Desactiva PowerBall restaurando collider normal.</summary>
    public void DeactivatePowerBall()
    {
        Debug.Log($" desactivar, PowerBall: {isPowerBall}");

        isPowerBall = false;

        if (ballCollider != null)
            ballCollider.isTrigger = false;
    }

    public void ActivateMagnetOnNextPaddleHit()
    {
        Debug.Log(" Power-up Magnet activado: se aplicará en el siguiente rebote");
        pendingMagnet = true;
    }
}