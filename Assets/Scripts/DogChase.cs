using UnityEngine;
using UnityEngine.AI;

public class DogChase : MonoBehaviour
{
    public Transform player;
    private DogTimer timerScript;
    private NavMeshAgent agent;
    public GameOverUITMP gameOverUI;
    private Quaternion initialRotation;
    public float interactionDistance = 2f;
    private bool isPlayerNearby = false;
    private bool hasInteracted = false;

    public GameObject Indice;
    public GameObject TelefonoCanvas;
    public GameObject VentiladorCanvas;
    public GameObject TermometroCanvas;

    // 🔊 AUDIO
    public AudioClip audioSinInteractuar;   // sonido base mientras espera
    public AudioClip audioInteractuado;     // sonido cuando interactúas
    public AudioClip audioPersiguiendo;     // 🔥 sonido cuando persigue
    private AudioSource audioSource;

    void Start()
    {
        if (GameManager.instancia != null && GameManager.instancia.perroSacado)
        {
            gameObject.SetActive(false);
            return;
        }

        timerScript = GetComponent<DogTimer>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        timerScript.OnTimerExpired += OnTimerExpired;

        initialRotation = transform.rotation;

        // ▶️ Empieza con el audio de NO interactuado
        ReproducirAudioSinInteractuar();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerNearby = distanceToPlayer <= interactionDistance;

        if (!timerScript.isChasing)
            transform.rotation = initialRotation;

        // 🟢 INTERACCIÓN
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !timerScript.isChasing && !hasInteracted)
        {
            hasInteracted = true;
            Debug.Log("Interacción realizada");

            ReproducirAudioInteractuado();
        }

        // 🐕 PERSECUCIÓN
        if (timerScript.isChasing && player != null)
        {
            agent.SetDestination(player.position);

            Vector3 direction = agent.velocity.normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion correction = Quaternion.Euler(-90f, 10f, transform.rotation.eulerAngles.z);
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

            // 🔊 Vuelve al sonido de perro inquieto
            ReproducirAudioSinInteractuar();
        }
        else
        {
            timerScript.isChasing = true;
            Debug.Log("¡El enemigo comienza la persecución!");

            // 🔥 SONIDO DE PERSECUCIÓN
            ReproducirAudioPersiguiendo();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && timerScript.isChasing)
        {
            Debug.Log("¡El jugador ha sido alcanzado por el perro!");
            gameOverUI.ShowGameOverMessage();
        }
    }

    // 🔊 FUNCIONES DE AUDIO
    void ReproducirAudioSinInteractuar()
    {
        if (audioSource.clip == audioSinInteractuar) return;

        audioSource.clip = audioSinInteractuar;
        audioSource.loop = true;
        audioSource.Play();
    }

    void ReproducirAudioInteractuado()
    {
        audioSource.clip = audioInteractuado;
        audioSource.loop = true;
        audioSource.Play();
    }

    void ReproducirAudioPersiguiendo()
    {
        if (audioSource.clip == audioPersiguiendo) return;

        audioSource.clip = audioPersiguiendo;
        audioSource.loop = true;
        audioSource.Play();
    }

    void OnGUI()
    {
        if (Time.timeScale == 0f) return;

        if ((Indice != null && Indice.activeSelf) ||
            (TelefonoCanvas != null && TelefonoCanvas.activeSelf) ||
            (VentiladorCanvas != null && VentiladorCanvas.activeSelf) ||
            (TermometroCanvas != null && TermometroCanvas.activeSelf))
            return;

        if (isPlayerNearby && !timerScript.isChasing)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 36;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            Rect rect = new Rect(Screen.width / 2 - 250, Screen.height - 120, 500, 60);
            GUI.Label(rect, "Presiona E para interactuar", style);
        }

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

