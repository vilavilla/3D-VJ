using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 scaleOnHover = new Vector3(1.1f, 1.1f, 1f);
    Vector3 originalScale;

    void Start() => originalScale = transform.localScale;

    public void OnPointerEnter(PointerEventData eventData) =>
        transform.localScale = scaleOnHover;

    public void OnPointerExit(PointerEventData eventData) =>
        transform.localScale = originalScale;
}
