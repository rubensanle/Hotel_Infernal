using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Controla la pantalla de Game Over usando textos TMP y una imagen de fondo
public class GameOverUITMP : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;   // Texto principal de "Game Over"
    public Image fondoNegro;               // Imagen oscura detrás del texto
    public TextMeshProUGUI reiniciarTexto; // Texto que indica cómo reinicia

    [Header("Audio de muerte")]
    public AudioClip[] sonidosMuerte;      // 🎵 ARRAY de sonidos de muerte
    private AudioSource audioSource;       // AudioSource interno

    private bool mostrarGameOver = false;  // Controla si el estado de Game Over está activo

    void Start()
    {
        // Ocultar todos los elementos al inicio
        gameOverText.enabled = false;
        fondoNegro.enabled = false;
        reiniciarTexto.enabled = false;

        // Crear o recuperar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D
        audioSource.ignoreListenerPause = true; // 🔥 Permite sonar aunque el juego esté pausado
    }

    void Update()
    {
        // Permitir reiniciar la escena solo si Game Over ya está mostrado
        if (mostrarGameOver && Input.GetKeyDown(KeyCode.U))
        {
            Time.timeScale = 1f;
            AudioListener.pause = false; // aseguramos que el audio vuelve
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameTaskManager taskManager = FindFirstObjectByType<GameTaskManager>();
            if (taskManager != null)
            {
                taskManager.ResetAllTasks();
            }

            // Reiniciar estados del GameManager para empezar desde cero
            if (GameManager.instancia != null)
            {
                GameManager.instancia.ResetGame();
            }

            SceneManager.LoadScene("PruebaDeMenuDeDia");
        }
    }

    // Activa la interfaz de Game Over
    public void ShowGameOverMessage()
    {
        // 🔊 Reproducir sonido de muerte aleatorio
        if (sonidosMuerte != null && sonidosMuerte.Length > 0)
        {
            int index = Random.Range(0, sonidosMuerte.Length);
            audioSource.PlayOneShot(sonidosMuerte[index]);
        }

        // Mostrar UI
        gameOverText.enabled = true;
        fondoNegro.enabled = true;
        reiniciarTexto.enabled = true;
        mostrarGameOver = true;

        // Pausar el juego
        Time.timeScale = 0f;

        // 🔇 Pausar TODO el audio excepto el sonido de muerte
        AudioListener.pause = true;
    }
}
