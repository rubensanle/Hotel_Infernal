using UnityEngine;

public class InteractionScript : MonoBehaviour
{
    public float distancia = 2f;  
    public Color initialColor = Color.aliceBlue; 
    public Camera playerCamera; 
    private GameObject objetoInter;
    private Renderer cubeRenderer;
    private bool cerca = false;
    private float timer = 10f; 
    private int interactionCount = 0; 
    private bool isTimerStopped = false; 
    private bool isCubeAngry = false; 
    private CubeBehaviorScript cubeBehavior; 


    void Start()
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Inter");
        if (interactables.Length > 0)
        {
            objetoInter = interactables[0];
            if (objetoInter.TryGetComponent(out cubeRenderer))
            {
                cubeRenderer.material.color = initialColor;
                Debug.Log("Color inicial asignado: " + initialColor);
            }
            cubeBehavior = objetoInter.AddComponent<CubeBehaviorScript>(); 
        }
    }

    void Update()
    {
        if (playerCamera == null)
        {
            Debug.LogError("playerCamera no está asignado en el Inspector. Asigna la cámara para evitar este error.");
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, distancia))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Inter"))
            {
                objetoInter = hit.collider.gameObject;
                if (objetoInter.TryGetComponent(out cubeRenderer))
                {
                    cerca = true;
                    Debug.Log("Objeto interactuable detectado: " + objetoInter.name);
                }
                else
                {
                    cerca = false;
                    Debug.LogWarning("Objeto sin Renderer: " + objetoInter.name);
                }
            }
            else
            {
                cerca = false;
            }
        }
        else
        {
            cerca = false;
            Debug.Log("No se detectó ningún objeto");
        }

        if (!isTimerStopped && !isCubeAngry)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (interactionCount < 3)
                {
                    isCubeAngry = true;
                    cubeRenderer.material.color = Color.darkRed;
                    cubeBehavior.StartJumping(); 
                    Debug.Log("El cubo está enfadado");
                }
                timer = 0f; 
            }
        }

        if (cerca && Input.GetKeyDown(KeyCode.E))
        {
            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f); 
                Debug.Log("Color cambiado");
                if (!isCubeAngry)
                {
                    interactionCount++;
                    if (interactionCount >= 3)
                    {
                        isTimerStopped = true;
                        Debug.Log("Has interactuado 3 veces, temporizador detenido.");
                    }
                }
                else
                {
                    isCubeAngry = false;
                    cubeRenderer.material.color = initialColor;
                    cubeBehavior.StopJumping();
                    isTimerStopped = true; 
                    Debug.Log("El cubo se ha calmado");
                }
            }
        }
    }

    void OnGUI()
    {
        if (cerca)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            float x = (Screen.width - 200) / 2; 
            float y = Screen.height - 100; 
            GUI.Label(new Rect(x, y, 200, 50), "Presiona E para interactuar", style);
        }

        if (!isTimerStopped && !isCubeAngry)
        {
            GUIStyle timerStyle = new GUIStyle();
            timerStyle.fontSize = 16;
            timerStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(10, 10, 150, 30), "Tiempo: " + Mathf.Ceil(timer).ToString() + "s", timerStyle);
        }
        else if (timer <= 0)
        {
            GUIStyle timerStyle = new GUIStyle();
            timerStyle.fontSize = 16;
            timerStyle.normal.textColor = Color.red; 
            GUI.Label(new Rect(10, 10, 150, 30), "Tiempo: 0s", timerStyle);
        }
    }

    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * distancia);
        }
    }
}