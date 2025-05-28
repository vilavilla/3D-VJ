using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BearJump : MonoBehaviour
{
    [Header("Fuerza de salto")]
    [Tooltip("Impulso vertical que se aplica al saltar")]
    public float jumpForce = 5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        Debug.Log("[BearJumpOnBed] Awake: Rigidbody configurado  useGravity=ON, isKinematic=OFF");
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log($"[BearJumpOnBed] OnCollisionEnter con '{other.gameObject.name}' (tag='{other.gameObject.tag}')");

        if (other.gameObject.CompareTag("Cama"))
        {
            Debug.Log("[BearJumpOnBed] �Es cama! Preparando salto�");

            // Reseteamos velocidad vertical
            Vector3 vAntes = rb.linearVelocity;
            rb.linearVelocity = new Vector3(vAntes.x, 0f, vAntes.z);
            Debug.Log($"[BearJumpOnBed] Velocidad antes reset Y: {vAntes.y}, ahora: {rb.linearVelocity.y}");

            // Aplicamos impulso hacia arriba
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log($"[BearJumpOnBed] Saltando con fuerza: {jumpForce}");
        }
        else
        {
            Debug.Log("[BearJumpOnBed] No es cama, no salto.");
        }
    }

    void OnCollisionExit(Collision other)
    {
        Debug.Log($"[BearJumpOnBed] OnCollisionExit de '{other.gameObject.name}' (tag='{other.gameObject.tag}')");
    }
}
