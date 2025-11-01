using UnityEngine;

public class InteractionDemon : MonoBehaviour
{
    public float tiempoLimite = 60f;
    public float tiempoFinal = 90f;
    public DemonBehaviour demonio;
    public Renderer cuboRenderer;
    public Camera camaraJugador;
    public float distanciaInteraccion = 2f;

    private float tiempoActual = 0f;
    private bool faseFinal = false;
    private bool jugadorCerca = false;
    private bool cronometroDetenido = false;

    void Start()
    {
        tiempoActual = 0f;
        cuboRenderer.material.color = Color.white;

        if (camaraJugador == null)
            Debug.LogError("Asigna la cámara del jugador en el inspector.");
    }

    void Update()
    {
        // Detectar si el jugador está mirando al cubo y está cerca
        RaycastHit hit;
        jugadorCerca = false;

        if (Physics.Raycast(camaraJugador.transform.position, camaraJugador.transform.forward, out hit, distanciaInteraccion))
        {
            if (hit.collider.gameObject == gameObject)
            {
                jugadorCerca = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    cuboRenderer.material.color = Color.green;

                    if (!faseFinal)
                    {
                        cronometroDetenido = true;
                        demonio.Calmar();
                        Debug.Log("Cubo tocado: demonio calmado y cronómetro detenido.");
                    }
                    else
                    {
                        Debug.Log("Cubo tocado, pero el demonio ya no se calma.");
                    }
                }
            }
        }

        // Temporizador demonio
        if (!cronometroDetenido && !faseFinal)
        {
            tiempoActual += Time.deltaTime;

            if (tiempoActual >= tiempoLimite)
            {
                demonio.Enfadar();
            }

            if (tiempoActual >= tiempoFinal)
            {
                faseFinal = true;
                demonio.Teletransportarse();
            }
        }
    }

    void OnGUI()
    {
        if (jugadorCerca)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            Rect rect = new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 50);
            GUI.Label(rect, "Pulsa E para interactuar", style);
        }

        if (!cronometroDetenido)
        {
            GUIStyle timerStyle = new GUIStyle();
            timerStyle.fontSize = 16;
            timerStyle.normal.textColor = (tiempoActual < tiempoFinal) ? Color.white : Color.red;
            GUI.Label(new Rect(10, 10, 250, 30), "Tiempo: " + Mathf.Ceil(tiempoFinal - tiempoActual).ToString() + "s", timerStyle);
        }
    }

    public bool EstaEnFaseFinal()
    {
        return faseFinal;
    }
}



