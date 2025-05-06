using UnityEngine;
public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject[] blockPrefabs;
    public int rows = 5;
    public int columns = 8;
    public float horizontalSpacing = 10f;

    // Ajustes específicos para tu escena:
    public float floorY = -3f;         // Altura del centro del suelo
    public float floorThickness = -1f;  // Su altura total (scale.y)
    public float blockHeight = 2f;     // Altura total de tu block prefab (scale.y)
    public float verticalSpacing = 2f; // Espacio extra entre filas

    private int[,] levelMap = new int[,]
    {
        {0,1,1,1,1,1,1,0},
        {1,0,1,1,1,1,0,1},
        {1,1,0,1,1,0,1,1},
        {1,1,1,0,0,1,1,1},
        {1,1,1,1,1,1,1,1}
    };

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Calcula la Y de la primera fila de bloques:
        float startY = floorY + floorThickness * 0.5f + blockHeight * 0.5f;

        for (int row = 0; row < levelMap.GetLength(0); row++)
        {
            for (int col = 0; col < levelMap.GetLength(1); col++)
            {
                if (levelMap[row, col] == 1)
                {
                    // Elige prefab aleatorio
                    GameObject prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                    // Calcula posición
                    float x = col * horizontalSpacing - (columns / 2f * horizontalSpacing);
                    float y = startY + row * verticalSpacing;
                    float z = 0f; // ajusta si quieres adelantar/atrasar

                    GameObject block = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
                    // Asegúrate de que no caiga al inicio

                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}