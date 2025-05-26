using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Movimiento")]
    public float baseSpeed = 10f;
    public float speedIncreaseRate = 0.2f;
    public float minForwardComponent = 0.3f;

    [Header("PowerBall")]
    public bool isPowerBall = false;
    public float powerBallDuration = 5f;

    private Rigidbody rb;
    private Collider ballCollider;
    private float currentSpeed;
    private float timeElapsed;
    private float powerBallTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // asegura que tenemos el collider, incluso si está en un child
        ballCollider = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
        if (ballCollider == null)
            Debug.LogError("Ball necesita un Collider.");
    }

    void Start()
    {
        currentSpeed = baseSpeed;
        // lanzamiento inicial
        Vector3 dir = new Vector3(Random.Range(-0.5f, 0.5f), 0f, 1f).normalized;
        rb.linearVelocity = dir * currentSpeed;
    }

    void Update()
    {
        // velocidad creciente
        timeElapsed += Time.deltaTime;
        currentSpeed = baseSpeed + speedIncreaseRate * timeElapsed;

        // cuenta atrás PowerBall
        if (isPowerBall)
        {
            powerBallTimer -= Time.deltaTime;
            if (powerBallTimer <= 0f)
                DeactivatePowerBall();
        }

        // asegurar componente Z mínimo
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
    }

    void OnCollisionEnter(Collision col)
    {
        if (!isPowerBall && col.gameObject.CompareTag("Block"))
        {
            // rebote normal: no hacemos nada, BlockController lo maneja
        }

        if (col.gameObject.CompareTag("Paddle"))
        {
            // rebote en paddle
            Vector3 contact = col.GetContact(0).point;
            float halfW = ballCollider.bounds.extents.x;
            float offset = contact.x - transform.position.x;
            float norm = Mathf.Clamp(offset / halfW, -1f, 1f);
            float angleRad = norm * 60f * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));
            rb.linearVelocity = dir.normalized * currentSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPowerBall && other.CompareTag("Block"))
        {
            Debug.Log($"PowerBall triggered block: {other.name}");
            var bc = other.GetComponent<BlockController>();
            if (bc != null)
                bc.DestroyByPowerBall();
        }
    }

    /// <summary>Activa PowerBall durante 'duration' segundos.</summary>
    public void ActivatePowerBall(float duration)
    {
        Debug.Log($"ActivatePowerBall called. Duration: {duration}");
        if (isPowerBall) return;
        isPowerBall = true;
        powerBallDuration = duration;
        powerBallTimer = duration;
        if (ballCollider != null)
            ballCollider.isTrigger = true;
    }

    /// <summary>Desactiva PowerBall y restaura collider.</summary>
    public void DeactivatePowerBall()
    {
        Debug.Log("DeactivatePowerBall called.");
        isPowerBall = false;
        if (ballCollider != null)
            ballCollider.isTrigger = false;
    }
}
