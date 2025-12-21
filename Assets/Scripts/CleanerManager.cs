using UnityEngine;
using System.Collections.Generic;

// Administrador que controla la limpieza de varios cubos "limpiables"
public class CleanerManager : MonoBehaviour
{
    public Camera playerCamera;                   // Cámara del jugador
    public GameObject[] cubosLimpiables;          // Lista de cubos limpiables
    public PlayerMovement playerMovement;         // Referencia al jugador

    private Dictionary<GameObject, int> interacciones = new Dictionary<GameObject, int>(); // Conteo de interacciones
    private GameObject cuboActual;                // Cubo que el jugador está mirando
    private bool cerca = false;                   // Si el jugador está cerca mirando un cubo
    private bool limpiezaCompletada = false;      // Si todas las tareas de limpieza han terminado

    void Start()
    {
        // Inicializa cada cubo
        foreach (GameObject cubo in cubosLimpiables)
        {
            cubo.SetActive(true);
            interacciones[cubo] = 0;

            // Asegurar que cada cubo tenga el comportamiento de limpieza
            if (!cubo.TryGetComponent(out CleanerBehavior behavior))
                cubo.AddComponent<CleanerBehavior>();
        }
    }

    void Update()
    {
        if (limpiezaCompletada) return;

        // Bloqueo: si el jugador lleva un objeto, no puede limpiar
        if (playerMovement != null && playerMovement.EstaLlevandoObjeto)
        {
            cerca = false;
            cuboActual = null;
            return;
        }

        // Detectar cubo con raycast
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("Cleanable"))
            {
                cuboActual = hit.collider.gameObject;
                cerca = true;
            }
            else
            {
                cerca = false;
            }
        }
        else
        {
            cerca = false;
        }

        // Interactuar con cubo
        if (cerca && Input.GetKeyDown(KeyCode.E) && cuboActual != null)
        {
            if (interacciones.ContainsKey(cuboActual))
            {
                interacciones[cuboActual]++;
                Debug.Log("Cubo " + cuboActual.name + ": " + interacciones[cuboActual] + " interacciones.");

                // Después de 3 interacciones, desactivar el cubo
                if (interacciones[cuboActual] >= 3)
                {
                    cuboActual.SetActive(false);
                    interacciones[cuboActual] = 0;
                }

                // Verificar si todos los cubos están desactivados
                if (TodosCubosDesactivados())
                {
                    limpiezaCompletada = true;
                    Debug.Log("Tarea de limpieza completada.");
                }
            }
        }
    }

    // Revisar si todos los cubos están desactivados
    bool TodosCubosDesactivados()
    {
        foreach (GameObject cubo in cubosLimpiables)
        {
            if (cubo.activeSelf)
                return false;
        }
        return true;
    }

    // Propiedad para verificar si la limpieza está completada
    public bool LimpiezaCompletada => limpiezaCompletada;

    // Método para verificar si la tarea está completada
    public bool TareaCompletada()
    {
        return limpiezaCompletada;
    }

    // 🔄 NUEVO MÉTODO: Resetear tarea de limpieza
    public void ResetTask()
    {
        Debug.Log("🔄 Reseteando tarea de limpieza...");

        limpiezaCompletada = false;
        cerca = false;
        cuboActual = null;

        // Resetear cada cubo
        foreach (GameObject cubo in cubosLimpiables)
        {
            if (cubo != null)
            {
                cubo.SetActive(true);
                interacciones[cubo] = 0;

                // Resetear el comportamiento de limpieza
                CleanerBehavior behavior = cubo.GetComponent<CleanerBehavior>();
                if (behavior != null)
                {
                    behavior.ResetCleaner();
                }
            }
        }

        Debug.Log("✅ Tarea de limpieza reseteada");
    }

    // GUI para mostrar mensaje en pantalla
    void OnGUI()
    {
        if (Time.timeScale == 0f)
            return;

        if (cerca && !limpiezaCompletada && cuboActual != null)
        {
            GUIStyle estilo = new GUIStyle(GUI.skin.label);
            estilo.fontSize = 40;
            estilo.normal.textColor = Color.white;
            estilo.alignment = TextAnchor.MiddleCenter;
            Rect mensaje = new Rect(Screen.width / 2 - 200, Screen.height - 120, 400, 80);
            GUI.Label(mensaje, "Pulsa E para interaccionar", estilo);
        }
    }
}
