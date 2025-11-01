using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Text gameOverText;
    public Image fondoNegro;

    public void ShowGameOverMessage()
    {
        gameOverText.enabled = true;
        fondoNegro.enabled = true;
        Time.timeScale = 0f; 
    }
}


