using TMPro;
using UnityEngine;

// Gestiona el tiempo general, las tareas y el comportamiento del demonio
public class GameTaskManager : MonoBehaviour
{
    [Header("Referencias Tareas")]
    public BedTaskManager bedTaskManager;         // Manager de la tarea de la cama
    public DemonBehaviour demonBehaviour;         // Comportamiento del demonio

    [Header("Interfaz de usuario")]
    public GameObject juegoFinalizadoCanvas;      // Canvas de fin de noche
    public TMP_Text textoFinal;                   // Texto del resumen final
    public GameOverUITMP interfazGameOver;        // Canvas de derrota (solo si te mata el demonio)
    public GameObject Indice;                     // Canvas del checklist
    public GameObject TelefonoCanvas;
    public GameObject VentiladorCanvas;
    public GameObject TermometroCanvas;

    [Header("Temporizadores")]
    public float tiempoDemonio = 210f;            // 3.5 minutos para tareas del demonio
    public float tiempoGeneral = 480f;            // 8 minutos para todas las tareas

    // Variables internas de tiempo
    private float tiempoRestanteDemonio;
    private float tiempoRestanteGeneral;
    private bool persecucionActivada = false;     // Si el demonio está en persecución suave
    private bool modoMatarActivado = false;       // Si el demonio está en modo matar
    private bool demonioCalmado = false;          // Si el demonio fue calmado
    private bool juegoGanado = false;             // Si se finaliza la noche (por tareas o tiempo)
    private bool juegoPerdido = false;            // Solo si el demonio te mata

    [Header("Puntuación tareas (20 puntos por tarea completada)")]
    // No acumulamos puntTotal en tiempo real; lo calculamos al final para evitar dobles sumas
    private int puntTotal = 0;
    public int tar1 = 20;   // Latas
    public int tar2 = 20;   // Cama
    public int tar3 = 20;   // Toalla
    public int tar4 = 20;   // Patito
    public int tar5 = 20;   // Limpieza
    public int tar6 = 20;   // Váteres
    public int tar7 = 20;   // Grifos
    public int tar8 = 20;   // Cuadros
    public int tar9 = 20;   // Lámparas
    public int tar10 = 20;  // Teléfono
    public int tar11 = 20;  // Termómetro
    public int tar12 = 20;  // Ventilador

    void Start()
    {
        tiempoRestanteDemonio = tiempoDemonio;
        tiempoRestanteGeneral = tiempoGeneral;

        if (juegoFinalizadoCanvas != null)
            juegoFinalizadoCanvas.SetActive(false);
    }

    void Update()
    {
        if (juegoGanado || juegoPerdido) return;

        tiempoRestanteDemonio -= Time.deltaTime;
        tiempoRestanteGeneral -= Time.deltaTime;

        // Fin por completar todas las tareas
        if (TodasLasTareasCompletadas() && !juegoGanado)
        {
            juegoGanado = true;
            MostrarFinalNoche();
            return;
        }

        // Fin por tiempo (no derrota): se muestra fin de noche igualmente
        if (tiempoRestanteGeneral <= 0f && !juegoGanado)
        {
            juegoGanado = true;
            MostrarFinalNoche();
            return;
        }

        // Lógica del demonio (solo si no está calmado y no se ha finalizado la noche)
        if (!demonioCalmado && !juegoGanado)
        {
            VerificarTareasDemonio();
        }
    }

    // Verifica las tareas específicas del demonio (cama y latas)
    void VerificarTareasDemonio()
    {
        bool latasOk = TrashPickUp.TareaCompletada();
        bool camaOk = bedTaskManager != null && bedTaskManager.TareaCompletada();
        bool tareasDemonioCompletas = latasOk && camaOk;

        if (tareasDemonioCompletas && !demonioCalmado)
        {
            demonioCalmado = true;
            if (demonBehaviour != null)
                demonBehaviour.Calmar();
        }

        if (!tareasDemonioCompletas && !demonioCalmado)
        {
            if (tiempoRestanteDemonio <= 60f && !persecucionActivada)
            {
                demonBehaviour.ActivarPersecucionSuave();
                persecucionActivada = true;
            }

            if (tiempoRestanteDemonio <= 0f && !modoMatarActivado)
            {
                demonBehaviour.ActivarModoMatar();
                modoMatarActivado = true;
            }
        }
    }

