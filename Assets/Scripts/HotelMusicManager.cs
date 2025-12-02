using UnityEngine;

public class HotelMusicManager : MonoBehaviour
{
    [Header("Referencias de audio")]
    public AudioSource audioSource;        // El AudioSource que reproducirá la música
    public AudioClip musicaNormal;         // Música si NO arreglaste los altavoces
    public AudioClip musicaAltavoces;      // Música si SÍ arreglaste los altavoces

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Comprobar el estado guardado en GameManager
        if (GameManager.instancia != null && GameManager.instancia.altavocesArreglados)
        {
            audioSource.clip = musicaAltavoces;
        }
        else
        {
            audioSource.clip = musicaNormal;
        }

        audioSource.loop = true;   // Que la música se repita
        audioSource.Play();        // Reproducir al entrar en la escena
    }
}
