using UnityEngine;
using UnityEngine.AI;

public class DemonBehaviour : MonoBehaviour
{
    public Transform jugador;
    public Transform[] puntosPatrulla;
    public GameOverUITMP interfazGameOver;
    public InteractionDemon scriptInteraccion;

    private NavMeshAgent agente;
    private int indicePatrulla = 0;
    private bool enfadado = false;
    private bool faseFinal = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        IrAlSiguientePunto();
    }

    void Update()
    {
        if (!enfadado && !faseFinal)
        {
            Patrullar();
        }
        else
        {
            PerseguirJugador();
        }
    }

    public void Enfadar()
    {
        enfadado = true;
        Debug.Log("El demonio se ha enfadado y comienza la persecución.");
    }

    public void Calmar()
    {
        enfadado = false;
        faseFinal = false;

        Vector3 destinoAleatorio = GenerarDestinoAleatorio();
        agente.SetDestination(destinoAleatorio);

        Debug.Log("El demonio se ha calmado y vuelve a patrullar aleatoriamente.");
    }


    public void Teletransportarse()
    {
        Vector3 posicionJugador = jugador.position;
        Vector3 direccionFrontal = jugador.forward;

        Vector3 nuevaPosicion = posicionJugador + direccionFrontal * 1.5f;

        transform.position = nuevaPosicion;

        transform.LookAt(posicionJugador);
    }


    void Patrullar()
    {
        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            Vector3 destinoAleatorio = GenerarDestinoAleatorio();
            agente.SetDestination(destinoAleatorio);
        }
    }

    Vector3 GenerarDestinoAleatorio()
    {
        float rango = 10f; 
        Vector3 centro = transform.position;

        Vector3 puntoAleatorio = centro + new Vector3(
            Random.Range(-rango, rango),
            0,
            Random.Range(-rango, rango)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(puntoAleatorio, out hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position; 
    }


    void IrAlSiguientePunto()
    {
        if (puntosPatrulla.Length > 0)
            agente.SetDestination(puntosPatrulla[indicePatrulla].position);
    }

    void PerseguirJugador()
    {
        if (jugador != null)
            agente.SetDestination(jugador.position);
    }

    void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("Player") && scriptInteraccion.EstaEnFaseFinal())
        {
            interfazGameOver.ShowGameOverMessage();
            Debug.Log("El demonio ha matado al jugador.");
        }
    }


}

