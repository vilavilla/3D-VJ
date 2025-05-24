using UnityEngine;

public class LampAutoFade : MonoBehaviour
{
    public Light lampLight;

    [Header("Intensidad")]
    public float maxIntensity = 3f;
    public float minIntensity = 0f;
    public float fadeSpeed = 2f;

    [Header("Tiempo entre cambios")]
    public float minBlinkTime = 1f;
    public float maxBlinkTime = 5f;

    private bool targetOn = true;
    private float nextBlinkTime = 0f;
    private float timer = 0f;

    void Start()
    {
        ScheduleNextBlink();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Cambio de estado tras el tiempo aleatorio
        if (timer >= nextBlinkTime)
        {
            targetOn = !targetOn;
            ScheduleNextBlink();
        }

        // Transición suave
        if (lampLight != null)
        {
            float targetIntensity = targetOn ? maxIntensity : minIntensity;
            lampLight.intensity = Mathf.MoveTowards(
                lampLight.intensity,
                targetIntensity,
                fadeSpeed * Time.deltaTime
            );
        }
    }

    void ScheduleNextBlink()
    {
        timer = 0f;
        nextBlinkTime = Random.Range(minBlinkTime, maxBlinkTime);
    }
}
