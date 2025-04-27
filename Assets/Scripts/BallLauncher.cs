using UnityEngine;

public class Ball : MonoBehaviour
{
    public float launchSpeed = 10f;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 launchDirection = new Vector3(Random.Range(-0.5f, 0.5f), 0f, 1f).normalized;
        rb.linearVelocity = launchDirection * launchSpeed; // Updated to use linearVelocity
    }

    // Update is called once per frame
    void Update()
    {

    }
}

