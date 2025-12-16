using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// Demonio que rota hacia el jugador y persigue con precision cuando esta enfadado
public class DemonBehaviour2 : MonoBehaviour
{
    // Referencias
    public Transform jugador;
    public GameOverUITMP interfazGameOver;
    public TelefonoInteract telefono;

    // Rotacion
    public float velocidadRotacion = 6f; // rotacion mas rapida para apuntar al jugador

    // NavMesh
    private NavMeshAgent agente;

    // Animator
    private Animator anim;

    // Estado
    private bool enfadado = false;
    private Vector3 puntoOrigen;

    // Parametros de captura
    public float distanciaMatar = 0.6f; // distancia exacta para matar

    // Velocidad base (para cálculo según dificultad)
    public float velocidadBaseMatar = 70f; // Valor base original (100%)

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Calcular velocidad final según dificultad
        float velocidadFinal = velocidadBaseMatar;

        if (GameManager.instancia != null)
        {
            switch (GameManager.instancia.dificultadSeleccionada)
            {
                case 0: // Fácil - 70% de la velocidad
                    velocidadFinal = velocidadBaseMatar * 0.7f;
                    break;
                case 1: // Medio - 85% de la velocidad
                    velocidadFinal = velocidadBaseMatar * 0.85f;
                    break;
                case 2: // Difícil - 100% de la velocidad
                    velocidadFinal = velocidadBaseMatar; // Valor original
                    break;
            }
        }

        if (agente != null)
        {
            // Configuracion para parada precisa en el objetivo
            agente.isStopped = true;           // inicia quieto
            agente.stoppingDistance = 0.5f;    // margen para no atravesar al jugador
            agente.autoBraking = true;         // imprescindible para frenar en el destino
            agente.updateRotation = false;     // rotacion manual para mayor control
            agente.updatePosition = true;      // que el agente actualice su posicion
            agente.acceleration = 300f;        // respuesta inmediata
            agente.angularSpeed = 720f;        // giros rapidos
            agente.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

            // Establecer la velocidad ajustada
            agente.speed = velocidadFinal;
        }

        puntoOrigen = transform.position;
    }

    void Update()
    {
        if (jugador == null || agente == null) return;

        // Rotar siempre hacia el jugador
        Vector3 dir = jugador.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion rotObjetivo = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotObjetivo, velocidadRotacion * 90f * Time.deltaTime);
        }

        // Perseguir con recalculo continuo del destino
        if (enfadado)
        {
            agente.isStopped = false;
            // Velocidad ya está configurada en Start según dificultad
            agente.SetDestination(jugador.position); // actualiza destino cada frame

            // Si esta dentro de la distancia de matar, activa GameOver
            float dist = Vector3.Distance(transform.position, jugador.position);
            if (dist <= distanciaMatar)
            {
                ActivarGameOver();
                return;
            }

            // Si ya alcanzo el stoppingDistance, frena para no pasarse
            if (dist <= agente.stoppingDistance + 0.05f)
            {
                agente.isStopped = true; // detener en el borde
            }
        }
        ActualizarAnimacion();
    }

    // Animacion
    void ActualizarAnimacion()
    {
        if (anim == null || agente == null) return;

        // Está caminando si el agente no está parado y se está moviendo algo
        bool estaCaminando =
            !agente.isStopped &&
            agente.velocity.sqrMagnitude > 0.01f;  // velocidad > 0

        anim.SetBool("isWalking", estaCaminando);
    }

    // Activa persecucion rapida durante un tiempo limitado
    public void ActivarPersecucionRapida()
    {
        if (agente != null && jugador != null)
        {
            enfadado = true;
            agente.isStopped = false;
            // La velocidad ya está configurada según dificultad en Start

            Debug.Log($"Demonio2 activado - Velocidad: {agente.speed} (Dificultad: {GameManager.instancia?.dificultadSeleccionada})");

            // Opcional: calma despues de 10 segundos
            StartCoroutine(CalmarDespuesDeTiempo(15f));
        }
    }

    private IEnumerator CalmarDespuesDeTiempo(float segundos)
    {
        yield return new WaitForSeconds(segundos);

        enfadado = false;

        if (agente != null)
        {
            agente.speed = 5f;              // velocidad normal
            agente.isStopped = false;
            agente.autoBraking = true;      // mantener frenado en destino
            agente.SetDestination(puntoOrigen);
        }
    }

    // Kill adicional por trigger si tienes collider con isTrigger
    void OnTriggerEnter(Collider other)
    {
        if (enfadado && other.CompareTag("Player"))
        {
            ActivarGameOver();
        }
    }

    // Lanza GameOver y cierra el telefono si estaba abierto
    void ActivarGameOver()
    {
        // Cerrar canvas del telefono si estaba abierto
        if (telefono != null)
        {
            telefono.CerrarCanvas();
        }

        if (interfazGameOver != null)
        {
            interfazGameOver.ShowGameOverMessage();
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Interfaz Game Over no asignada en DemonBehaviour2.");
        }
    }
}