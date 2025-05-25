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
    NextLevel,
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
    public float duration = 1f;

    [Header("Expand / Shrink Paddle")]
    public float paddleMultiplier = 1.5f;
    public float clampMaxMultiplier = 2f;
    public float clampMinMultiplier = 0.75f;

    [Header("MultiBall")]
    public GameObject ballPrefab;
    public int extraBalls = 2;

    [Header("SpeedUp / SlowDown")]
    public float speedDelta = 2f;

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
        if (!other.CompareTag("Paddle")) return;
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
                {
                    /*var ball = GameObject.FindGameObjectWithTag("Ball");
                    if (ball != null
                        && ball.GetComponent<PowerBallBehaviour>() == null)
                    {
                        ball.AddComponent<PowerBallBehaviour>();
                    }*/
                }
                break;

            case PowerUpType.PowerBallOff:
                {
                   /* var ball = GameObject.FindGameObjectWithTag("Ball");
                    var pb = ball?.GetComponent<PowerBallBehaviour>();
                    if (pb != null) Destroy(pb);*/
                }
                break;

            case PowerUpType.MultiBall:
                for (int i = 0; i < extraBalls; i++)
                {
                    Instantiate(
                        ballPrefab,
                        paddle.transform.position + Vector3.up * 0.5f,
                        Quaternion.identity
                    );
                }
                break;

            case PowerUpType.Magnet:
                {
                    /*var ball = GameObject.FindGameObjectWithTag("Ball");
                    if (ball != null
                        && ball.GetComponent<MagnetBall>() == null)
                    {
                        ball.AddComponent<MagnetBall>();
                    }*/
                }
                break;

            case PowerUpType.NextLevel:
                {
                    int next = SceneManager.GetActiveScene().buildIndex + 1;
                    if (next < SceneManager.sceneCountInBuildSettings)
                        SceneManager.LoadScene(next);
                }
                break;

            case PowerUpType.SpeedUp:
                StartCoroutine(HandleSpeed(+speedDelta));
                break;

            case PowerUpType.SlowDown:
                StartCoroutine(HandleSpeed(-speedDelta));
                break;

            case PowerUpType.ExtraLife:
               // GameManager.Instance.AddLife(1);
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
