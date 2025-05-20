using UnityEngine;

public class DestroyIfOutOfBounds : MonoBehaviour
{
    public float limiteY = -5.0f;

    void Update()
    {
        if (transform.position.y < limiteY)
        {
            Destroy(gameObject);
        }
    }
}
