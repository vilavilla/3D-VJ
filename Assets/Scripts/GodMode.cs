using UnityEngine;

public class GodMode : MonoBehaviour
{
    public GameObject paredInvisible;

    private bool godmodeActivo = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            godmodeActivo = !godmodeActivo;
            paredInvisible.SetActive(godmodeActivo);
        }
    }
}
