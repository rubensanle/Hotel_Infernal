using UnityEngine;

public class PatitoPickup : MonoBehaviour
{
    public PlayerMovement playerMovement;         // Movimiento del jugador
    public Transform patitoAnchor;                // Punto donde se sujeta el patito
    public GameObject patitoVisualPrefab;         // Prefab visual que aparece en la bañera
    public Transform puntoColocacion;             // Lugar exacto de colocación
    public Camera camaraJugador;                  // Cámara del jugador
    public float radioInteraccion = 0.5f;         // Radio del SphereCast
    public float distanciaInteraccion = 3.5f;     // Distancia máxima de interacción

    [Header("UI Visual del patito")]
    public GameObject patitoUI;                   // Imagen PNG en el Canvas que simula el patito

    private bool recogido = false;                // Si el patito ya fue recogido
    private bool entregado = false;               // Si ya se entregó
    private bool cerca = false;                   // Si estamos mirando el patito
    private bool cercaBanera = false;             // Si estamos cerca de la bañera

    // 🔄 NUEVO: Guardar transform inicial
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private Vector3 escalaInicial;

    // Variables estáticas para contar todos los patitos
    public static int patitosEntregados = 0;
    public static int totalPatitos = 3;

    [Header("Sonido del patito")]
    public PlayerSounds playerSounds; // referencia al script de sonidos del jugador


    void Start()
    {
        // Guardar transform inicial
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
        escalaInicial = transform.localScale;

        // Contar este patito en el total
        if (patitoUI != null)
            patitoUI.SetActive(false); // aseguramos que empieza oculto
    }

    void Update()
    {
        // Detectar patito para recoger
        cerca = DetectarPatito();

        // Recoger patito si no está recogido y el jugador no lleva objeto
        if (!recogido && !playerMovement.EstaLlevandoObjeto && cerca && Input.GetKeyDown(KeyCode.E))
        {
            // 🔊 reproducir gruñido aleatorio
            if (playerSounds != null)
                playerSounds.PlayGruñido();

            recogido = true;
            playerMovement.LlevarObjeto(true, false);

            // Mostrar imagen PNG en pantalla
            if (patitoUI != null)
                patitoUI.SetActive(true);

            // Ocultar solo el modelo visual (no el GameObject entero)
            foreach (var r in GetComponentsInChildren<MeshRenderer>())
                r.enabled = false;

            GetComponent<Collider>().enabled = false;

            Debug.Log("Patito recogido");
        }

        // Detectar bañera para entregar
        if (recogido && !entregado)
        {
            cercaBanera = DetectarBanera();

            // Entregar patito en la bañera
            if (cercaBanera && Input.GetKeyDown(KeyCode.E))
            {
                entregado = true;
                patitosEntregados++; // Contar patito entregado
                playerMovement.SoltarObjeto();

                // Ocultar imagen PNG
                if (patitoUI != null)
                    patitoUI.SetActive(false);

                // Instanciar prefab visual en la bañera
                if (patitoVisualPrefab != null && puntoColocacion != null)
                {
                    Vector3 offset = new Vector3(0, 0.5f, 0);
                    Instantiate(patitoVisualPrefab, puntoColocacion.position + offset, puntoColocacion.rotation);
                }

                Debug.Log("Patito entregado");
            }
        }
    }

    // Detectar si el jugador está mirando este patito
    bool DetectarPatito()
    {
        Ray ray = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, radioInteraccion, out hit, distanciaInteraccion))
            return hit.collider != null && hit.collider.gameObject == gameObject;
        return false;
    }

    // Detectar si el jugador está cerca de la bañera
    bool DetectarBanera()
    {
        Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Bañera"))
                return true;
        }
        return false;
    }

    // Propiedad para verificar si este patito fue entregado
    public bool PatitoEntregado => entregado;

    // Método estático para verificar si todos los patitos fueron entregados
    public static bool TareaCompletada()
    {
        return patitosEntregados >= totalPatitos;
    }

    // 🔄 NUEVO MÉTODO: Resetear patito a estado inicial
    public void ResetTask()
    {
        Debug.Log("🔄 Reseteando patito...");

        recogido = false;
        entregado = false;
        cerca = false;
        cercaBanera = false;

        // Restaurar transform inicial
        transform.SetParent(null);
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
        transform.localScale = escalaInicial;

        // Reactivar renderers y collider
        foreach (var r in GetComponentsInChildren<MeshRenderer>())
            r.enabled = true;

        GetComponent<Collider>().enabled = true;

        // Ocultar UI
        if (patitoUI != null)
            patitoUI.SetActive(false);

        Debug.Log("✅ Patito reseteado a estado inicial");
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
            GUI.Label(mensaje, "Pulsa E para recoger patito", estilo);

        // Mensaje al entregar
        if (recogido && !entregado && cercaBanera)
            GUI.Label(mensaje, "Pulsa E para entregar patito", estilo);
    }
}


