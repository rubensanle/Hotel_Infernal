using UnityEngine;
using UnityEngine.AI;

public class DemonBehaviour : MonoBehaviour
{
    public Transform jugador;                     // Referencia al jugador
    public GameOverUITMP interfazGameOver;        // Referencia a la interfaz de Game Over
    public Transform[] puntosPatrulla;            // Lista de puntos por donde patrulla

    public float velocidadNormal = 27f;          // Velocidad cuando patrulla o persigue suave
    public float velocidadMatar = 35f;            // Velocidad cuando esta en modo matar
    public float distanciaMinima = 6f;            // Distancia a la que se detiene cuando no mata

    private NavMeshAgent agente;                  // Componente de navegacion
    private bool enfadado = false;                // Persigue sin matar
    private bool faseFinal = false;               // Persigue para matar
    private int puntoActual = -1;                 // Indice del punto de patrulla actual
    private float tiempoAtascado = 0f;            // Tiempo que lleva atascado
    private Vector3 ultimaPosicion;               // Para detectar si esta atascado
    private float tiempoRecalculo = 0f;           // Control de recalculo de destino

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        // Ajustar velocidad de matar según dificultad
        if (GameManager.instancia != null)
        {
            switch (GameManager.instancia.dificultadSeleccionada)
            {
                case 0: // Fácil
                    velocidadMatar = 27f;  // Más lento en Fácil
                    break;
                case 1: // Medio
                    velocidadMatar = 30f;  // Intermedio en Medio
                    break;
                case 2: // Difícil
                    velocidadMatar = 35f;  // Valor original en Difícil
                    break;
            }
        }

        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;
        agente.acceleration = 8f;
        agente.angularSpeed = 360f;              // Giro rapido para pasillos
        agente.autoBraking = true;

        // Evitacion de obstaculos de alta calidad
        agente.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agente.avoidancePriority = 50;
        agente.radius = 0.4f;                    // Radio reducido para pasillos estrechos

        ultimaPosicion = transform.position;

        MoverAPuntoDePatrulla();
    }

    void Update()
    {
        DetectarAtasco();

        // Patrulla si esta tranquilo
        if (!enfadado && !faseFinal)
        {
            if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
            {
                MoverAPuntoDePatrulla();
            }
        }

        // Persigue al jugador si esta enfadado o en fase final
        if ((enfadado || faseFinal) && jugador != null)
        {
            tiempoRecalculo += Time.deltaTime;

            // Recalcular destino cada medio segundo en vez de cada frame
            if (tiempoRecalculo >= 0.5f)
            {
                agente.SetDestination(jugador.position);
                tiempoRecalculo = 0f;
            }
        }
    }

    // Detectar si el agente esta atascado
    void DetectarAtasco()
    {
        float distanciaRecorrida = Vector3.Distance(transform.position, ultimaPosicion);

        if (distanciaRecorrida < 0.1f && agente.hasPath && !agente.isStopped)
        {
            tiempoAtascado += Time.deltaTime;

            if (tiempoAtascado > 2f)
            {
                ResolverAtasco();
            }
        }
        else
        {
            tiempoAtascado = 0f;
        }

        ultimaPosicion = transform.position;
    }

    void ResolverAtasco()
    {
        Debug.Log("Demonio atascado - recalculando ruta...");
        agente.ResetPath();
        Invoke("RecalcularRuta", 0.5f);
        tiempoAtascado = 0f;
    }

    void RecalcularRuta()
    {
        if (enfadado || faseFinal)
        {
            if (jugador != null)
            {
                agente.SetDestination(jugador.position);
            }
        }
        else
        {
            MoverAPuntoDePatrulla();
        }
    }

    void MoverAPuntoDePatrulla()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;

        int nuevoIndice;
        do
        {
            nuevoIndice = Random.Range(0, puntosPatrulla.Length);
        }
        while (nuevoIndice == puntoActual && puntosPatrulla.Length > 1);

        puntoActual = nuevoIndice;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(puntosPatrulla[puntoActual].position, out hit, 1f, NavMesh.AllAreas))
        {
            agente.SetDestination(hit.position);
            Debug.Log($"Demonio patrullando hacia: {puntosPatrulla[puntoActual].name}");
        }
        else
        {
            Debug.LogWarning($"Punto de patrulla fuera del NavMesh: {puntosPatrulla[puntoActual].name}");
        }
    }

    public void ActivarPersecucionSuave()
    {
        enfadado = true;
        faseFinal = false;
        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;
        agente.autoBraking = true;
        agente.acceleration = 8f;
        agente.angularSpeed = 360f;
    }

    public void ActivarModoMatar()
    {
        enfadado = false;
        faseFinal = true;
        agente.speed = velocidadMatar;  // Usar la velocidad ajustada por dificultad
        agente.stoppingDistance = 0f;
        agente.autoBraking = false;
        agente.acceleration = 12f;
        agente.angularSpeed = 480f;

        if (jugador != null)
        {
            agente.SetDestination(jugador.position);
        }

        Debug.Log($"Demonio en modo matar: velocidad = {velocidadMatar} (Dificultad: {GameManager.instancia?.dificultadSeleccionada})");
    }

    public void Calmar()
    {
        if (faseFinal) return;

        enfadado = false;
        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;
        agente.autoBraking = true;
        agente.acceleration = 8f;
        agente.angularSpeed = 360f;
        MoverAPuntoDePatrulla();
    }

    void OnTriggerEnter(Collider other)
    {
        if (faseFinal && other.CompareTag("Player"))
        {
            if (interfazGameOver != null)
            {
                interfazGameOver.ShowGameOverMessage();
                Time.timeScale = 0f;
                Debug.Log("El demonio ha atrapado al jugador en modo matar.");
            }
        }
    }

    void OnDrawGizmos()
    {
        if (agente != null && agente.hasPath)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < agente.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(agente.path.corners[i], agente.path.corners[i + 1]);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(agente.destination, 0.5f);
        }
    }
}
