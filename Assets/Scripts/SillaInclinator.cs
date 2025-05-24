using UnityEngine;

public class ChairLean : MonoBehaviour
{
    public float leanAngle = 10f;
    public float leanSpeed = 2f;

    private Quaternion originalRotation;
    private Quaternion leanedRotation;
    private bool leaning = false;

    void Start()
    {
        // ==== Opción A: Kinemática ====
        //      var rb = GetComponent<Rigidbody>();
        //      if (rb != null) { rb.useGravity = false; rb.isKinematic = true; }

        // ==== Opción B: Constraints ====        
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationY
                           | RigidbodyConstraints.FreezeRotationZ;
        }

        originalRotation = transform.localRotation;
        leanedRotation = Quaternion.Euler(leanAngle, 0f, 0f) * originalRotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            leaning = !leaning;

        Quaternion target = leaning ? leanedRotation : originalRotation;
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            target,
            Time.deltaTime * leanSpeed
        );
    }
}
