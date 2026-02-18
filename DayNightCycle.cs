using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sunLight;
    public float dayLengthInMinutes = 20f; // 20 минут = день в Minecraft
    
    [Range(0, 1)]
    public float timeOfDay = 0.5f; // 0 = полночь, 0.5 = полдень
    
    public AnimationCurve sunIntensity;
    public Color dayAmbient = new Color(0.5f, 0.5f, 0.5f);
    public Color nightAmbient = new Color(0.1f, 0.1f, 0.2f);
    
    void Update()
    {
        // Обновляем время
        timeOfDay += Time.deltaTime / (dayLengthInMinutes * 60);
        if (timeOfDay > 1) timeOfDay = 0;
        
        // Вращаем солнце
        float sunRotation = timeOfDay * 360f - 90f;
        sunLight.transform.rotation = Quaternion.Euler(sunRotation, 0, 0);
        
        // Изменяем интенсивность
        sunLight.intensity = sunIntensity.Evaluate(timeOfDay);
        
        // Изменяем окружающий свет
        float blend = Mathf.Sin(timeOfDay * Mathf.PI * 2) * 0.5f + 0.5f;
        RenderSettings.ambientLight = Color.Lerp(nightAmbient, dayAmbient, blend);
        
        // Включаем/выключаем луну и звезды (если есть)
        bool isNight = timeOfDay < 0.2f || timeOfDay > 0.8f;
        // Включаем луну когда ночь
    }
}
