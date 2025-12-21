using UnityEngine;

public class TrashPickUp : MonoBehaviour
{
    public PlayerMovement playerMovement;         // Movimiento del jugador
    public Transform trashAnchor;                 // Punto donde se sujeta la basura
    public GameObject trashVisualPrefab;          // Prefab visual que aparece en el contenedor
    public Transform puntoColocacion;             // Lugar exacto de colocación
    public Camera camaraJugador;                  // Cámara del jugador
    public float radioInteraccion = 0.5f;         // Radio del SphereCast
    public float distanciaInteraccion = 3.5f;     // Distancia máxima de interacción

    private bool recogido = false;                // Si la basura ya fue recogida
    private bool entregado = false;               // Si ya se entregó
    private bool cerca = false;                   // Si estamos mirando la basura
    private bool cercaContenedor = false;         // Si estamos cerca de la papelera

    // ?? NUEVO: Guardar transform inicial
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private Vector3 escalaInicial;

    // Variables estáticas para contar todas las latas
    public static int latasRecogidas = 0;
    public static int totalLatas = 0;
    public static int latasEntregadas = 0;

    [Header("Sonido lata")]
    public PlayerSounds playerSounds; // referencia al script de sonidos del jugador


    void Start()
    {
        // Guardar transform inicial
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
        escalaInicial = transform.localScale;

        // Contar este objeto como una lata más
        totalLatas++;
    }

    void Update()
    {
        // Detectar basura para recoger
        cerca = DetectarBasura();

        // Recoger lata si no está recogida y el jugador no lleva objeto
        if (!recogido && !playerMovement.EstaLlevandoObjeto && cerca && Input.GetKeyDown(KeyCode.E))
        {
            RecogerLata();
        }

        // Detectar contenedor para entregar
        if (recogido && !entregado)
        {
            cercaContenedor = DetectarContenedor();
            if (cercaContenedor && Input.GetKeyDown(KeyCode.E))
            {
                EntregarLata();
            }
        }
    }

    // Recoger una lata
    void RecogerLata()
    {
        if (playerSounds != null)
            playerSounds.PlayGruñido();


        recogido = true;
        latasRecogidas++;
        playerMovement.LlevarObjeto(true, true);

        // Colocar en mano del jugador
        transform.SetParent(trashAnchor);
        transform.localPosition = new Vector3(0, -0.4f, 0);
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one * 0.5f;
        GetComponent<Collider>().enabled = false;
    }

    // Entregar una lata en el contenedor
    void EntregarLata()
    {
        entregado = true;
        latasEntregadas++;
        playerMovement.SoltarObjeto();
        gameObject.SetActive(false);

        // Instanciar el prefab visual en el contenedor
        if (trashVisualPrefab != null && puntoColocacion != null)
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);
            Instantiate(trashVisualPrefab, puntoColocacion.position + offset, puntoColocacion.rotation);
        }
    }

    // Método estático para verificar si todas las latas fueron entregadas
    public static bool TodasLasLatasEntregadas()
    {
        return latasEntregadas >= totalLatas;
    }

    // Método estático para verificar si la tarea está completada
    public static bool TareaCompletada()
    {
        return TodasLasLatasEntregadas();
    }

    // Propiedades estáticas para acceder desde otros scripts
    public static int LatasEntregadas => latasEntregadas;
    public static int TotalLatas => totalLatas;

    // Resetear contador (útil si reinicias el juego)
    public static void ResetearContador()
    {
        latasRecogidas = 0;
        latasEntregadas = 0;
        totalLatas = 0;
    }

    // ?? NUEVO MÉTODO: Resetear lata individual
    public void ResetTask()
    {
        Debug.Log("?? Reseteando lata de basura...");

        recogido = false;
        entregado = false;
        cerca = false;
        cercaContenedor = false;

        // Reactivar el objeto
        gameObject.SetActive(true);

        // Restaurar transform inicial
        transform.SetParent(null);
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
        transform.localScale = escalaInicial;

        // Reactivar collider
        GetComponent<Collider>().enabled = true;

        Debug.Log("? Lata de basura reseteada a estado inicial");
    }

    // Detectar si el jugador está mirando esta basura
    bool DetectarBasura()
    {
        Ray ray = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, radioInteraccion, out hit, distanciaInteraccion))
            return hit.collider != null && hit.collider.gameObject == gameObject;
        return false;
    }

    // Detectar si el jugador está cerca del contenedor
    bool DetectarContenedor()
    {
        Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Contenedor"))
                return true;
        }
        return false;
    }

    // Mostrar mensajes de interacción en pantalla
    void OnGUI()
    {
        if (Time.timeScale == 0f)
            return;

        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 40;
        estilo.normal.textColor = Color.white;
        estilo.alignment = TextAnchor.MiddleCenter;
        Rect mensaje = new Rect(Screen.width / 2 - 200, Screen.height - 120, 400, 80);

        // Mensaje al recoger
        if (cerca && !recogido)
            GUI.Label(mensaje, "Pulsa E para recoger basura", estilo);

        // Mensaje al entregar
        if (recogido && !entregado && cercaContenedor)
            GUI.Label(mensaje, "Pulsa E para entregar basura", estilo);
    }
}