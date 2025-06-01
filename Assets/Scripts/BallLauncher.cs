using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Ball : MonoBehaviour
{
    [Header("Movimiento")]
    public float baseSpeed = 5f;
    public float speedIncreaseRate = 0.0f;
    public float minForwardComponent = 0.3f;
    public float maxBounceAngle = 60f;

    [Header("PowerBall")]
    public bool isPowerBall = false;
    public float powerBallDuration = 5f;

    [Header("Sonido de Colisi�n")]
    [Tooltip("Clip que suena cada vez que la bola choca")]
    public AudioClip collisionClip;
    private float collisionVolume = 0.3f;

    [Header("PowerBall Magnet")]
    public bool pendingMagnet = false;

    private Rigidbody rb;
    private Collider ballCollider;
    private AudioSource audioSrc;
    private float currentSpeed;
    private float timeElapsed;
    private float powerBallTimer;
    private bool isStuckToPaddle = false;
    private Transform paddle;
    private Vector3 magnetOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ballCollider = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
        if (ballCollider == null)
            Debug.LogError("Ball necesita un Collider.");

        audioSrc = GetComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.loop = false;
    }

    void Start()
    {
        currentSpeed = baseSpeed;
        // lanzamiento inicial hacia adelante
        Vector3 dir = new Vector3(Random.Range(-0.5f, 0.5f), 0f, -1f).normalized;
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
                isStuckToPaddle = false;
                rb.linearVelocity = Vector3.forward * currentSpeed;
            }
            return;
        }

        if (isPowerBall)
        {
            powerBallTimer -= Time.deltaTime;
            if (ballCollider != null && !ballCollider.isTrigger)
                ballCollider.isTrigger = true;
            if (powerBallTimer <= 0f)
                DeactivatePowerBall();
        }

        // Asegura componente Z m�nimo
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

        if (transform.position.z < -18f)
        {
            // Cuenta cuántas bolas hay
            var balls = GameObject.FindGameObjectsWithTag("Ball");
            if (balls.Length <= 1)
            {
                // Si era la última bola, restamos vida
                LevelManager.Instance.LoseLife();
            }
            // Siempre destruye esta bola
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // Suena en cualquier choque f�sico (si no es PowerBall)
        if (!isPowerBall && collisionClip != null && !col.gameObject.CompareTag("Ground"))
            audioSrc.PlayOneShot(collisionClip, collisionVolume);

        if (isPowerBall)
        {
            Debug.Log($"Ignorando colisi�n con {col.gameObject.name} porque est� activo PowerBall.");
            return;
        }

        if (col.gameObject.CompareTag("Paddle"))
        {
            if (pendingMagnet)
            {
                pendingMagnet = false;
                isStuckToPaddle = true;
                paddle = col.transform;
                magnetOffset = transform.position - paddle.position;
                rb.linearVelocity = Vector3.zero;
                return;
            }
            HandlePaddleBounce(col);
        }
        else if (col.gameObject.CompareTag("RewardBall") || (col.gameObject.CompareTag("PaddleAux" )))
        {
            Physics.IgnoreCollision(ballCollider, col.collider);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isPowerBall) return;

        switch (other.tag)
        {
            case "Paddle":
                HandlePaddleTriggerBounce(other);
                break;
            case "Wall":
                Vector3 closest = other.ClosestPoint(transform.position);
                Vector3 normal = (transform.position - closest).normalized;
                Vector3 newDir = Vector3.Reflect(rb.linearVelocity.normalized, normal);
                rb.linearVelocity = newDir * currentSpeed;
                break;
            default:
                break;
        }
    }

    void HandlePaddleBounce(Collision col)
    {
        // 1) Averiguamos qué collider del paddle tocó la bola.
        //    Si tu paddle está compuesto por varios colliders (Centro, LeftExt, RightExt),
        //    col.collider será el BoxCollider concreto que recibió el choque.
        //    Tomamos la posición X del punto de contacto:
        Vector3 contactPoint = col.GetContact(0).point;

        // 2) Calculamos el offset horizontal relativo al centro del paddle.
        //    col.transform.position es la posición del GameObject que tiene el PaddleController.
        //    (Si el PaddleController está en PaddleRoot, col.transform.parent coincide con PaletteRoot).
        float paddleCenterX = col.transform.position.x;
        float offset = contactPoint.x - paddleCenterX;

        // 3) Normalizamos ese offset con el ancho del collider activo:
        //    halfWidth = distancia entre el centro y el extremo del collider donde ocurrió el choque.
        float halfWidth = 0f;
        Collider usedCollider = col.collider; // El collider “físico” que recibió el golpe
        halfWidth = usedCollider.bounds.extents.x;

        // 4) Obtenemos un valor entre -1 y +1:
        float normalizedOffset = Mathf.Clamp(offset / halfWidth, -1f, 1f);

        // 5) Calculamos el ángulo de rebote en radianes:
        //    −1 → 60° hacia la izquierda   (máximo rebote hacia X negativa)
        //    0  → rebote “recto” (Z positiva)
        //    +1 → 60° hacia la derecha  (máximo rebote hacia X positiva)
        float bounceAngleRad = normalizedOffset * maxBounceAngle * Mathf.Deg2Rad;

        // 6) Construimos la nueva dirección: X = sin, Z = cos
        Vector3 newDirection = new Vector3(Mathf.Sin(bounceAngleRad), 0f, Mathf.Cos(bounceAngleRad)).normalized;

        // 7) Mantenemos la velocidad que llevaba la bola antes del impacto:
        float speedBefore = rb.linearVelocity.magnitude;

        // 8) Asignamos la nueva velocidad:
        rb.linearVelocity = newDirection * speedBefore;
    }

    /// <summary>
    /// Lógica de rebote cuando es PowerBall y usa trigger en lugar de física normal.
    /// </summary>
    void HandlePaddleTriggerBounce(Collider other)
    {
        // 1) Similar: obtenemos el punto más cercano en el paddle al centro de la bola.
        Vector3 closestPoint = other.ClosestPoint(transform.position);

        // 2) Offset horizontal respecto al centro del Paddle:
        float paddleCenterX = other.transform.position.x;
        float offset = closestPoint.x - paddleCenterX;

        // 3) HalfWidth según el collider de paddle que usamos para trigger (puede ser cualquiera de los hijos)
        float halfWidth = other.bounds.extents.x;

        // 4) Normalizamos:
        float normalizedOffset = Mathf.Clamp(offset / halfWidth, -1f, 1f);

        // 5) Cálculo del ángulo en radianes:
        float bounceAngleRad = normalizedOffset * maxBounceAngle * Mathf.Deg2Rad;

        // 6) Dirección nueva:
        Vector3 newDirection = new Vector3(Mathf.Sin(bounceAngleRad), 0f, Mathf.Cos(bounceAngleRad)).normalized;

        // 7) Mantener la velocidad actual (currentSpeed es tu campo que guarda la velocidad actual de la bola):
        rb.linearVelocity = newDirection * currentSpeed;
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

    public void ActivatePowerBall(float duration)
    {
        if (isPowerBall) return;
        isPowerBall = true;
        powerBallDuration = duration;
        powerBallTimer = duration;
        if (ballCollider != null)
            ballCollider.isTrigger = true;
    }

    public void DeactivatePowerBall()
    {
        isPowerBall = false;
        if (ballCollider != null)
            ballCollider.isTrigger = false;
    }

    public void ActivateMagnetOnNextPaddleHit()
    {
        pendingMagnet = true;
    }
}
