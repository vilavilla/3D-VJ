using UnityEngine;

public class BlockController : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Destroy(gameObject); // Se destruye al tocar la bola
        }
    }
    void MakeBlocksAboveFall()
    {
        float rayLength = 5f; // distancia para buscar bloques encima
        Ray ray = new Ray(transform.position, Vector3.up); // lanza un rayo hacia arriba
        RaycastHit[] hits = Physics.RaycastAll(ray, rayLength);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Block"))
            {
                Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = hit.collider.gameObject.AddComponent<Rigidbody>();
                }
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
