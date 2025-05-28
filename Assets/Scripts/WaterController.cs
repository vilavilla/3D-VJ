using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BathtubFillController : MonoBehaviour
{
    [Header("Plano de agua (hijo de la bañera)")]
    public Transform waterPlane;

    [Header("Velocidad de llenado (unidades de escala Y por segundo)")]
    public float fillSpeed = 0.2f;

    [Header("Escala Y máxima (1 = lleno hasta la altura del plano original)")]
    public float maxScaleY = 1f;

    [Header("Objeto a activar cuando la bañera se rompa")]
    public GameObject objectToActivate;

    [Header("Audio de llenado")]
    [Tooltip("Clip en bucle mientras se llena")]
    public AudioClip fillClip;
    [Range(0f, 1f)]
    public float fillVolume = 0.2f;

    [Header("Audio de derrame")]
    [Tooltip("Clip que suena cuando se rompe/derrama")]
    public AudioClip spillClip;
    [Range(0f, 1f)]
    public float spillVolume = 0.3f;

    private float currentScaleY = 0.1f;
    private AudioSource audioSrc;

    void Awake()
    {
        // AudioSource para ambos clips
        audioSrc = GetComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.loop = true;
    }

    void Start()
    {
        // Aplica escala inicial de agua
        currentScaleY = 0.1f;
        ApplyWaterScale();

        // Arranca el sonido de llenado en bucle
        if (fillClip != null)
        {
            audioSrc.clip = fillClip;
            audioSrc.volume = fillVolume;
            audioSrc.loop = true;
            audioSrc.Play();
        }
    }

    void Update()
    {
        // Llenado automático
        if (currentScaleY < maxScaleY)
        {
            currentScaleY += fillSpeed * Time.deltaTime;
            if (currentScaleY >= maxScaleY)
            {
                currentScaleY = maxScaleY;
                // Al llegar al tope, paramos el sonido de llenado
                if (audioSrc.isPlaying)
                    audioSrc.Stop();
            }
            ApplyWaterScale();
        }
    }

    void ApplyWaterScale()
    {
        // Escalado local en Y (XZ fijos según tu escala deseada)
        waterPlane.localScale = new Vector3(0.8f, currentScaleY, 1.7f);

        // Compensa posición Y para que la base quede fija
        float yOffset = currentScaleY * 0.5f;
        waterPlane.localPosition = new Vector3(
            waterPlane.localPosition.x,
            yOffset,
            waterPlane.localPosition.z
        );
    }

    // Este método se llama justo antes de que se destruya este GameObject
    void OnDestroy()
    {
        // Reproduce derrame
        if (spillClip != null)
            audioSrc.PlayOneShot(spillClip, spillVolume);

        // Activa el objeto de derrame y su animación
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);

            // Reproduce animación “WaterSpill” si existe
            var anim = objectToActivate.GetComponent<Animation>();
            if (anim != null && anim.GetClip("WaterSpill") != null)
            {
                anim.Play("WaterSpill");
            }
            else
            {
                var animator = objectToActivate.GetComponent<Animator>();
                if (animator != null)
                    animator.SetTrigger("Spill");
            }
        }
    }
}
