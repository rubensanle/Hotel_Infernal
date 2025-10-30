using UnityEngine;
using UnityEngine.AI;

public class DogChase : MonoBehaviour
{
    public Transform player;
    private DogTimer timerScript;
    private NavMeshAgent agent;
    public GameOverUI gameOverUI;
    private Quaternion initialRotation;

    public float interactionDistance = 2f;
    private bool isPlayerNearby = false;
    private bool hasInteracted = false;

    void Start()
    {
        timerScript = GetComponent<DogTimer>();
        agent = GetComponent<NavMeshAgent>();
        timerScript.OnTimerExpired += OnTimerExpired;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerNearby = distanceToPlayer <= interactionDistance;

        if (!timerScript.isChasing)
        {
            transform.rotation = initialRotation;
        }

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !timerScript.isChasing)
        {
            hasInteracted = true;
            Debug.Log("Interacción realizada");
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

    void OnTimerExpired()
    {
        if (hasInteracted)
        {
            hasInteracted = false;
            timerScript.RestartTimer();
            Debug.Log("Temporizador reiniciado tras interacción");
        }
        else
        {
            timerScript.isChasing = true;
            Debug.Log("¡El enemigo comienza la persecución!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡El jugador ha sido alcanzado!");
            gameOverUI.ShowGameOverMessage();
        }
    }

    void OnGUI()
    {
        if (isPlayerNearby && !timerScript.isChasing)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            Rect rect = new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 50);
            GUI.Label(rect, "Presiona E para interactuar", style);
        }

        if (!timerScript.isChasing)
        {
            GUIStyle timerStyle = new GUIStyle(GUI.skin.label);
            timerStyle.fontSize = 20;
            timerStyle.normal.textColor = Color.red;
            timerStyle.alignment = TextAnchor.UpperCenter;

            Rect timerRect = new Rect(Screen.width / 2 - 100, 20, 200, 30);
            GUI.Label(timerRect, "Tiempo restante: " + timerScript.GetTimeRemaining().ToString("F1"), timerStyle);
        }
    }
}
