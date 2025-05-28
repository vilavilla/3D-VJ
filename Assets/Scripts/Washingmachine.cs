using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WashingMachine : MonoBehaviour
{
    [Header("Rotación del tambor")]
    [Tooltip("Grados por segundo")]
    public float rotationSpeed = 180f;

    [Header("Sonido de motor")]
    [Tooltip("Clip en bucle de la lavadora funcionando")]
    public AudioClip motorClip;
    [Range(0f, 1f)]
    public float motorVolume = 0.5f;

    AudioSource motorSrc;

    void Awake()
    {
        // Añade/configura AudioSource para el motor
        motorSrc = GetComponent<AudioSource>();
        motorSrc.clip = motorClip;
        motorSrc.volume = motorVolume;
        motorSrc.loop = true;
        motorSrc.playOnAwake = false;
    }

    void Start()
    {
        // Arranca el sonido
        if (motorClip != null)
            motorSrc.Play();
    }

    void Update()
    {
        // Rota el tambor en su eje local Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
