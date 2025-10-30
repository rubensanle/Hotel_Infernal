using UnityEngine;
using UnityEngine.AI;

public class DemonBehaviour : MonoBehaviour
{
    public Transform jugador;
    public Transform[] puntosPatrulla;
    public GameOverUI interfazGameOver;
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
        agente.SetDestination(puntosPatrulla[indicePatrulla].position);
        Debug.Log("El demonio se ha calmado y vuelve a patrullar.");
    }

    public void Teletransportarse()
    {
        faseFinal = true;
        transform.position = jugador.position + jugador.forward * 2f;
        Debug.Log("El demonio se ha teletransportado frente al jugador.");
    }

    void Patrullar()
    {
        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
            IrAlSiguientePunto();
        }
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

