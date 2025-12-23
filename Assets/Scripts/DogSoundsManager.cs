using UnityEngine;

public class DogSoundManager : MonoBehaviour
{
    [Header("Sonidos del perro (NO aleatorios)")]
    public AudioClip sonidoNoInteractuado; // cuando NO interactuamos
    public AudioClip sonidoInteractuado;   // cuando SÍ interactuamos

    private AudioSource audioSource;

    void Awake()
    {
        // Obtener o crear AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D para que siempre se oiga
    }

    public void PlayNoInteractSound()
    {
        if (sonidoNoInteractuado != null)
            audioSource.PlayOneShot(sonidoNoInteractuado);
    }

    public void PlayInteractSound()
    {
        if (sonidoInteractuado != null)
            audioSource.PlayOneShot(sonidoInteractuado);
    }
}
