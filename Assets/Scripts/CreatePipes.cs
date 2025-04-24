using UnityEngine;

public class CreatePipes : MonoBehaviour
{
    public GameObject pipe;
    public GameObject player;

    float lastPipe = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        createPipes();
    }

    // Update is called once per frame
    void Update()
    {
        createPipes();
    }

    void createPipes()
    {
        while(lastPipe < player.transform.position.x + 8.0f)
        {
            lastPipe += 8.0f;
            float rnd = 2f * Random.value - 1f;
            GameObject obj = GameObject.Instantiate(pipe, new Vector3(lastPipe, -2f + rnd, 0f), Quaternion.identity);
            obj.transform.parent = transform;
            obj = GameObject.Instantiate(pipe, new Vector3(lastPipe, 3f + rnd, 0f), Quaternion.AngleAxis(180f, Vector3.right));
            obj.transform.parent = transform;
        }
    }
}
