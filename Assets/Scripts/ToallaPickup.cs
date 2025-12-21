using UnityEngine;

public class ToallaPickup : MonoBehaviour
{
    public PlayerMovement playerMovement;         // Movimiento del jugador
    public Transform toallaAnchor;                // Punto donde se sujeta la toalla
    public GameObject toallaVisualPrefab;         // Prefab visual que aparece al entregar
    public Transform puntoColocacion;             // Lugar exacto de colocación
    public DemonBehaviour2 demonio2;              // Referencia al segundo demonio

    public Camera camaraJugador;                  // Cámara del jugador
    public float radioInteraccion = 0.5f;         // Radio del SphereCast
    public float distanciaInteraccion = 3.5f;     // Distancia máxima de interacción

    private bool recogida = false;                // Si la toalla ya fue recogida
    private bool entregada = false;               // Si ya se entregó
    private bool cerca = false;                   // Si estamos mirando la toalla
    private bool cercaEntrega = false;            // Si estamos cerca del punto de entrega

    // 🔄 NUEVO: Guardar posición y rotación inicial
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private Vector3 escalaInicial;

    // VARIABLE ESTÁTICA para rastrear si la toalla fue entregada
    public static bool toallaEntregadaStatic = false;

    [Header("Sonido toalla")]
    public PlayerSounds playerSounds; // referencia al script de sonidos del jugador


    void Start()
    {
        // Guardar transform inicial
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
        escalaInicial = transform.localScale;
    }

    void Update()
    {
        // Detectar toalla para recoger
        cerca = DetectarToalla();

        // NO SE PUEDE RECOGER SI YA LLEVA OTRO OBJETO
        if (!recogida && !playerMovement.EstaLlevandoObjeto && cerca && Input.GetKeyDown(KeyCode.E))
        {
            recogida = true;

            // La toalla SÍ reduce velocidad → reduceVelocidad = true
            playerMovement.LlevarObjeto(true, true);

            if (playerSounds != null)
                playerSounds.PlayGruñido();

            // Colocar en mano del jugador
            transform.SetParent(toallaAnchor);
            transform.localPosition = new Vector3(0, -0.25f, 0.5f);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 0.28f;

            GetComponent<Collider>().enabled = false;

            Debug.Log("Toalla recogida");
        }

        // Entregar toalla
        if (recogida && !entregada)
        {
            // Detectamos si estamos cerca de un punto de entrega
            cercaEntrega = DetectarEntrega();

            Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);

            foreach (Collider hit in hits)
            {
                // Entregar en sitio correcto
                if (hit.CompareTag("EntregaToalla") && Input.GetKeyDown(KeyCode.E))
                {
                    entregada = true;
                    toallaEntregadaStatic = true; // ACTUALIZAR VARIABLE ESTÁTICA
                    playerMovement.SoltarObjeto();
                    gameObject.SetActive(false);

                    // Instanciar prefab visual en el punto de entrega
                    if (toallaVisualPrefab != null && puntoColocacion != null)
                    {
                        Vector3 offset = new Vector3(0, 0.5f, 0);
                        Instantiate(toallaVisualPrefab, puntoColocacion.position + offset, puntoColocacion.rotation);
                    }

                    Debug.Log("🛁 Toalla entregada correctamente - MARCADA COMO COMPLETADA");
                }

                // Entregar en sitio incorrecto
                if (hit.CompareTag("EntregaToallaWrong") && Input.GetKeyDown(KeyCode.E))
                {
                    entregada = true;
                    toallaEntregadaStatic = true; // ACTUALIZAR VARIABLE ESTÁTICA
                    playerMovement.SoltarObjeto();
                    gameObject.SetActive(false);

                    Debug.Log("🛁 Toalla entregada en el sitio equivocado - PERO MARCADA COMO COMPLETADA");

                    // Activar persecución del segundo demonio
                    if (demonio2 != null)
                        demonio2.ActivarPersecucionRapida();
                }
            }
        }
    }

    // Detectar si miramos la toalla
    bool DetectarToalla()
    {
        Ray ray = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, radioInteraccion, out hit, distanciaInteraccion))
            return hit.collider != null && hit.collider.gameObject == gameObject;
        return false;
    }

    // Detectar si estamos cerca de un punto de entrega
    bool DetectarEntrega()
    {
        Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("EntregaToalla") || hit.CompareTag("EntregaToallaWrong"))
                return true;
        }
        return false;
    }

    // Propiedad para verificar si la toalla fue entregada (instancia)
    public bool ToallaEntregada => entregada;

    // Método para verificar si la tarea está completada (instancia)
    public bool TareaCompletada()
    {
        return entregada;
    }

    // MÉTODO ESTÁTICO para verificar si la toalla fue entregada
    public static bool TareaCompletadaStatic()
    {
        return toallaEntregadaStatic;
    }

    // 🔄 NUEVO MÉTODO: Resetear toalla a estado inicial
    public void ResetTask()
    {
        Debug.Log("🔄 Reseteando toalla...");

        recogida = false;
        entregada = false;
        cerca = false;
        cercaEntrega = false;

        // Reactivar el objeto
        gameObject.SetActive(true);

        // Restaurar transform inicial
        transform.SetParent(null);
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
        transform.localScale = escalaInicial;

        // Reactivar collider
        GetComponent<Collider>().enabled = true;

        Debug.Log("✅ Toalla reseteada a estado inicial");
    }

    // GUI para mostrar mensajes en pantalla
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
        if (cerca && !recogida)
            GUI.Label(mensaje, "Pulsa E para recoger toalla", estilo);

        // Mensaje al entregar
        if (recogida && !entregada && cercaEntrega)
            GUI.Label(mensaje, "Pulsa E para entregar toalla", estilo);
    }
}
