using UnityEngine;
using TMPro;

public class RelojManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text textoReloj;       // Texto donde se muestra la hora

    [Header("Configuración")]
    public float duracionEscena = 480f; // 8 minutos en segundos
    public int horaInicial = 0;         // Hora inicial del reloj (00:00)

    private float tiempoTranscurrido = 0f;

    void Update()
    {
        // Avanzar tiempo real
        tiempoTranscurrido += Time.deltaTime;

        // Proporción del tiempo transcurrido (0 ? 1)
        float proporcion = tiempoTranscurrido / duracionEscena;

        // Calcular horas avanzadas (0 a 8)
        int horasAvanzadas = Mathf.FloorToInt(proporcion * 8);

        // Hora actual
        int horaActual = (horaInicial + horasAvanzadas) % 24;

        // Mostrar en formato HH:00
        if (textoReloj != null)
            textoReloj.text = horaActual.ToString("00") + ":00";
    }
}

