using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float timeToDestroy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy < 0f)
            Destroy(gameObject);
    }
}
