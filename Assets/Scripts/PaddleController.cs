using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 10f; // Velocidad de movimiento
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float input = Input.GetAxis("Horizontal"); // Flechas izquierda/derecha o A/D
        transform.Translate(Vector3.right * input * speed * Time.deltaTime);
    }
}
