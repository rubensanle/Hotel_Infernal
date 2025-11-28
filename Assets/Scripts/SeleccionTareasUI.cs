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

    [Header("UI - Otros")]
    public TMP_Text puntosTexto;
    public Button btnContinuarNoche;    // Botón para confirmar y pasar a la noche

    [Header("Config")]
    public int puntosIniciales = 100;
    public int costeLuces = 10;
    public int costeVelocidad = 25;
    public int costeConductos = 35;
    public int costePerro = 35;
    public int costePerroAlimentado = 35;
    public int costeRelojes = 15;

    private int puntosRestantes;

    // Estados internos de cada tarea
    private bool lucesOn, velocidadOn, conductosOn, perroOn, perroAlimentadoOn, relojesOn;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        puntosRestantes = puntosIniciales;
        ActualizarTexto();

        // Suscribir botones de tareas
        btnLuces.onClick.AddListener(() => ToggleTarea(ref lucesOn, costeLuces, (s) => GameManager.instancia.lucesEncendidas = s, btnLuces));
        btnVelocidad.onClick.AddListener(() => ToggleTarea(ref velocidadOn, costeVelocidad, (s) => GameManager.instancia.velocidadNormalSeleccionada = s, btnVelocidad));
        btnConductos.onClick.AddListener(() => ToggleTarea(ref conductosOn, costeConductos, (s) => GameManager.instancia.conductosLimpios = s, btnConductos));
        btnPerro.onClick.AddListener(() => ToggleTarea(ref perroOn, costePerro, (s) => GameManager.instancia.perroSacado = s, btnPerro));
        btnPerroAlimentado.onClick.AddListener(() => ToggleTarea(ref perroAlimentadoOn, costePerroAlimentado, (s) => GameManager.instancia.perroAlimentado = s, btnPerroAlimentado));
        btnRelojes.onClick.AddListener(() => ToggleTarea(ref relojesOn, costeRelojes, (s) => GameManager.instancia.relojesArreglados = s, btnRelojes));

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

    // Cambiar solo el color del botón, manteniendo el texto original
    void CambiarVisualBoton(Button boton, Color colorFondo)
    {
        boton.GetComponent<Image>().color = colorFondo;
    }

    // Actualizar texto de puntos
    void ActualizarTexto()
    {
        puntosTexto.text = "" + puntosRestantes;
    }

    // Confirmar selección y cargar escena principal
    public void ConfirmarSeleccion()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Hotel");
    }

}

