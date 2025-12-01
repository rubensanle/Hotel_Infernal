using UnityEngine;
using System.Collections;

public class DemonioSonidos : MonoBehaviour
{
    [Header("Sonidos del demonio")]
    public AudioClip[] clips;          // Array con 3 sonidos
    public AudioSource audioSource;    // AudioSource del demonio

    [Header("Configuración")]
    public float intervaloMin = 5f;    // Tiempo mínimo entre sonidos
    public float intervaloMax = 12f;   // Tiempo máximo entre sonidos

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (clips.Length > 0 && audioSource != null)
            StartCoroutine(ReproducirSonidosAleatorios());
    }

    IEnumerator ReproducirSonidosAleatorios()
    {
        while (true)
        {
            // Esperar un tiempo aleatorio entre min y max
            float espera = Random.Range(intervaloMin, intervaloMax);
            yield return new WaitForSeconds(espera);

            // Elegir un clip aleatorio
            int indice = Random.Range(0, clips.Length);
            AudioClip clip = clips[indice];

            // Reproducir
            audioSource.PlayOneShot(clip);
            Debug.Log($"?? Demonio reproduce sonido: {clip.name}");
        }
    }
}

