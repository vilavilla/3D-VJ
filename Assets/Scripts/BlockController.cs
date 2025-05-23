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

    [Header("Power-Up Settings")]
    [Tooltip("Prefabs de power-ups que puede soltar este bloque")]
    public GameObject[] powerUpPrefabs;
    [Range(0f, 1f), Tooltip("Probabilidad de soltar un power-up al romperse")]
    public float spawnChance = 1.0f;

    void Awake()
    {
        // Carga los power-ups si no están asignados en el Inspector
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0)
            powerUpPrefabs = Resources.LoadAll<GameObject>("PowerUps");
    }

    void Start()
    {
        // Inicializa render y resistencia
        rend = GetComponent<Renderer>();
        maxVidas = vidas;
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

            // Notifica al LevelManager que un bloque ha sido destruido
            FindObjectOfType<LevelManager>()?.BloqueDestruido();

            Destroy(gameObject);

            // Spawn de power-up si corresponde
            if (powerUpPrefabs.Length > 0 && Random.value < spawnChance)
            {
                int idx = Random.Range(0, powerUpPrefabs.Length);
                Vector3 spawnPos = new Vector3(
                    transform.position.x,
                    -3.5f,
                    transform.position.z
                );
                Instantiate(powerUpPrefabs[idx], spawnPos, Quaternion.identity);
            }

            GivePoints();
        }
        else
        {
            ActualizarVisual();
        }
    }

    void GivePoints()
    {
        int points = maxVidas * 100;
        ScoreManager.Instance.AddPoints(points);
    }

    void ActualizarVisual()
    {
        float t = Mathf.Clamp01((float)vidas / maxVidas);
        if (useCutoutMode)
            rend.material.SetFloat("_Cutoff", 1f - t);
        else
            rend.material.color = Color.Lerp(damagedColor, originalColor, t);
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
