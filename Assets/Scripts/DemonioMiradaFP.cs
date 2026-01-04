using UnityEngine;
using UnityEngine.AI;
using System.Linq;

// Demonio que se mueve solo cuando el jugador NO lo está mirando
public class DemonioMiradaFP : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;          // Transform del jugador (posición a seguir)
    public Camera camaraJugador;       // Cámara en primera persona usada para detectar la mirada
    public GameOverUITMP interfazGameOver;

    [Header("Movimiento")]
    public float velocidadPersecucion = 2f;  // Velocidad lenta al perseguir
    public float distanciaMatar = 1.2f;      // Distancia de muerte si no se usa trigger

    [Header("Detección de mirada")]
    public float umbralDot = 0.7f;           // Sensibilidad del ángulo de visión (0.7 ≈ 45°)
    public float maxDistanciaVista = 100f;    // Máxima distancia en la que el jugador puede verlo
    public LayerMask mascaraObstaculos;      // Capas que bloquean la visión


    public Transform[] puntosMirada;
    private NavMeshAgent agente;

    void Start()
    {
        // Verificar si debemos desactivar este demonio en dificultad Fácil
        if (GameManager.instancia != null && GameManager.instancia.dificultadSeleccionada == 0)
        {
            // Desactivar completamente el GameObject del demonio
            gameObject.SetActive(false);
            return; // Salir del Start para no inicializar nada más
        }

        if (GameManager.instancia != null && GameManager.instancia.dificultadSeleccionada == 1)
        {
            velocidadPersecucion = 1f;
        }

        // Obtener NavMeshAgent y configurar movimiento inicial
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadPersecucion;
        agente.stoppingDistance = 0f;

        // Asigna cámara principal si no se asignó manualmente
        if (camaraJugador == null && Camera.main != null)
            camaraJugador = Camera.main;
    }

    void Update()
    {
        // Si el demonio está desactivado por dificultad Fácil, no hacer nada
        if (GameManager.instancia != null && GameManager.instancia.dificultadSeleccionada == 0)
            return;

        // Resto del código permanece igual...
        if (jugador == null || camaraJugador == null) return;

        bool meMira = JugadorMeMiraConLineaDeVista();

        if (meMira)
        {
            agente.isStopped = true;
            agente.velocity = Vector3.zero; // ← Frena en seco
        }
        else
        {
            agente.isStopped = false;
            agente.SetDestination(jugador.position);
        }

        if (!meMira && Vector3.Distance(transform.position, jugador.position) <= distanciaMatar)
        {
            ActivarGameOver();
        }
    }

    // Detecta si el jugador realmente está mirando al demonio con FOV + línea de visión
    bool JugadorMeMiraConLineaDeVista()
{
    foreach (Transform punto in puntosMirada)
    {
        Vector3 dirHaciaPunto = (punto.position - camaraJugador.transform.position).normalized;
        float dot = Vector3.Dot(camaraJugador.transform.forward, dirHaciaPunto);

        if (dot < umbralDot) continue; // fuera del ángulo

        float distancia = Vector3.Distance(camaraJugador.transform.position, punto.position);
        // if (distancia > maxDistanciaVista) continue; // demasiado lejos

        if (Physics.Raycast(
        camaraJugador.transform.position,
        dirHaciaPunto,
        out RaycastHit hit,
        distancia,
        ~0, // ← esto significa "todas las capas"
        QueryTriggerInteraction.Ignore))
{
    // Si lo primero que golpea NO es el demonio, la vista está bloqueada
        if (hit.transform != transform && !puntosMirada.Contains(hit.transform))
            continue;
}

        // Si pasa todas las pruebas para este punto, el jugador lo está mirando
        return true;
    }

    // Si ningún punto cumple las condiciones, no lo está mirando
    return false;
}

    // Si usa colisionador como detección de muerte
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivarGameOver();
        }
    }

    // Llama a la interfaz para mostrar Game Over
    void ActivarGameOver()
    {
        if (interfazGameOver != null)
        {
            interfazGameOver.ShowGameOverMessage();
            Time.timeScale = 0f;  // Pausa el juego
        }
        else
        {
            Debug.LogWarning("Interfaz Game Over no asignada.");
        }
    }
}
