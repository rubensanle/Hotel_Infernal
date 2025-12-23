using UnityEngine;
using UnityEngine.AI;

// Script que controla el comportamiento de un perro que persigue al jugador según un temporizador
public class DogChase : MonoBehaviour
{
    public Transform player;                 // Referencia al jugador
    private DogTimer timerScript;            // Script del temporizador del perro
    private NavMeshAgent agent;              // Agente de navegación
    public GameOverUITMP gameOverUI;         // UI de Game Over
    private Quaternion initialRotation;       // Rotación inicial para cuando no persigue

    public float interactionDistance = 2f;    // Distancia para permitir interacción
    private bool isPlayerNearby = false;      // Si el jugador está lo suficientemente cerca
    private bool hasInteracted = false;       // Para saber si el jugador ya interactuó

    public GameObject Indice;
    public GameObject TelefonoCanvas;
    public GameObject VentiladorCanvas;
    public GameObject TermometroCanvas;

    private DogSoundManager soundManager;

    void Start()
    {
        if (GameManager.instancia != null && GameManager.instancia.perroSacado)
        {
            gameObject.SetActive(false);
            return;
        }

            // Obtener referencias necesarias
            timerScript = GetComponent<DogTimer>();
        agent = GetComponent<NavMeshAgent>();

        // Suscripción al evento de que el temporizador acabe
        timerScript.OnTimerExpired += OnTimerExpired;

        // Guardar rotación inicial
        initialRotation = transform.rotation;
        soundManager = GetComponent<DogSoundManager>();

    }

    void Update()
    {
        // Calcular distancia al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerNearby = distanceToPlayer <= interactionDistance;

        // Si no persigue, mantener la rotación inicial
        if (!timerScript.isChasing)
        {
            transform.rotation = initialRotation;
        }

        // Si el jugador está cerca y presiona la tecla E, y todavía no persigue
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !timerScript.isChasing)
        {
            hasInteracted = true;
            Debug.Log("Interacción realizada");
        }

        // Si el perro está persiguiendo
        if (timerScript.isChasing && player != null)
        {
            // Actualizar destino del agente
            agent.SetDestination(player.position);

            // Ajustar rotación para que mire hacia la dirección en que se mueve
            Vector3 direction = agent.velocity.normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Corrección manual de rotación según la orientación del modelo 3D
                Quaternion correction = Quaternion.Euler(-90f, 10f, transform.rotation.eulerAngles.z);

                transform.rotation = targetRotation * correction;
            }
        }
    }

    // Se llama cuando el temporizador llega a cero
    void OnTimerExpired()
    {
        if (hasInteracted)
        {
            // 🔊 Sonido cuando SÍ hemos interactuado
            if (soundManager != null)
                soundManager.PlayInteractSound();

            hasInteracted = false;
            timerScript.RestartTimer();
            Debug.Log("Temporizador reiniciado tras interacción");
        }
        else
        {
            // 🔊 Sonido cuando NO hemos interactuado
            if (soundManager != null)
                soundManager.PlayNoInteractSound();

            timerScript.isChasing = true;
            Debug.Log("¡El enemigo comienza la persecución!");
        }
    }


    // Cuando el perro colisiona con el jugador
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && timerScript.isChasing)
        {
            Debug.Log("¡El jugador ha sido alcanzado por el perro!");
            gameOverUI.ShowGameOverMessage();
        }
        else if (other.CompareTag("Player"))
        {
            Debug.Log("El jugador está cerca del perro, pero no está siendo perseguido.");
        }
    }

    // Mostrar interacción y temporizador en pantalla
    void OnGUI()
    {
        if (Time.timeScale == 0f) return;

        // Si alguno de los Canvas de interacción está activo, no mostrar GUI
        if ((Indice != null && Indice.activeSelf) ||
            (TelefonoCanvas != null && TelefonoCanvas.activeSelf) ||
            (VentiladorCanvas != null && VentiladorCanvas.activeSelf) ||
            (TermometroCanvas != null && TermometroCanvas.activeSelf))
        {
            return;
        }

        // Mensaje para interactuar si el jugador está cerca y el perro no persigue
        if (isPlayerNearby && !timerScript.isChasing)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 36;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            Rect rect = new Rect(Screen.width / 2 - 250, Screen.height - 120, 500, 60);
            GUI.Label(rect, "Presiona E para interactuar", style);
        }

        // Mostrar temporizador mientras el perro no persigue
        if (!timerScript.isChasing && GameManager.instancia.relojesArreglados)
        {
            GUIStyle timerStyle = new GUIStyle(GUI.skin.label);
            timerStyle.fontSize = 40;
            timerStyle.normal.textColor = Color.red;
            timerStyle.alignment = TextAnchor.UpperRight;

            Rect timerRect = new Rect(Screen.width - 520, 40, 400, 50);
            GUI.Label(timerRect, "Tiempo perro: " + timerScript.GetTimeRemaining().ToString("F1") + "s", timerStyle);
        }
    }



}
