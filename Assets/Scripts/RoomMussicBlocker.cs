using UnityEngine;

public class RoomMusicBlocker : MonoBehaviour
{
    [Header("Audio global del hotel")]
    public AudioSource musicaGlobal;

    [Header("Ambiente tenebroso (solo AudioClip)")]
    public AudioClip ambienteTenebrosoClip;

    [Header("Configuración")]
    public string playerTag = "Player";

    private AudioSource ambienteSource; // se crea dinámicamente

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Silenciar música global
        if (musicaGlobal != null)
            musicaGlobal.mute = true;

        // Crear AudioSource temporal para el ambiente tenebroso
        if (ambienteTenebrosoClip != null && ambienteSource == null)
        {
            ambienteSource = gameObject.AddComponent<AudioSource>();
            ambienteSource.clip = ambienteTenebrosoClip;
            ambienteSource.loop = true;
            ambienteSource.spatialBlend = 0f; // 2D (puedes poner 1f si quieres 3D)
            ambienteSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Reactivar música global
        if (musicaGlobal != null)
            musicaGlobal.mute = false;

        // Destruir el AudioSource temporal
        if (ambienteSource != null)
        {
            ambienteSource.Stop();
            Destroy(ambienteSource);
            ambienteSource = null;
        }
    }
}
