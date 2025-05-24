using UnityEngine;

public class CarOrbit : MonoBehaviour
{
    [Tooltip("Punto en el espacio alrededor del que orbitar� el coche")]
    public Vector3 orbitCenter = Vector3.zero;

    [Tooltip("Velocidad angular en grados por segundo")]
    public float orbitSpeed = 90f;

    [Tooltip("Radio de la �rbita. Si es 0, se calcular� desde la posici�n inicial.")]
    public float orbitRadius = 0f;

    // �ngulo actual en radianes
    private float angle;

    // Control de si a�n orbita
    private bool isOrbiting = true;

    void Start()
    {
        Vector3 delta = transform.position - orbitCenter;
        Vector3 planar = new Vector3(delta.x, 0f, delta.z);
        if (orbitRadius <= 0f)
            orbitRadius = planar.magnitude;

        angle = Mathf.Atan2(planar.z, planar.x);
    }

    void Update()
    {
        if (!isOrbiting) return;

        // 1) Avanza el �ngulo (convertimos orbitSpeed de grados a rad/s)
        angle += orbitSpeed * Mathf.Deg2Rad * Time.deltaTime;

        // 2) Calcula la nueva posici�n sobre el plano XZ
        float x = orbitCenter.x + Mathf.Cos(angle) * orbitRadius;
        float z = orbitCenter.z + Mathf.Sin(angle) * orbitRadius;
        float y = transform.position.y;  // mantenemos la Y

        Vector3 newPos = new Vector3(x, y, z);
        transform.position = newPos;

        // 3) Calcula el vector tangente (direcci�n de movimiento)
        //    derivada de (cos, sin) -> (-sin, cos)
        Vector3 tangent = new Vector3(
            -Mathf.Sin(angle),
             0f,
             Mathf.Cos(angle)
        );

        // 4) Or�entalo para que el �frontal� (forward) siga esa tangente
        if (tangent.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            return;
        isOrbiting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
            return;
        isOrbiting = false;
    }
}
