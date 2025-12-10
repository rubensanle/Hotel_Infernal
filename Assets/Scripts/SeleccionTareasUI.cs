using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeleccionTareasUI : MonoBehaviour
{
    [Header("UI - Botones de tareas")]
    public Button btnLuces;
    public Button btnVelocidad;
    public Button btnConductos;
    public Button btnPerro;             // Sacar al perro
    public Button btnPerroAlimentado;   // Alimentar al perro
    public Button btnRelojes;           // Arreglar relojes
    public Button btnAltavoces;         // NUEVO: arreglar altavoces

    [Header("UI - Otros")]
    public TMP_Text puntosTexto;
    public Button btnContinuarNoche;    // Botón para confirmar y pasar a la noche
    public TMP_Dropdown dificultadDropdown; // 🔄 NUEVO: Dropdown de dificultad

    [Header("Config")]
    public int puntosIniciales = 100;
    public int costeLuces = 10;
    public int costeVelocidad = 25;
    public int costeConductos = 35;
    public int costePerro = 35;
    public int costePerroAlimentado = 35;
    public int costeRelojes = 15;
    public int costeAltavoces = 15;     // NUEVO

    private int puntosRestantes;

    // Estados internos de cada tarea
    private bool lucesOn, velocidadOn, conductosOn, perroOn, perroAlimentadoOn, relojesOn, altavocesOn;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 🔄 Suscribir evento del dropdown
        if (dificultadDropdown != null)
            dificultadDropdown.onValueChanged.AddListener(OnDifficultyChanged);

        // Aplicar dificultad inicial
        OnDifficultyChanged(dificultadDropdown != null ? dificultadDropdown.value : 2); // por defecto Difícil

        ActualizarTexto();

        // Suscribir botones de tareas
        btnLuces.onClick.AddListener(() => ToggleTarea(ref lucesOn, costeLuces, (s) => GameManager.instancia.lucesEncendidas = s, btnLuces));
        btnVelocidad.onClick.AddListener(() => ToggleTarea(ref velocidadOn, costeVelocidad, (s) => GameManager.instancia.velocidadNormalSeleccionada = s, btnVelocidad));
        btnConductos.onClick.AddListener(() => ToggleTarea(ref conductosOn, costeConductos, (s) => GameManager.instancia.conductosLimpios = s, btnConductos));
        btnPerro.onClick.AddListener(() => ToggleTarea(ref perroOn, costePerro, (s) => GameManager.instancia.perroSacado = s, btnPerro));
        btnPerroAlimentado.onClick.AddListener(() => ToggleTarea(ref perroAlimentadoOn, costePerroAlimentado, (s) => GameManager.instancia.perroAlimentado = s, btnPerroAlimentado));
        btnRelojes.onClick.AddListener(() => ToggleTarea(ref relojesOn, costeRelojes, (s) => GameManager.instancia.relojesArreglados = s, btnRelojes));
        btnAltavoces.onClick.AddListener(() => ToggleTarea(ref altavocesOn, costeAltavoces, (s) => GameManager.instancia.altavocesArreglados = s, btnAltavoces));

        // Suscribir botón de continuar
        btnContinuarNoche.onClick.AddListener(ConfirmarSeleccion);
    }

    // Método genérico para alternar tareas con botones
    void ToggleTarea(ref bool estado, int coste, System.Action<bool> aplicarEstado, Button boton)
    {
        if (!estado) // Activar tarea
        {
            if (puntosRestantes >= coste)
            {
                puntosRestantes -= coste;
                estado = true;
                aplicarEstado(true);
                CambiarVisualBoton(boton, Color.green); // verde activado
            }
            else
            {
                Debug.Log("No tienes suficientes puntos.");
            }
        }
        else // Desactivar tarea
        {
            puntosRestantes += coste;
            estado = false;
            aplicarEstado(false);
            CambiarVisualBoton(boton, Color.red); // rojo desactivado
        }

        ActualizarTexto();
    }

    void CambiarVisualBoton(Button boton, Color colorFondo)
    {
        boton.GetComponent<Image>().color = colorFondo;
    }

    void ActualizarTexto()
    {
        puntosTexto.text = "" + puntosRestantes;
    }

    // 🔄 NUEVO: Cambiar puntos según dificultad
    void OnDifficultyChanged(int index)
    {
        // Ajustar puntos iniciales según dificultad
        switch (index)
        {
            case 0: // Fácil
                puntosIniciales = 180;
                break;
            case 1: // Medio
                puntosIniciales = 125;
                break;
            case 2: // Difícil
                puntosIniciales = 90;
                break;
        }

        puntosRestantes = puntosIniciales;

        lucesOn = velocidadOn = conductosOn = perroOn = perroAlimentadoOn = relojesOn = altavocesOn = false;

        GameManager.instancia.lucesEncendidas = false;
        GameManager.instancia.velocidadNormalSeleccionada = false;
        GameManager.instancia.conductosLimpios = false;
        GameManager.instancia.perroSacado = false;
        GameManager.instancia.perroAlimentado = false;
        GameManager.instancia.relojesArreglados = false;
        GameManager.instancia.altavocesArreglados = false;

        CambiarVisualBoton(btnLuces, Color.red);
        CambiarVisualBoton(btnVelocidad, Color.red);
        CambiarVisualBoton(btnConductos, Color.red);
        CambiarVisualBoton(btnPerro, Color.red);
        CambiarVisualBoton(btnPerroAlimentado, Color.red);
        CambiarVisualBoton(btnRelojes, Color.red);
        CambiarVisualBoton(btnAltavoces, Color.red);

        // Actualizar texto de puntos
        ActualizarTexto();
    }


    public void ConfirmarSeleccion()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Hotel");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

