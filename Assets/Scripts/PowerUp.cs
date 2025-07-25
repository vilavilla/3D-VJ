using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PowerUpType
{
    ExpandPaddle,
    ShrinkPaddle,
    PowerBallOn,
    PowerBallOff,
    MultiBall,
    Magnet,
    SpeedUp,
    SlowDown,
    ExtraLife
}

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PowerUp : MonoBehaviour
{
    [Header("Movimiento de caída hacia -Z")]
    public float fallSpeed = 5f;

    [Header("Configuración común")]
    public PowerUpType type;
    private float duration = 5.0f;

    [Header("Expand / Shrink Paddle")]
    public float paddleMultiplier = 1.5f;
    public float clampMaxMultiplier = 2f;
    public float clampMinMultiplier = 0.75f;

    [Header("MultiBall")]
    public GameObject ballPrefab;
   

    [Header("SpeedUp / SlowDown")]
    public float speedDelta = 2f;

    [Header("Audio & Puntuación")]
    public AudioClip pickUpSound;     // Asigna en Inspector un clip de recogida
    public int scoreAmount = 100;     // Puntos que suma este PowerUp al ScoreManager

    void Awake()
    {
        // Caída continua sin gravedad
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // Trigger para detectar paddle
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Update()
    {
        // Movimiento hacia -Z
        transform.Translate(Vector3.back * fallSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"PowerUp of type {type} collided with {other.name} (tag={other.tag})");
        if (!other.CompareTag("Paddle")) return;

        // 1) Reproducir sonido de recogida (en la posición de la cámara principal)
        if (pickUpSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(pickUpSound, Camera.main.transform.position);
        }

        // 2) Sumar puntos al ScoreManager
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPoints(scoreAmount);
        }

        Debug.Log("Matched Paddle, applying effect");
        ApplyEffect(other.gameObject);
        Destroy(gameObject);
    }

    void ApplyEffect(GameObject paddle)
    {
        switch (type)
        {
            case PowerUpType.ExpandPaddle:
                {
                    var paddleCtrl = paddle.GetComponent<PaddleController>();
                    if (paddleCtrl != null)
                        StartCoroutine(HandleExtend(paddleCtrl, duration, true));
                    else
                    {
                        Debug.Log("PowerUp chocó con " + paddle.name + " pero no encontró PaddleController arriba.");


                    }

                }
                break;

            case PowerUpType.ShrinkPaddle:
                {
                    var paddleCtrl = paddle.GetComponent<PaddleController>();
                    if (paddleCtrl != null)
                        StartCoroutine(HandleExtend(paddleCtrl, duration, false));

                }
                break;

          case PowerUpType.PowerBallOn:
                Debug.Log("PowerUpType.PowerBallOn branch");
                var ballOn = GameObject.FindGameObjectWithTag("Ball")?.GetComponent<Ball>();
                if (ballOn == null)
                    Debug.LogWarning("No Ball with Ball component found!");
                else
                {
                    Debug.Log($"Calling ActivatePowerBall {duration}");

                    ballOn.ActivatePowerBall();
                }
                break;

            case PowerUpType.PowerBallOff:
                Debug.Log("PowerUpType.PowerBallOff branch");
                var ballOff = GameObject.FindGameObjectWithTag("Ball")?.GetComponent<Ball>();
                if (ballOff == null)
                    Debug.LogWarning("No Ball with Ball component found for OFF!");
                else
                {
                    Debug.Log("Calling DeactivatePowerBall");
                    ballOff.DeactivatePowerBall();
                }
                break;

            case PowerUpType.MultiBall:
                GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

                foreach (var b in balls)
                {
                    Instantiate(
                          ballPrefab,
                          paddle.transform.position
                              + Vector3.up * 0.5f
                              + Vector3.forward * 5.5f,  // +Z positivo (0.5f + 2f)
                          Quaternion.identity
                      );

                }
                break;


            case PowerUpType.Magnet:
                {
                    var ball = GameObject.FindGameObjectWithTag("Ball");
                    if (ball != null)
                    {
                        var b = ball.GetComponent<Ball>();
                        if (b != null)
                            b.ActivateMagnetOnNextPaddleHit();
                    }
                    break;
                }


            case PowerUpType.SpeedUp:
                StartCoroutine(HandleSpeed(+speedDelta));
                break;

            case PowerUpType.SlowDown:
                StartCoroutine(HandleSpeed(-speedDelta));
                break;

            case PowerUpType.ExtraLife:
                LevelManager.Instance.AddLife();
                break;
        }
    }

    IEnumerator HandleExtend(PaddleController pc, float duration, bool expand)
    {
        pc.SetExtended(expand);
        if (!expand) yield break;
        yield return new WaitForSeconds(duration);
        pc.SetExtended(false);
    }

    IEnumerator HandleSpeed(float delta)
    {
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (var b in balls)
        {
            var rb = b.GetComponent<Rigidbody>();
            rb.linearVelocity = rb.linearVelocity.normalized * (rb.linearVelocity.magnitude + delta);
        }

        yield return new WaitForSeconds(duration);

        // Revertir
        balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (var b in balls)
        {
            var rb = b.GetComponent<Rigidbody>();
            rb.linearVelocity = rb.linearVelocity.normalized * (rb.linearVelocity.magnitude - delta);
        }
    }
}
