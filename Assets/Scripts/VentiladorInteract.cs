using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VentiladorInteract : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject canvasVentilador;           // Canvas del ventilador
    public Slider sliderTiempo;                   // Slider para seleccionar tiempo
    public TMP_Text textoTiempo;                  // Texto que muestra el tiempo
    public TMP_Dropdown dropdownPotencia;         // Dropdown para seleccionar potencia
    public Button botonConfirmar;                 // Botón de confirmación

    [Header("Referencias externas")]
    public DemonBehaviour2 demonio2;              // Referencia al segundo demonio
    public PlayerMovement playerMovement;         // Referencia al jugador

    [Header("Valores correctos")]
    public int potenciaCorrecta = 3;              // Potencia correcta a seleccionar
    public int tiempoCorrecto = 90;               // Tiempo correcto a seleccionar

    private bool abierto = false;                 // Si el canvas está abierto
    private bool cerca = false;                   // Si el jugador está cerca del ventilador
    private bool tareaCompletada = false;         // Si la tarea está completada

    void Start()
    {
        // Configurar UI al inicio
        canvasVentilador.SetActive(false);
        sliderTiempo.onValueChanged.AddListener(ActualizarTiempo);
        dropdownPotencia.onValueChanged.AddListener(ActualizarPotencia);
        botonConfirmar.onClick.AddListener(ValidarVentilador);
        ActualizarTiempo(sliderTiempo.value);
        ActualizarPotencia(dropdownPotencia.value);
    }

    void Update()
    {
        // Detectar si el jugador está cerca del ventilador
        cerca = DetectarVentilador();

        // Abrir canvas si no está completado, no está abierto y el jugador interactúa
        if (!tareaCompletada && !abierto && !playerMovement.EstaLlevandoObjeto && cerca && Input.GetKeyDown(KeyCode.E))
        {
            abierto = true;
            canvasVentilador.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Canvas del ventilador abierto");
        }
    }

    // Detectar si el jugador está mirando el ventilador
    bool DetectarVentilador()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.5f, out hit, 3.5f))
            return hit.collider != null && hit.collider.gameObject == gameObject;
        return false;
    }

    // Actualizar texto del tiempo cuando cambia el slider
    void ActualizarTiempo(float valor)
    {
        textoTiempo.text = valor.ToString("F0") + " min";
    }

    // Actualizar cuando cambia la potencia (para debug)
    void ActualizarPotencia(int indice)
    {
        Debug.Log("Potencia seleccionada: " + dropdownPotencia.options[indice].text);
    }

    // Validar la configuración del ventilador
    public void ValidarVentilador()
    {
        int tiempo = (int)sliderTiempo.value;
        int potencia = int.Parse(dropdownPotencia.options[dropdownPotencia.value].text);

        // Marcar como completada independientemente del resultado
        tareaCompletada = true;

        // Verificar si la configuración es correcta
        if (tiempo == tiempoCorrecto && potencia == potenciaCorrecta)
        {
            Debug.Log("Ventilador configurado correctamente");
        }
        else
        {
            Debug.Log("Ventilador configurado incorrectamente, demonio enfadado!");
            if (demonio2 != null)
                demonio2.ActivarPersecucionRapida();
        }

        CerrarCanvas();
    }

    // Cerrar el canvas del ventilador
    void CerrarCanvas()
    {
        canvasVentilador.SetActive(false);
        abierto = false;
    }

    // Método para verificar si la tarea está completada
    public bool TareaCompletada()
    {
        return tareaCompletada;
    }

    // ?? NUEVO MÉTODO: Resetear ventilador
    public void ResetTask()
    {
        Debug.Log("?? Reseteando ventilador...");

        tareaCompletada = false;
        abierto = false;
        cerca = false;

        // Cerrar canvas si está abierto
        if (canvasVentilador != null)
            canvasVentilador.SetActive(false);

        // Restaurar estado del cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Resetear UI a valores por defecto (opcional)
        if (sliderTiempo != null)
            sliderTiempo.value = sliderTiempo.minValue;

        if (dropdownPotencia != null)
            dropdownPotencia.value = 0;

        Debug.Log("? Ventilador reseteado");
    }

    // Mostrar mensaje de interacción en pantalla
    void OnGUI()
    {
        if (Time.timeScale == 0f)
            return;

        // Solo mostrar mensaje si no está completada y no está abierto
        if (cerca && !abierto && !tareaCompletada)
        {
            GUIStyle estilo = new GUIStyle(GUI.skin.label);
            estilo.fontSize = 40;
            estilo.normal.textColor = Color.white;
            estilo.alignment = TextAnchor.MiddleCenter;
            Rect mensaje = new Rect(Screen.width / 2 - 200, Screen.height - 120, 400, 80);
            GUI.Label(mensaje, "Pulsa E para interactuar", estilo);
        }
    }
}