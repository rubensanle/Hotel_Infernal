using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    public GameObject juegoFinalizado;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (juegoFinalizado != null)
        {
            juegoFinalizado.SetActive(false);
        }
    }

    public void backToMainMenu()
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

        SceneManager.LoadScene("MainMenuControlador");
    }
}
