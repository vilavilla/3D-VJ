using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AudioClip musicClip;

    void Start()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = musicClip;
        source.loop = true;
        source.playOnAwake = false;
        source.volume = 0.3f;
        source.Play();
    }
}
