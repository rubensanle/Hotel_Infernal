using UnityEngine;
using UnityEngine.UI;

public class DogTimerUI : MonoBehaviour
{
    public DogTimer timerScript;
    public Text timerText;

    void Update()
    {
        float timeLeft = timerScript.GetTimeRemaining();
        timerText.text = $"Tiempo restante: {timeLeft:F1}s";
    }
}
