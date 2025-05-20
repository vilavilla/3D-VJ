using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    [Tooltip("RectTransform del Text que contiene los créditos")]
    public RectTransform creditsTextRect;
    [Tooltip("Velocidad de scroll en unidades UI por segundo")]
    public float scrollSpeed = 50f;

    void Update()
    {
        if (creditsTextRect == null) return;

        // Mover hacia abajo (negative Y)
        creditsTextRect.anchoredPosition -= Vector2.down * scrollSpeed * Time.deltaTime;
    }
}
