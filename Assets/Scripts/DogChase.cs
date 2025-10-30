using UnityEngine;
using UnityEngine.AI;

public class DogChase : MonoBehaviour
{
    public Transform player;
    private DogTimer timerScript;
    private NavMeshAgent agent;
    public GameOverUI gameOverUI;
    private Quaternion initialRotation;

    void Start()
    {
        timerScript = GetComponent<DogTimer>();
        agent = GetComponent<NavMeshAgent>();
        timerScript.OnTimerExpired += StartChase;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (!timerScript.isChasing)
    {
            transform.rotation = initialRotation; // Mantiene la rotación original
        }
        if (timerScript.isChasing && player != null)
        {
            agent.SetDestination(player.position);

            Vector3 direction = agent.velocity.normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion correction = Quaternion.Euler(-90f, 10f, transform.rotation.eulerAngles.z); // Ajusta según tu modelo
                transform.rotation = targetRotation * correction;
            }
        }
    }

    void StartChase()
    {
        Debug.Log("¡El enemigo comienza la persecución!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡El jugador ha sido alcanzado!");
            gameOverUI.ShowGameOverMessage();
        }
    }
}