    // Verifica si las 12 tareas están completadas
    bool TodasLasTareasCompletadas()
    {
        bool latasOk = TrashPickUp.TareaCompletada();
        if (!latasOk) return false;

        bool camaOk = bedTaskManager != null && bedTaskManager.TareaCompletada();
        if (!camaOk) return false;

        bool toallaOk = ToallaPickup.TareaCompletadaStatic();
        if (!toallaOk) return false;

        bool patitoOk = PatitoPickup.TareaCompletada();
        if (!patitoOk) return false;

        CleanerManager limpieza = FindFirstObjectByType<CleanerManager>();
        bool limpiezaOk = limpieza != null && limpieza.TareaCompletada();
        if (!limpiezaOk) return false;

        ToiletTaskManager vateres = FindFirstObjectByType<ToiletTaskManager>();
        bool vateresOk = vateres != null && vateres.TareaCompletada();
        if (!vateresOk) return false;

        FaucetTaskManager grifos = FindFirstObjectByType<FaucetTaskManager>();
        bool grifosOk = grifos != null && grifos.TareaCompletada();
        if (!grifosOk) return false;

        FrameTaskManager cuadros = FindFirstObjectByType<FrameTaskManager>();
        bool cuadrosOk = cuadros != null && cuadros.TareaCompletada();
        if (!cuadrosOk) return false;

        LampTaskManager lamparas = FindFirstObjectByType<LampTaskManager>();
        bool lamparasOk = lamparas != null && lamparas.TareaCompletada();
        if (!lamparasOk) return false;

        TelefonoInteract telefono = FindFirstObjectByType<TelefonoInteract>();
        bool telefonoOk = telefono != null && telefono.TareaCompletada();
        if (!telefonoOk) return false;

        TermometroInteract termometro = FindFirstObjectByType<TermometroInteract>();
        bool termometroOk = termometro != null && termometro.TareaCompletada();
        if (!termometroOk) return false;

        VentiladorInteract ventilador = FindFirstObjectByType<VentiladorInteract>();
        bool ventiladorOk = ventilador != null && ventilador.TareaCompletada();
        if (!ventiladorOk) return false;

        return true;
    }

    // Solo cuenta cuántas tareas están completadas (sin sumar puntos ni modificar estado)
    int ContarTareasCompletadas()
    {
        int contador = 0;

        if (TrashPickUp.TareaCompletada()) contador++;
        if (bedTaskManager != null && bedTaskManager.TareaCompletada()) contador++;
        if (ToallaPickup.TareaCompletadaStatic()) contador++;
        if (PatitoPickup.TareaCompletada()) contador++;

        CleanerManager limpieza = FindFirstObjectByType<CleanerManager>();
        if (limpieza != null && limpieza.TareaCompletada()) contador++;

        ToiletTaskManager vateres = FindFirstObjectByType<ToiletTaskManager>();
        if (vateres != null && vateres.TareaCompletada()) contador++;

        FaucetTaskManager grifos = FindFirstObjectByType<FaucetTaskManager>();
        if (grifos != null && grifos.TareaCompletada()) contador++;

        FrameTaskManager cuadros = FindFirstObjectByType<FrameTaskManager>();
        if (cuadros != null && cuadros.TareaCompletada()) contador++;

        LampTaskManager lamparas = FindFirstObjectByType<LampTaskManager>();
        if (lamparas != null && lamparas.TareaCompletada()) contador++;

        TelefonoInteract telefono = FindFirstObjectByType<TelefonoInteract>();
        if (telefono != null && telefono.TareaCompletada()) contador++;

        TermometroInteract termometro = FindFirstObjectByType<TermometroInteract>();
        if (termometro != null && termometro.TareaCompletada()) contador++;

        VentiladorInteract ventilador = FindFirstObjectByType<VentiladorInteract>();
        if (ventilador != null && ventilador.TareaCompletada()) contador++;

        return contador;
    }

