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
        // 1) velocidad progresiva
        timeElapsed += Time.deltaTime;
        currentSpeed = baseSpeed + speedIncreaseRate * timeElapsed;

        // 2) PowerBall timer
        if (isPowerBall)
        {
            powerBallTimer -= Time.deltaTime;

            //  fuerza que el collider esté siempre como trigger mientras dura PowerBall
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
    }

    void OnCollisionEnter(Collision col)
    {
        if (isPowerBall)
        {
            Debug.Log($" Ignorando colisión con {col.gameObject.name} porque está activo PowerBall.");
            return; //Previene llamadas indebidas que puedan causar reset
        }

        Debug.Log($" Colisión con {col.gameObject.name}, Trigger: {ballCollider.isTrigger}, PowerBall: {isPowerBall}");

        // Rebote normal contra la paleta
        if (col.gameObject.CompareTag("Paddle"))
            HandlePaddleBounce(col);
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log($" Contacto con {other.tag}, Trigger: {ballCollider.isTrigger}, PowerBall: {isPowerBall}");

        if (!isPowerBall) return;

        switch (other.tag)
        {
            // 1) Blocks: destruye y atraviesa
            case "Block":
                var bc = other.GetComponent<BlockController>();
                if (bc != null) bc.DestroyByPowerBall();
                break;

            // 2) Paddle: rebote angular personalizado
            case "Paddle":
                HandlePaddleTriggerBounce(other);
                break;

            // 3) Walls: cualquiera de las 4 etiquetas "Wall" (izq, der, frente, atrás)
            case "Wall":
                // Calculamos la normal entre el punto más cercano de la pared y la bola
                Vector3 closest = other.ClosestPoint(transform.position);
                Vector3 normal = (transform.position - closest).normalized;
                rb.linearVelocity = Vector3.Reflect(rb.linearVelocity, normal).normalized * currentSpeed;
                break;

            // 4) Suelo/destrucción no queremos rebotar: atraviesa sin acción
            default:
                // otros tags: nada
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
        isPowerBall = false;
        if (ballCollider != null)
            ballCollider.isTrigger = false;
    }
}
