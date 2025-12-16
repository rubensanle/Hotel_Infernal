using UnityEngine;

// Controla un temporizador usado por el perro antes de comenzar la persecución
public class DogTimer : MonoBehaviour
{
    public float startTime = 60f;   // Tiempo inicial del temporizador
    public float extraTimeAlimentar = 30f; // Tiempo extra si se selecciona alimentar al perro
    private float currentTime;      // Tiempo restante que va disminuyendo
    public bool isChasing = false;  // Indica si el perro está persiguiendo al jugador
    public bool hasExpired = false; // Indica si el temporizador llegó a cero

    // Delegado y evento que avisa cuando el temporizador se termina
    public delegate void TimerExpiredHandler();
    public event TimerExpiredHandler OnTimerExpired;

    void Start()
    {
        // Determinar tiempo inicial según dificultad
        if (GameManager.instancia != null)
        {
            switch (GameManager.instancia.dificultadSeleccionada)
            {
                case 0: // Fácil - 2 minutos
                    startTime = 120f;
                    break;
                case 1: // Medio - 1 minuto y medio
                    startTime = 90f;
                    break;
                case 2: // Difícil - 1 minuto (60f según tu código original)
                    startTime = 60f;
                    break;
            }
        }

        currentTime = startTime;

        // Si se seleccionó alimentar al perro → +30s
        if (GameManager.instancia != null && GameManager.instancia.perroAlimentado)
        {
            startTime += extraTimeAlimentar;
            currentTime = startTime;
        }
    }


    void Update()
    {
        // Si el temporizador aún no ha expirado, sigue contando
        if (!hasExpired)
        {
            // Restar tiempo en función de los segundos reales
            currentTime -= Time.deltaTime;

            // Comprobar si llegó a cero
            if (currentTime <= 0f)
            {
                hasExpired = true;            // Marcar como expirado
                OnTimerExpired?.Invoke();     // Lanzar evento si alguien está suscrito
            }
        }
    }

    // Reinicia el temporizador desde cero (usado cuando el jugador interactúa a tiempo)
    public void RestartTimer()
    {
        currentTime = startTime;  // Restaurar valor inicial
        hasExpired = false;       // Ya no está expirado
        isChasing = false;        // Asegura que el perro no esté persiguiendo tras reiniciar
    }

    // Retorna el tiempo restante, sin bajar de 0
    public float GetTimeRemaining()
    {
        return Mathf.Max(currentTime, 0f);
    }
}
