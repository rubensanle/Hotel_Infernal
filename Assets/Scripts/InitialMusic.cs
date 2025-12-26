using UnityEngine;

public class InitialMusic : MonoBehaviour
{
    public AudioClip musicaDeFondo;   // Canción o sonido
    private AudioSource audioSource;  // AudioSource interno

    void Start()
    {
        // Crear o recuperar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Configuración del AudioSource
        audioSource.clip = musicaDeFondo;
        audioSource.loop = true;          
        audioSource.playOnAwake = false;  // No sonar automáticamente
        audioSource.spatialBlend = 0f;    // 2D
        audioSource.volume = 0.3f;
        // Reproducir música
        audioSource.Play();
    }

    public void SetVolume(float value) { 
        audioSource.volume = value; 
    }

    // Llamado desde el botón ANTES de cambiar de escena
    public void DetenerMusica()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}

