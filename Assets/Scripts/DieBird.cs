using UnityEngine;
using UnityEngine.SceneManagement;

public class DieBird : MonoBehaviour
{
    public AudioClip hitSound;
    public GameObject reloader;


    private void OnCollisionEnter(Collision collision)
    {
        AudioSource.PlayClipAtPoint(hitSound, transform.position + new Vector3(0f, 0f, -30f));
        Instantiate(reloader);

    }
}
