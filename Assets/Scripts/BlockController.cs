using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("Resistencia del bloque")]
    [Tooltip("Número de impactos que resiste antes de destruirse")]
    public int vidas = 1;

    [Header("Modo visual")]
    public bool useCutoutMode = false;      // true = Cutout, false = Brightness
    [Tooltip("Color base del bloque (para Brightness mode)")]
    public Color originalColor = Color.white;
    [Tooltip("Color de daño máximo (para Brightness mode)")]
    public Color damagedColor = Color.gray;

    private int maxVidas;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        maxVidas = vidas;
        // Si es modo Brightness, captura el color inicial
        if (!useCutoutMode)
            originalColor = rend.material.color;
        ActualizarVisual();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;

        vidas--;

        if (vidas <= 0)
        {
            MakeBlocksAboveFall();
            Destroy(gameObject);
        }
        else
        {
            ActualizarVisual();
        }
    }

    void ActualizarVisual()
    {
        float t = Mathf.Clamp01((float)vidas / maxVidas);

        if (useCutoutMode)
        {
            // Cutout shader: _Cutoff de 1-t
            // Asegúrate de que el material está en Rendering Mode = Cutout
            rend.material.SetFloat("_Cutoff", 1f - t);
        }
        else
        {
            // Brightness mode: lerp entre damagedColor y originalColor
            Color c = Color.Lerp(damagedColor, originalColor, t);
            rend.material.color = c;
        }
    }

    void MakeBlocksAboveFall()
    {
        float rayLength = 5f;
        Ray ray = new Ray(transform.position + Vector3.up * 0.01f, Vector3.up);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayLength);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Block"))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb == null) rb = hit.collider.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }
    }
}
