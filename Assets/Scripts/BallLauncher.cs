using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Ball : MonoBehaviour
{
    [Header("Movimiento")]
    public float baseSpeed = 5f;
    public float speedIncreaseRate = 0.0f;
    public float minForwardComponent = 0.3f;

    [Header("PowerBall")]
    public bool isPowerBall = false;
    public float powerBallDuration = 5f;

    [Header("Sonido de Colisi�n")]
    [Tooltip("Clip que suena cada vez que la bola choca")]
    public AudioClip collisionClip;
    [Range(0f, 1f)]
    public float collisionVolume = 0.3f;

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

        // Si cae por detr�s de Z = -18, restar vida y destruir
        if (transform.position.z < -18f)
        {
            LevelManager.Instance.LoseLife();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // Suena en cualquier choque f�sico (si no es PowerBall)
        if (!isPowerBall && collisionClip != null)
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
        else if (col.gameObject.CompareTag("RewardBall"))
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
        BounceFromPoint(col.GetContact(0).point);
    }

    void HandlePaddleTriggerBounce(Collider paddleCol)
    {
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
