using UnityEngine;

// Maneja los ajustes globales del juego como sensibilidad del mouse y volumen, y persiste los datos entre escenas
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;  // Singleton para acceder desde cualquier script

    public float sensibilidad = 120f;        // Valor de sensibilidad por defecto
    public float volumen = 1f;               // Valor de volumen por defecto

    void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener este objeto al cambiar de escena

            // Cargar valores guardados previamente o usar valores por defecto
            sensibilidad = PlayerPrefs.GetFloat("Sensibilidad", 120f);
            volumen = PlayerPrefs.GetFloat("Volumen", 1f);

            // 🔊 Aplica el volumen global al iniciar
            AudioListener.volume = volumen;
        }
        else
        {
            Destroy(gameObject); // Evitar duplicados
        }
    }

    // Establece la sensibilidad y la guarda en PlayerPrefs
    public void SetSensibilidad(float value)
    {
        sensibilidad = value;
        PlayerPrefs.SetFloat("Sensibilidad", value);
        PlayerPrefs.Save();
        // ✅ No se toca nada más aquí
    }

    // Establece el volumen y lo guarda en PlayerPrefs
    public void SetVolumen(float value)
    {
        volumen = value;
        PlayerPrefs.SetFloat("Volumen", value);
        PlayerPrefs.Save();

        // 🔊 Aplica el volumen global a todo el juego
        AudioListener.volume = volumen;
    }
}

