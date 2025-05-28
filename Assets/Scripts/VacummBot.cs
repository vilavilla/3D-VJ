using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(AudioSource))]
public class VacuumBot : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    [Tooltip("Velocidad de avance (unidades/segundo)")]
    public float speed = 2f;

    [Header("Ajustes de Giro Suave")]
    [Tooltip("Duración en segundos de la interpolación de giro")]
    public float turnDuration = 0.5f;

    [Header("Sonido de Choque")]
    [Tooltip("Clip que suena al chocar")]
    public AudioClip bumpClip;
    [Range(0f, 1f)]
    public float bumpVolume = 0.2f;

    [Header("Choque con plano Z")]
    [Tooltip("Coordenada mínima de Z donde rebota")]
    public float zMin = -14.5f;

    Rigidbody rb;
    Animator anim;
    AudioSource audioSrc;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();

        // Configuración básica de Rigidbody
        rb.constraints = RigidbodyConstraints.FreezePositionY
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        // 1) Avanza
        rb.linearVelocity = transform.forward * speed;

         if (transform.position.z <= zMin)
        {
            SimulateZBounce180();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ground")) return;

        // Choque físico
        Vector3 normal = col.contacts[0].normal;
        SimulateZBounce(normal, playPhysicsSound: true);
    }

    void SimulateZBounce180()
    {
        anim.SetTrigger("bump");

        if (bumpClip != null)
            audioSrc.PlayOneShot(bumpClip, bumpVolume);

        // Gira 180 grados en el eje Y
        Vector3 currentForward = transform.forward;
        Vector3 flippedDirection = -currentForward;
        StartCoroutine(SmoothTurn(flippedDirection));
    }
    // Método común para reproducir animación/sonido y girar
    void SimulateZBounce(Vector3 normal, bool playPhysicsSound = false)
    {
        // 1) Animación de choque
        anim.SetTrigger("bump");

        // 2) Sonido de choque (si viene de física o siempre)
        if (bumpClip != null && (playPhysicsSound || !playPhysicsSound))
            audioSrc.PlayOneShot(bumpClip, bumpVolume);

        // 3) Calcula nueva dirección reflejada
        Vector3 targetDir = Vector3.Reflect(transform.forward, normal).normalized;

        // 4) Lanza giro suave
        StartCoroutine(SmoothTurn(targetDir));

        // 5) Evita que siga acumulando velocidad fuera de límites
        rb.linearVelocity = Vector3.zero;
    }

    IEnumerator SmoothTurn(Vector3 targetDir)
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(targetDir, Vector3.up);

        float elapsed = 0f;
        while (elapsed < turnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / turnDuration);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        transform.rotation = endRot;
        rb.linearVelocity = Vector3.zero;
    }
}
