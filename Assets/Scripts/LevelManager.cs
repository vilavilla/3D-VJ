using UnityEngine;
public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject[] niveles; // Array para meter tus niveles
    private void Start()
    {
        ActivarNivel(0); // Activa el primer nivel al inicio
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivarNivel(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivarNivel(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivarNivel(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ActivarNivel(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ActivarNivel(4);
    }

    void ActivarNivel(int index)
    {
        for (int i = 0; i < niveles.Length; i++)
        {
            niveles[i].SetActive(i == index); // Solo activa el nivel seleccionado
        }
    }
}