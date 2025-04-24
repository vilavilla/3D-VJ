using UnityEngine;

public class MoveBird : MonoBehaviour
{
    public float speed;
    public float secondsBetweenFlaps = 0.1f;
    public float maxSpeedY = 10f;
    public AudioClip flapSound;

    float secondsSinceLastFlap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondsSinceLastFlap = secondsBetweenFlaps;
        gameObject.GetComponent<Rigidbody>().linearVelocity = new Vector3(speed, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 speed = gameObject.GetComponent<Rigidbody>().linearVelocity;
        if(speed.y > maxSpeedY)
            speed.y = maxSpeedY;
        else if(speed.y < -maxSpeedY)
            speed.y = -maxSpeedY;
        GetComponent<Rigidbody>().linearVelocity = speed;
        secondsSinceLastFlap += Time.deltaTime;
        if(Input.GetKey(KeyCode.Space) && (secondsSinceLastFlap >= secondsBetweenFlaps))
        {
            AudioSource.PlayClipAtPoint(flapSound, transform.position + new Vector3(0f, 0f, -30f));
            secondsSinceLastFlap = 0f;
            GetComponent<Rigidbody>().AddForce(0f, 12f, 0f, ForceMode.Impulse);
        }
    }
}