    // Calcula la puntuación total según las tareas completadas (sin alterar estado)
    int CalcularPuntuacionTotal()
    {
        int puntos = 0;

        if (TrashPickUp.TareaCompletada()) puntos += tar1;
        if (bedTaskManager != null && bedTaskManager.TareaCompletada()) puntos += tar2;
        if (ToallaPickup.TareaCompletadaStatic()) puntos += tar3;
        if (PatitoPickup.TareaCompletada()) puntos += tar4;

        CleanerManager limpieza = FindFirstObjectByType<CleanerManager>();
        if (limpieza != null && limpieza.TareaCompletada()) puntos += tar5;

        ToiletTaskManager vateres = FindFirstObjectByType<ToiletTaskManager>();
        if (vateres != null && vateres.TareaCompletada()) puntos += tar6;

        FaucetTaskManager grifos = FindFirstObjectByType<FaucetTaskManager>();
        if (grifos != null && grifos.TareaCompletada()) puntos += tar7;

        FrameTaskManager cuadros = FindFirstObjectByType<FrameTaskManager>();
        if (cuadros != null && cuadros.TareaCompletada()) puntos += tar8;

        LampTaskManager lamparas = FindFirstObjectByType<LampTaskManager>();
        if (lamparas != null && lamparas.TareaCompletada()) puntos += tar9;

        TelefonoInteract telefono = FindFirstObjectByType<TelefonoInteract>();
        if (telefono != null && telefono.TareaCompletada()) puntos += tar10;

        TermometroInteract termometro = FindFirstObjectByType<TermometroInteract>();
        if (termometro != null && termometro.TareaCompletada()) puntos += tar11;

        VentiladorInteract ventilador = FindFirstObjectByType<VentiladorInteract>();
        if (ventilador != null && ventilador.TareaCompletada()) puntos += tar12;

        return puntos;
    }

    // Muestra el canvas de fin de noche con la puntuación
    void MostrarFinalNoche()
    {
        Time.timeScale = 0f;

        if (juegoFinalizadoCanvas != null)
            juegoFinalizadoCanvas.SetActive(true);

        // Calculamos la puntuación en el momento del final
        puntTotal = CalcularPuntuacionTotal();

        int totalTareas = 12;
        int tareasCompletadas = ContarTareasCompletadas();
        int maxPuntos = tar1 + tar2 + tar3 + tar4 + tar5 + tar6 + tar7 + tar8 + tar9 + tar10 + tar11 + tar12;

        string mensaje = $"Has pasado la noche\nTareas completadas: {tareasCompletadas}/{totalTareas}\nHas conseguido un total de {puntTotal} puntos de {maxPuntos}";

        if (textoFinal != null)
            textoFinal.text = mensaje;

        Debug.Log(mensaje);
    }

    // Derrota solo si te mata el demonio
    public void MostrarDerrota(string motivo)
    {
        juegoPerdido = true;
        Time.timeScale = 0f;
        if (interfazGameOver != null)
            interfazGameOver.ShowGameOverMessage();
        Debug.Log("DERROTA: " + motivo);
    }

    public void JugadorMuertoPorDemonio()
    {
        if (!juegoGanado && !juegoPerdido)
            MostrarDerrota("El demonio te atrapó");
    }

    // Reset general
    public void ResetAllTasks()
    {
        Debug.Log("🔄 Reiniciando todas las tareas...");

        // 1. Reset de estáticos
        TrashPickUp.ResetearContador();
        ToallaPickup.toallaEntregadaStatic = false;
        PatitoPickup.patitosEntregados = 0; // totalPatitos se recalcula en Start de cada instancia

        // 2. Reset de banderas internas
        juegoGanado = false;
        juegoPerdido = false;
        demonioCalmado = false;
        persecucionActivada = false;
        modoMatarActivado = false;

        // 3. Temporizadores
        tiempoRestanteDemonio = tiempoDemonio;
        tiempoRestanteGeneral = tiempoGeneral;

        // 4. Puntuación (limpia)
        puntTotal = 0;

        Debug.Log("✅ Todas las tareas y puntuación reiniciadas");
    }

