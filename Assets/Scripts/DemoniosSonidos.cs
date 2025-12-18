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
    public float maxDuracionClip = 5f; // ? Máximo tiempo de reproducción por clip

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

            // Reproducir clip y cortar a los 5 segundos
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log($"?? Demonio reproduce sonido: {clip.name}");

            // Esperar máximo 5 segundos o menos si el clip dura menos
            yield return new WaitForSeconds(Mathf.Min(maxDuracionClip, clip.length));

            // Parar el sonido
            audioSource.Stop();
        }
    }
}


