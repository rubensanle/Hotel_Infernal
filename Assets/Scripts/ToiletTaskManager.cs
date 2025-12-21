using UnityEngine;

public class ToiletTaskManager : MonoBehaviour
{
    public Camera playerCamera;                   // Cámara del jugador
    public ToiletBehavior[] vateres;              // Array de váteres a limpiar
    private ToiletBehavior vaterActual;           // Váter que el jugador está mirando
    private bool cerca = false;                   // Si el jugador está cerca de un váter
    private bool tareaCompletada = false;         // Si todos los váteres están limpios

    private float tiempoMantener = 3f;            // Tiempo necesario para limpiar
    private float contadorMantener = 0f;          // Contador de tiempo manteniendo E
    private bool manteniendo = false;             // Si se está manteniendo E

    public PlayerMovement playerMovement;         // Referencia al jugador

    void Update()
    {
        // Si la tarea está completada, no hacer nada
        if (tareaCompletada) return;

        // Bloquear interacción si el jugador lleva un objeto
        if (playerMovement != null && playerMovement.EstaLlevandoObjeto)
        {
            cerca = false;
            vaterActual = null;
            return;
        }

        // Detectar váter con raycast
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
            ToiletBehavior toilet = hit.collider.GetComponent<ToiletBehavior>();
            if (toilet != null && !toilet.EstaLimpio)
            {
                vaterActual = toilet;
                cerca = true;
            }
            else
            {
                cerca = false;
                vaterActual = null;
            }
        }
        else
        {
            cerca = false;
            vaterActual = null;
        }

        // Interacción manteniendo E
        if (cerca && vaterActual != null)
        {
            if (Input.GetKey(KeyCode.E))
            {
                manteniendo = true;
                contadorMantener += Time.deltaTime;

                // Cuando se completa el tiempo, limpiar el váter
                if (contadorMantener >= tiempoMantener)
                {
                    vaterActual.Limpiar();
                    contadorMantener = 0f;
                    manteniendo = false;

                    // Verificar si todos los váteres están limpios
                    if (TodosLimpios())
                    {
                        tareaCompletada = true;
                        Debug.Log("✅ ¡Todos los váteres han sido limpiados! Tarea completada.");
                    }
                }
            }
            else
            {
                manteniendo = false;
                contadorMantener = 0f;
            }
        }
    }

    // Verifica si todos los váteres están limpios
    bool TodosLimpios()
    {
        foreach (ToiletBehavior toilet in vateres)
        {
            if (!toilet.EstaLimpio) return false;
        }
        return true;
    }

    // Método para verificar si la tarea está completada
    public bool TareaCompletada()
    {
        return tareaCompletada;
    }

    // 🔄 NUEVO MÉTODO: Resetear la tarea de váteres
    public void ResetTask()
    {
        Debug.Log("🔄 Reseteando tarea de váteres...");

        tareaCompletada = false;
        cerca = false;
        vaterActual = null;
        manteniendo = false;
        contadorMantener = 0f;

        // Resetear cada váter individual
        foreach (ToiletBehavior toilet in vateres)
        {
            if (toilet != null)
            {
                toilet.ResetToilet();
            }
        }

        Debug.Log("✅ Tarea de váteres reseteada");
    }

    // Mostrar mensaje de interacción en pantalla
    void OnGUI()
    {
        if (Time.timeScale == 0f)
            return;

        if (cerca && vaterActual != null && !vaterActual.EstaLimpio)
        {
            GUIStyle estilo = new GUIStyle(GUI.skin.label);
            estilo.fontSize = 40;
            estilo.normal.textColor = Color.white;
            estilo.alignment = TextAnchor.MiddleCenter;

            Rect mensaje = new Rect(Screen.width / 2 - 200, Screen.height - 120, 400, 80);

            if (manteniendo)
            {
                // Mostrar progreso de limpieza
                float progreso = contadorMantener / tiempoMantener;
                GUI.Label(mensaje, $"Limpiando váter... {progreso * 100:F0}%", estilo);
            }
            else
            {
                GUI.Label(mensaje, "Mantén E para interactuar", estilo);
            }
        }
    }
}
