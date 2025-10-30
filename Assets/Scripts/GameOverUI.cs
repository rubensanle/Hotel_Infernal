using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public Text gameOverText; // Asigna el Text desde el inspector

    public void ShowGameOverMessage()
    {
        StartCoroutine(ShowAndHide());
    }

    private IEnumerator ShowAndHide()
    {
        gameOverText.enabled = true;
        yield return new WaitForSeconds(3f);
        gameOverText.enabled = false;
    }
}

