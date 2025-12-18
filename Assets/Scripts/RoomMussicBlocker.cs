using UnityEngine;

public class RoomMusicBlocker : MonoBehaviour
{
    [Header("Audio a controlar")]
    public AudioSource musicaGlobal;   // arrastra aquí el AudioSource de la música

    [Header("Configuración")]
    public string playerTag = "Player"; // el tag del jugador

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (musicaGlobal != null)
                musicaGlobal.mute = true; // silencia inmediatamente
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (musicaGlobal != null)
                musicaGlobal.mute = false; // vuelve a sonar inmediatamente
        }
    }
}
