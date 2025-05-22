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
    public float duration = 5f;

    [Header("Expand / Shrink Paddle")]
    public float paddleMultiplier = 1.5f;

    [Header("MultiBall")]
    public GameObject ballPrefab;
    public int extraBalls = 2;

    [Header("SpeedUp / SlowDown")]
    public float speedDelta = 5f;

    void Awake()
    {
        // desactiva gravedad y física para que no atraviese el suelo
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // collider en trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Update()
    {
        // cae siempre hacia -Z
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
                    // Escala al 150%, pero sin pasar de 200%
                    StartCoroutine(HandleScale(
                        paddle.transform,
                        1.5f,    // widthMultiplier
                        5f,      // duration
                        2f       // clampMultiplier: máximo 200% del ancho original
                    ));
                    break;
                }
            case PowerUpType.ShrinkPaddle:
                {
                    // Reduce al 50%, pero sin bajar del 75% (por ejemplo)
                    StartCoroutine(HandleScale(
                        paddle.transform,
                        0.5f,    // widthMultiplier
                        5f,      // duration
                        0.75f    // clampMultiplier: mínimo 75% del ancho original
                    ));
                    break;
                }
            case PowerUpType.PowerBallOn:
                {
                    var ball = GameObject.FindGameObjectWithTag("Ball");
                    if (ball != null)
                    {
                        // PowerBallBehaviour debe ignorar colisiones y atravesar bloques
                       // ball.AddComponent<PowerBallBehaviour>();
                    }
                }
                break;

            case PowerUpType.PowerBallOff:
                {
                    var ball = GameObject.FindGameObjectWithTag("Ball");
                    if (ball != null)
                    {
                       // var pb = ball.GetComponent<PowerBallBehaviour>();
                       // if (pb != null) Destroy(pb);
                        // asegúrate de restaurar collider normal en PowerBallBehaviour
                    }
                }
                break;

            case PowerUpType.MultiBall:
                for (int i = 0; i < extraBalls; i++)
                {
                    Instantiate(
                        ballPrefab,
                        paddle.transform.position + Vector3.up,
                        Quaternion.identity
                    );
                }
                break;

            case PowerUpType.Magnet:
                {
                    var ball = GameObject.FindGameObjectWithTag("Ball");
                    if (ball != null)
                    {
                       // ball.AddComponent<MagnetBall>();
                    }
                }
                break;

            case PowerUpType.NextLevel:
                // Carga la siguiente escena en el Build Settings
                int next = SceneManager.GetActiveScene().buildIndex + 1;
                if (next < SceneManager.sceneCountInBuildSettings)
                    SceneManager.LoadScene(next);
                break;

            case PowerUpType.SpeedUp:
                StartCoroutine(HandleSpeed(speedDelta));
                break;

            case PowerUpType.SlowDown:
                StartCoroutine(HandleSpeed(-speedDelta));
                break;

            case PowerUpType.ExtraLife:
                //GameManager.Instance.AddLife(1);
                break;
        }
    }

    IEnumerator HandleScale(Transform paddle, float widthMultiplier, float duration, float clampMultiplier)
    {
        // 1) Guardamos escala y posición originales
        Vector3 originalScale = paddle.localScale;
        Vector3 originalPosition = paddle.position;

        // 2) Calculamos el factor de escala, con límite
        float clampedMultiplier = widthMultiplier;
        if (widthMultiplier > 1f)
            clampedMultiplier = Mathf.Min(widthMultiplier, clampMultiplier); // máximo
        else if (widthMultiplier < 1f)
            clampedMultiplier = Mathf.Max(widthMultiplier, clampMultiplier); // mínimo

        // 3) Aplicamos sólo en X
        float finalScaleX = originalScale.x * clampedMultiplier;
        paddle.localScale = new Vector3(finalScaleX, originalScale.y, originalScale.z);

        // Nos aseguramos de que la posición no cambie (si tu pivot no está en el centro puedes necesitar ajustarlo en el modelo)
        paddle.position = originalPosition;

        // 4) Esperamos la duración
        yield return new WaitForSeconds(duration);

        // 5) Revertimos a los valores originales
        if (paddle != null)
        {
            paddle.localScale = originalScale;
            paddle.position = originalPosition;
        }
    }


    IEnumerator HandleSpeed(float delta)
    {
        // Ajusta la velocidad de TODAS las bolas
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (var b in balls)
        {
            var rb = b.GetComponent<Rigidbody>();
            rb.linearVelocity = rb.linearVelocity.normalized * (rb.linearVelocity.magnitude + delta);
        }

        yield return new WaitForSeconds(duration);

        // Revertir cambio
        balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (var b in balls)
        {
            var rb = b.GetComponent<Rigidbody>();
            rb.linearVelocity = rb.linearVelocity.normalized * (rb.linearVelocity.magnitude - delta);
        }
    }
}
