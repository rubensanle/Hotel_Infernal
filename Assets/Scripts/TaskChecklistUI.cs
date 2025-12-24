using TMPro;   // Para usar TextMeshProUGUI
using UnityEngine;

public class TaskChecklistTMP : MonoBehaviour
{
    [Header("Canvas del checklist")]
    public GameObject Indice;   // El Canvas que contiene los textos

    [Header("Textos TMP de cada tarea")]
    public TextMeshProUGUI tareaLatas;
    public TextMeshProUGUI tareaCama;
    public TextMeshProUGUI tareaToalla;
    public TextMeshProUGUI tareaPatitos;
    public TextMeshProUGUI tareaLimpieza;
    public TextMeshProUGUI tareaVateres;
    public TextMeshProUGUI tareaGrifos;
    public TextMeshProUGUI tareaCuadros;
    public TextMeshProUGUI tareaLamparas;
    public TextMeshProUGUI tareaTelefono;
    public TextMeshProUGUI tareaTermometro;
    public TextMeshProUGUI tareaVentilador;

    [Header("Managers de tareas")]
    public BedTaskManager bedTaskManager;
    public CleanerManager cleanerManager;
    public ToiletTaskManager toiletTaskManager;
    public FaucetTaskManager faucetTaskManager;
    public FrameTaskManager frameTaskManager;
    public LampTaskManager lampTaskManager;
    public TelefonoInteract telefono;
    public TermometroInteract termometro;
    public VentiladorInteract ventilador;

    [Header("Sonidos")]
    public AudioSource audioSource;       // AudioSource del jugador
    public AudioClip gruñidos;

    void Start()
    {
        // Nada más empezar la escena, el Canvas está oculto
        if (Indice != null) { 
            Indice.SetActive(false);
         }
    }

    void Update()
    {
        // Abrir/cerrar con la tecla M
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (Indice != null && audioSource != null)
            {
                Indice.SetActive(!Indice.activeSelf);
                audioSource.PlayOneShot(gruñidos);
                Debug.Log("Indice alternado con M");
            }
        }

        // Solo actualiza tareas si el Canvas está visible
        if (Indice != null && Indice.activeSelf)
        {
            ActualizarTareas();
        }
    }

    void ActualizarTareas()
    {
        if (tareaLatas != null)
            tareaLatas.color = TrashPickUp.TareaCompletada() ? Color.green : Color.red;

        if (tareaCama != null)
            tareaCama.color = bedTaskManager != null && bedTaskManager.TareaCompletada() ? Color.green : Color.red;

        if (tareaToalla != null)
            tareaToalla.color = ToallaPickup.TareaCompletadaStatic() ? Color.green : Color.red;

        if (tareaPatitos != null)
            tareaPatitos.color = PatitoPickup.TareaCompletada() ? Color.green : Color.red;

        if (tareaLimpieza != null)
            tareaLimpieza.color = cleanerManager != null && cleanerManager.TareaCompletada() ? Color.green : Color.red;

        if (tareaVateres != null)
            tareaVateres.color = toiletTaskManager != null && toiletTaskManager.TareaCompletada() ? Color.green : Color.red;

        if (tareaGrifos != null)
            tareaGrifos.color = faucetTaskManager != null && faucetTaskManager.TareaCompletada() ? Color.green : Color.red;

        if (tareaCuadros != null)
            tareaCuadros.color = frameTaskManager != null && frameTaskManager.TareaCompletada() ? Color.green : Color.red;

        if (tareaLamparas != null)
            tareaLamparas.color = lampTaskManager != null && lampTaskManager.TareaCompletada() ? Color.green : Color.red;

        if (tareaTelefono != null)
            tareaTelefono.color = telefono != null && telefono.TareaCompletada() ? Color.green : Color.red;

        if (tareaTermometro != null)
            tareaTermometro.color = termometro != null && termometro.TareaCompletada() ? Color.green : Color.red;

        if (tareaVentilador != null)
            tareaVentilador.color = ventilador != null && ventilador.TareaCompletada() ? Color.green : Color.red;
    }

}




