using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TelefonoInteract : MonoBehaviour
{
    public Camera camaraJugador;                  // Cámara del jugador
    public float radioInteraccion = 0.5f;         // Radio del SphereCast
    public float distanciaInteraccion = 3.5f;     // Distancia máxima de interacción

    public GameObject canvasTelefono;             // Canvas del teléfono
    public TMP_InputField inputCodigo;            // Campo de entrada para el código
    public Button botonConfirmar;                 // Botón de confirmación

    public DemonBehaviour2 demonio2;              // Referencia al segundo demonio
    public PlayerMovement playerMovement;         // Referencia al jugador

    private bool abierto = false;                 // Si el canvas está abierto
    private bool cerca = false;                   // Si el jugador está cerca del teléfono
    private bool tareaCompletada = false;         // Si la tarea está completada

    void Start()
    {
        // Configurar UI al inicio
        canvasTelefono.SetActive(false);
        botonConfirmar.onClick.AddListener(ValidarCodigo);
    }

    void Update()
    {
        // Detectar si el jugador está cerca del teléfono
        cerca = DetectarTelefono();

        // Abrir canvas si no está completado, no está abierto y el jugador interactúa
        if (!tareaCompletada && !abierto && !playerMovement.EstaLlevandoObjeto && cerca && Input.GetKeyDown(KeyCode.E))
        {
            abierto = true;
            canvasTelefono.SetActive(true);
            inputCodigo.text = "";
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Canvas del teléfono abierto");
        }
    }

    // Detectar si el jugador está mirando el teléfono
    bool DetectarTelefono()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, radioInteraccion, out hit, distanciaInteraccion))
            return hit.collider != null && hit.collider.gameObject == gameObject;
        return false;
    }

    // Validar el código ingresado
    public void ValidarCodigo()
    {
        string codigoIngresado = inputCodigo.text;

        // Marcar como completada independientemente del resultado
        tareaCompletada = true;
        Debug.Log("📞 Teléfono - Tarea marcada como completada");

        if (codigoIngresado == "HAB-01")
        {
            Debug.Log("📞 Teléfono - Código correcto");
        }
        else
        {
            Debug.Log("📞 Teléfono - Código incorrecto");
            if (demonio2 != null)
                demonio2.ActivarPersecucionRapida();
        }

        CerrarCanvas();
    }

    // Cerrar el canvas del teléfono
    public void CerrarCanvas()
    {
        canvasTelefono.SetActive(false);
        abierto = false;
    }

    // Método para verificar si la tarea está completada
    public bool TareaCompletada()
    {
        return tareaCompletada;
    }

    // 🔄 NUEVO MÉTODO: Resetear teléfono
    public void ResetTask()
    {
        Debug.Log("🔄 Reseteando teléfono...");

        tareaCompletada = false;
        abierto = false;
        cerca = false;

        // Cerrar canvas si está abierto
        if (canvasTelefono != null)
            canvasTelefono.SetActive(false);

        // Restaurar estado del cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("✅ Teléfono reseteado");
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