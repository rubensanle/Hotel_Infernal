using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public AudioSource audioSource;       // AudioSource del jugador
    public AudioClip[] gruñidos;          // Array de gruñidos (3 clips)

    // Reproduce un gruñido aleatorio
    public void PlayGruñido()
    {
        if (gruñidos.Length > 0 && audioSource != null)
        {
            int index = Random.Range(0, gruñidos.Length);
            audioSource.PlayOneShot(gruñidos[index]);
        }
    }
}
