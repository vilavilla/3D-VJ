using UnityEngine;
using UnityEngine.SceneManagement;

public class TimedReload : MonoBehaviour
{
    public float timeToReload = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToReload -= Time.deltaTime;
        if (timeToReload < 0f)
            SceneManager.LoadScene("SampleScene");
    }
}
