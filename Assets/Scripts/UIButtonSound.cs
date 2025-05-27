using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip hoverSound;
    private AudioSource audioSource;

    void Start()
    {
        // Busca el AudioManager que contiene el AudioSource
        audioSource = GameObject.Find("AudioManagerUI").GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);
    }
}
