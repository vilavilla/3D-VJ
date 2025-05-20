using UnityEngine;

public class Reward : MonoBehaviour
{
    private Vector3 destino;
    private float speed = 3f;

    public void Init(Vector3 objetivo)
    {
        destino = objetivo;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destino, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paddle"))
        {
            FindObjectOfType<LevelManager>().CompletarNivel();
            Destroy(gameObject);
        }
    }
}
