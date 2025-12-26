using UnityEngine;
using UnityEngine.UI;

public class MenuVolumeSlider : MonoBehaviour
{
    public Slider volumenSlider;
    public InitialMusic musicController;

    void Start()
    {
        // Iniciar el slider en el mismo valor que la música
        volumenSlider.value = 0.3f;

        // Suscribir el slider al método que cambia el volumen
        volumenSlider.onValueChanged.AddListener(ChangeVolume);
    }

    void ChangeVolume(float value)
    {
        if (musicController != null)
            musicController.SetVolume(value);
    }
}

