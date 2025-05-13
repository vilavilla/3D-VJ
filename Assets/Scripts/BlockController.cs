using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("Resistencia del bloque")]
    [Tooltip("Número de impactos que resiste antes de destruirse")]
    public int vidas = 1;

    private int maxVidas;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        maxVidas = vidas;
        ActualizarTransparencia();
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
            ActualizarTransparencia();
        }
    }

    void ActualizarTransparencia()
    {
        // Calcula alfa en [0,1] según porcentaje de vida restante
        float alpha = Mathf.Clamp01((float)vidas / maxVidas);

        // Obtén el color actual (RGB) y sustituye el A
        Color c = rend.material.color;
        c.a = alpha;
        rend.material.color = c;
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
