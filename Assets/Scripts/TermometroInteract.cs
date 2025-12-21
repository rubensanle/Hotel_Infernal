using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TermometroInteract : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject canvasTermometro;           // Canvas del termómetro
    public Slider sliderTemperatura;              // Slider para seleccionar temperatura
    public TMP_Text textoTemperatura;             // Texto que muestra la temperatura
    public Button botonConfirmar;                 // Botón de confirmación

    [Header("Rango correcto")]
    public float minCorrecto = 23f;               // Temperatura mínima correcta
    public float maxCorrecto = 25f;               // Temperatura máxima correcta

    private bool abierto = false;                 // Si el canvas está abierto
    private bool cerca = false;                   // Si el jugador está cerca del termómetro
    private bool tareaCompletada = false;         // Si la tarea está completada

    public DemonBehaviour2 demonio2;              // Referencia al segundo demonio
    public PlayerMovement playerMovement;         // Referencia al jugador

    void Start()
    {
        // Configurar UI al inicio
        canvasTermometro.SetActive(false);
        sliderTemperatura.onValueChanged.AddListener(ActualizarTexto);
        botonConfirmar.onClick.AddListener(ValidarTemperatura);
    }

    void Update()
    {
        // Detectar si el jugador está cerca del termómetro
        cerca = DetectarTermometro();

        // Abrir canvas si no está completado, no está abierto y el jugador interactúa
        if (!tareaCompletada && !abierto && !playerMovement.EstaLlevandoObjeto && cerca && Input.GetKeyDown(KeyCode.E))
        {
            abierto = true;
            canvasTermometro.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Canvas del termómetro abierto");
        }
    }

    // Detectar si el jugador está mirando el termómetro
    bool DetectarTermometro()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.5f, out hit, 3.5f))
            return hit.collider != null && hit.collider.gameObject == gameObject;
        return false;
    }

    // Actualizar texto de la temperatura cuando cambia el slider
    void ActualizarTexto(float valor)
    {
        textoTemperatura.text = valor.ToString("F1") + " °C";
    }

    // Validar la temperatura seleccionada
    public void ValidarTemperatura()
    {
        float valor = sliderTemperatura.value;

        // Marcar como completada independientemente del resultado
        tareaCompletada = true;
        Debug.Log("🌡️ Termómetro - Tarea marcada como completada");

        if (valor >= minCorrecto && valor <= maxCorrecto)
        {
            Debug.Log($"🌡️ Termómetro - Temperatura correcta: {valor}");
        }
        else
        {
            Debug.Log($"🌡️ Termómetro - Temperatura incorrecta: {valor}");
            if (demonio2 != null)
                demonio2.ActivarPersecucionRapida();
        }

        CerrarCanvas();
    }

    // Cerrar el canvas del termómetro
    void CerrarCanvas()
    {
        canvasTermometro.SetActive(false);
        abierto = false;
    }

    // Método para verificar si la tarea está completada
    public bool TareaCompletada()
    {
        return tareaCompletada;
    }

    // 🔄 NUEVO MÉTODO: Resetear termómetro
    public void ResetTask()
    {
        Debug.Log("🔄 Reseteando termómetro...");

        tareaCompletada = false;
        abierto = false;
        cerca = false;

        // Cerrar canvas si está abierto
        if (canvasTermometro != null)
            canvasTermometro.SetActive(false);

        // Restaurar estado del cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Resetear slider a valor por defecto (opcional)
        if (sliderTemperatura != null)
            sliderTemperatura.value = sliderTemperatura.minValue;

        Debug.Log("✅ Termómetro reseteado");
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