    // HUD superior
    void OnGUI()
    {
        if (Time.timeScale == 0f) return;

        // Si algún Canvas de interacción está activo, no mostrar GUI
        if ((Indice != null && Indice.activeSelf) ||
            (TelefonoCanvas != null && TelefonoCanvas.activeSelf) ||
            (VentiladorCanvas != null && VentiladorCanvas.activeSelf) ||
            (TermometroCanvas != null && TermometroCanvas.activeSelf))
        {
            return;
        }

        GUIStyle estiloTexto = new GUIStyle(GUI.skin.label)
        {
            fontSize = 34,
            normal = { textColor = Color.white },
            alignment = TextAnchor.UpperLeft
        };

        float anchoBarra = 520f;
        float altoBarra = 45f;
        float x = 35f;
        float y = 55f;

        if (GameManager.instancia != null)
        {
            int tareasCompletadas = ContarTareasCompletadas();
            int totalTareas = 12;

            GUIStyle estiloContador = new GUIStyle(GUI.skin.label)
            {
                fontSize = 36,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.UpperCenter,
                fontStyle = FontStyle.Bold
            };

            string textoContador = $"Tareas: {tareasCompletadas}/{totalTareas}";
            GUI.Label(new Rect(Screen.width / 2 - 150, 20, 300, 50), textoContador, estiloContador);

            if (GameManager.instancia.relojesArreglados)
            {
                // Barra tiempo general
                GUI.color = Color.gray;
                GUI.DrawTexture(new Rect(x, y, anchoBarra, altoBarra), Texture2D.whiteTexture);

                float porcentajeGeneral = Mathf.Clamp01(tiempoRestanteGeneral / tiempoGeneral);
                GUI.color = juegoGanado ? Color.green : Color.cyan;
                GUI.DrawTexture(new Rect(x, y, anchoBarra * porcentajeGeneral, altoBarra), Texture2D.whiteTexture);

                string textoGeneral = juegoGanado ? "¡NOCHE FINALIZADA!" : $"Tiempo total: {Mathf.Max(0f, tiempoRestanteGeneral):F0}s";
                GUI.color = juegoGanado ? Color.green : Color.white;
                GUI.Label(new Rect(x, y - 45, anchoBarra, 45), textoGeneral, estiloTexto);

                // Barra tiempo demonio
                if (!demonioCalmado && !juegoGanado)
                {
                    float tiempoMostrar = Mathf.Max(0f, tiempoRestanteDemonio);
                    float porcentajeDemonio = Mathf.Clamp01(tiempoMostrar / tiempoDemonio);

                    GUI.color = Color.gray;
                    GUI.DrawTexture(new Rect(x, y + 60, anchoBarra, 25), Texture2D.whiteTexture);

                    if (tiempoMostrar <= 0f || modoMatarActivado)
                        GUI.color = Color.red;
                    else if (persecucionActivada)
                        GUI.color = new Color(1f, 0.5f, 0f);
                    else
                        GUI.color = new Color(1f, 0.7f, 0.2f);

                    GUI.DrawTexture(new Rect(x, y + 60, anchoBarra * porcentajeDemonio, 25), Texture2D.whiteTexture);

                    GUIStyle estiloDemonio = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 28,
                        normal = { textColor = Color.white },
                        alignment = TextAnchor.UpperLeft,
                        fontStyle = FontStyle.Bold
                    };

                    string textoDemonio = tiempoMostrar <= 0f ? "PELIGRO: Demonio desatado!" : $"Demonio: {tiempoMostrar:F0}s / 210s";
                    GUI.Label(new Rect(x, y + 90, anchoBarra, 35), textoDemonio, estiloDemonio);
                }

                string estadoDemonio = demonioCalmado ? "DEMONIO: CALMADO" :
                                      (modoMatarActivado ? "DEMONIO: MODO MATAR" :
                                      (persecucionActivada ? "DEMONIO: ENFADADO" : "DEMONIO: TRANQUILO"));
                GUI.Label(new Rect(x, y + 130, anchoBarra, 45), estadoDemonio, estiloTexto);

                if (juegoGanado)
                    GUI.Label(new Rect(x, y + 180, anchoBarra, 45), "NOCHE COMPLETADA", estiloTexto);
            }
        }
    }
}
