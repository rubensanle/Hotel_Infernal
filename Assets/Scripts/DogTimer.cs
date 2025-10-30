using UnityEngine;

public class DogTimer : MonoBehaviour
{
    public float countdownTime = 5f;
    private float timer;
    public bool isChasing = false;

    public delegate void TimerExpired();
    public event TimerExpired OnTimerExpired;

    void Start()
    {
        timer = countdownTime;
    }

    void Update()
    {
        if (!isChasing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isChasing = true;
                OnTimerExpired?.Invoke();
            }
        }
    }

    public void ResetTimer()
    {
        timer = countdownTime;
        isChasing = false;
    }

    public float GetTimeRemaining()
    {
        return Mathf.Max(0f, timer);
    }
}
