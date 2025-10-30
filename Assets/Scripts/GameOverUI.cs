using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Text gameOverText;

    public void ShowGameOverMessage()
    {
        gameOverText.text = "Has perdido";
        gameOverText.enabled = true;
    }
}
