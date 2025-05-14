using System.Collections;
using UnityEngine;

public enum PowerUpType
{
    ExpandPaddle,
    MultiBall,
    // añade otros cuando los implementes
}

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
    public float duration = 5f;
    public float paddleMultiplier = 1.5f;
    public GameObject ballPrefab;
    public int extraBalls = 2;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Paddle")) return;
        StartCoroutine(Activate(other.gameObject));
        Destroy(gameObject);
    }

    IEnumerator Activate(GameObject paddle)
    {
        switch (type)
        {
            case PowerUpType.ExpandPaddle:
                Vector3 original = paddle.transform.localScale;
                paddle.transform.localScale *= paddleMultiplier;
                yield return new WaitForSeconds(duration);
                paddle.transform.localScale = original;
                break;

            case PowerUpType.MultiBall:
                for (int i = 0; i < extraBalls; i++)
                    Instantiate(ballPrefab, paddle.transform.position + Vector3.up, Quaternion.identity);
                break;
        }
    }
}
