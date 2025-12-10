using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Controla la pantalla de Game Over usando textos TMP y una imagen de fondo
public class GameOverUITMP : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;   // Texto principal de "Game Over"
    public Image fondoNegro;               // Imagen oscura detrás del texto
    public TextMeshProUGUI reiniciarTexto; // Texto que indica cómo reiniciar

    private bool mostrarGameOver = false;  // Controla si el estado de Game Over está activo

    void Start()
    {
        // Ocultar todos los elementos al inicio
        gameOverText.enabled = false;
        fondoNegro.enabled = false;
        reiniciarTexto.enabled = false;
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
        gameOverText.enabled = true;   // Muestra el texto principal
        fondoNegro.enabled = true;     // Muestra el fondo negro
        reiniciarTexto.enabled = true; // Muestra el mensaje de reinicio
        mostrarGameOver = true;        // Cambia el estado

        Time.timeScale = 0f;           // Pausa el juego completo
    }
}
