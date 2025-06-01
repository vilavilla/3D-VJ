using UnityEngine;
using TMPro;

public class GodMode : MonoBehaviour
{
    // 1) Variable estática que todos los demás scripts pueden consultar
    public static bool godmodeActivo = false;

    [Header("Asigna aquí tu pared invisible (o collider protector)")]
    [Tooltip("Al activar GodMode, este GameObject se habilita/deshabilita.")]
    public GameObject paredInvisible;

    [Header("Asigna aquí el TextMeshPro para mostrar 'GODMODE: ON / OFF'")]
    public TMP_Text textoGodMode_TMP;

    // 2) Evento opcional para que otros scripts se suscriban si quieren reaccionar al cambio
    public delegate void GodModeChangeHandler(bool activo);
    public static event GodModeChangeHandler OnGodModeChanged;

    private void Start()
    {
        // Inicializamos el texto en pantalla y la pared invisible
        if (textoGodMode_TMP != null)
            textoGodMode_TMP.text = "GODMODE: OFF";

        if (paredInvisible != null)
            paredInvisible.SetActive(false);
    }

    private void Update()
    {
        // Al presionar G, invertimos el estado de GodMode
        if (Input.GetKeyDown(KeyCode.G))
        {
            godmodeActivo = !godmodeActivo;

            // 3) Activamos o desactivamos la pared invisible según el nuevo estado
            if (paredInvisible != null)
                paredInvisible.SetActive(godmodeActivo);

            // 4) Actualizamos el texto TMP en pantalla
            if (textoGodMode_TMP != null)
                textoGodMode_TMP.text = "GODMODE: " + (godmodeActivo ? "ON" : "OFF");

            // 5) Disparamos el evento para notificar a cualquier suscriptor
            OnGodModeChanged?.Invoke(godmodeActivo);
        }
    }
}
