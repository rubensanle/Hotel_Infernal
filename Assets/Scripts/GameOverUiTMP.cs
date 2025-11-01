using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUITMP : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    public Image fondoNegro;
    public TextMeshProUGUI reiniciarTexto;

    private bool mostrarGameOver = false;

    void Start()
    {
        gameOverText.enabled = false;
        fondoNegro.enabled = false;
        reiniciarTexto.enabled = false;
    }

    void Update()
    {
        if (mostrarGameOver && Input.GetKeyDown(KeyCode.U))
        {
            Time.timeScale = 0f; 
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ShowGameOverMessage()
    {
        gameOverText.enabled = true;
        fondoNegro.enabled = true;
        reiniciarTexto.enabled = true;
        mostrarGameOver = true;
        Time.timeScale = 0f;
    }
}


