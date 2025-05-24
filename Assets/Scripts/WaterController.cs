using UnityEngine;

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

    private float currentScaleY = 0.1f;

    void Start()
    {
        // Empieza con un pelín de agua (o 0 si prefieres)
        currentScaleY = 0.1f;
        ApplyWaterScale();
    }

    void Update()
    {
        // Llenado automático
        if (currentScaleY < maxScaleY)
        {
            currentScaleY += fillSpeed * Time.deltaTime;
            if (currentScaleY > maxScaleY)
                currentScaleY = maxScaleY;
            ApplyWaterScale();
        }
    }

    void ApplyWaterScale()
    {
        // 1) Escalado local en Y (XZ fijos según tu escala deseada)
        waterPlane.localScale = new Vector3(0.8f, currentScaleY, 1.7f);

        // 2) Compensa posición Y para que la base quede fija
        float yOffset = currentScaleY * 0.5f;
        waterPlane.localPosition = new Vector3(
            waterPlane.localPosition.x,
            yOffset,
            waterPlane.localPosition.z
        );
    }

    // Este método se llama **justo antes** de que se destruya este GameObject
    void OnDestroy()
    {
        // 2) Reproduce la animación “WaterSpill”
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
    

        if (objectToActivate != null)
            objectToActivate.SetActive(true);
    }
}
