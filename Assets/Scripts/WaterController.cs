using UnityEngine;

public class BathtubFillController : MonoBehaviour
{
    [Header("Plano de agua (hijo de la ba�era)")]
    public Transform waterPlane;

    [Header("Velocidad de llenado (unidades de escala Y por segundo)")]
    public float fillSpeed = 0.2f;

    [Header("Escala Y m�xima (1 = lleno hasta la altura del plano original)")]
    public float maxScaleY = 1f;

    [Header("Objeto a activar cuando la ba�era se rompa")]
    public GameObject objectToActivate;

    private float currentScaleY = 0.1f;

    void Start()
    {
        // Empieza con un pel�n de agua (o 0 si prefieres)
        currentScaleY = 0.1f;
        ApplyWaterScale();
    }

    void Update()
    {
        // Llenado autom�tico
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
        // 1) Escalado local en Y (XZ fijos seg�n tu escala deseada)
        waterPlane.localScale = new Vector3(0.8f, currentScaleY, 1.7f);

        // 2) Compensa posici�n Y para que la base quede fija
        float yOffset = currentScaleY * 0.5f;
        waterPlane.localPosition = new Vector3(
            waterPlane.localPosition.x,
            yOffset,
            waterPlane.localPosition.z
        );
    }

    // Este m�todo se llama **justo antes** de que se destruya este GameObject
    void OnDestroy()
    {
        // 2) Reproduce la animaci�n �WaterSpill�
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
