using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MrWorlwideScript : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject imagenMostrar;              // Imagen que quieres mostrar
    public AudioSource audioSource;               // AudioSource para el sonido (OPCIONAL)

    [Header("Configuración Sonido Mejorada")]
    public AudioClip clipSonido;                  // ?? NUEVO: Clip de audio directo
    public float volumen = 1f;                    // ?? NUEVO: Volumen del sonido

    [Header("Configuración")]
    public float duracion = 1f;                   // Duración en segundos

    private Button boton;

    public void Start()
    {
        // Obtener el componente Button
        boton = GetComponent<Button>();

        // Asignar el evento onClick
        if (boton != null)
        {
            boton.onClick.AddListener(ActivarImagenSonido);
        }

        // Asegurarse de que la imagen está oculta al inicio
        if (imagenMostrar != null)
        {
            imagenMostrar.SetActive(false);
        }

        // ?? NUEVO: Buscar AudioSource automáticamente si no está asignado
        if (audioSource == null)
        {
            audioSource = FindFirstObjectByType<AudioSource>();
            if (audioSource != null)
            {
                Debug.Log("? AudioSource encontrado automáticamente");
            }
        }
    }

    public void ActivarImagenSonido()
    {
        StartCoroutine(MostrarImagenYSonido());
    }

    IEnumerator MostrarImagenYSonido()
    {
        // Mostrar la imagen
        if (imagenMostrar != null)
        {
            imagenMostrar.SetActive(true);
        }

        // ?? MEJORADO: Reproducir sonido (múltiples métodos)
        ReproducirSonido();

        Debug.Log("??? Imagen y sonido activados");

        // Esperar la duración especificada
        yield return new WaitForSeconds(duracion);

        // Ocultar la imagen
        if (imagenMostrar != null)
        {
            imagenMostrar.SetActive(false);
        }

        Debug.Log("??? Imagen ocultada después de " + duracion + " segundos");
    }

    // ?? NUEVO: Método mejorado para reproducir sonido
    private void ReproducirSonido()
    {
        // PRIMERO: Intentar con el clip directo (siempre funciona)
        if (clipSonido != null)
        {
            AudioSource.PlayClipAtPoint(clipSonido, Camera.main.transform.position, volumen);
            Debug.Log("?? Sonido reproducido con clip directo");
            return;
        }

        // SEGUNDO: Intentar con el AudioSource asignado
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            Debug.Log("?? Sonido reproducido con AudioSource");
            return;
        }

        // TERCERO: Buscar cualquier AudioSource en la escena
        AudioSource audioDeEmergencia = FindFirstObjectByType<AudioSource>();
        if (audioDeEmergencia != null && audioDeEmergencia.clip != null)
        {
            audioDeEmergencia.Play();
            Debug.Log("?? Sonido reproducido con AudioSource de emergencia");
            return;
        }

        Debug.LogWarning("?? No se pudo reproducir sonido - ningún método disponible");
    }

    // Opcional: Método para cambiar la duración desde otros scripts
    public void CambiarDuracion(float nuevaDuracion)
    {
        duracion = nuevaDuracion;
    }

    // ?? NUEVO: Método para asignar un nuevo clip de audio
    public void AsignarNuevoSonido(AudioClip nuevoClip)
    {
        clipSonido = nuevoClip;
        Debug.Log("?? Nuevo sonido asignado: " + nuevoClip.name);
    }
}
