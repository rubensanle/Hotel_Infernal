using UnityEngine;

public class DogTimer : MonoBehaviour
{
    public float startTime = 10f;
    private float currentTime;
    public bool isChasing = false;
    public bool hasExpired = false;

    public delegate void TimerExpiredHandler();
    public event TimerExpiredHandler OnTimerExpired;

    void Start()
    {
        currentTime = startTime;
    }

    void Update()
    {
        if (!hasExpired)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                hasExpired = true;
                OnTimerExpired?.Invoke();
            }
        }
    }

    public void RestartTimer()
    {
        currentTime = startTime;
        hasExpired = false;
        isChasing = false;
    }

    public float GetTimeRemaining()
    {
        return Mathf.Max(currentTime, 0f);
    }
}

