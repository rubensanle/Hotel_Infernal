using UnityEngine;

public class InteractionScript : MonoBehaviour
{
    public float distancia = 2f; 
    public Color initialColor = Color.yellow; 
    public Camera playerCamera; 
    private GameObject objetoInter;
    private Renderer cubeRenderer;
    private bool cerca = false;

    void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, distancia))
        {
            if (hit.collider.CompareTag("Inter"))
            {
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = initialColor;
                    Debug.Log("Color inicial asignado: " + initialColor);
                }
            }
        }
    }

    void Update()
    {
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

        if (cerca && Input.GetKeyDown(KeyCode.E))
        {
            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f); 
                Debug.Log("Color cambiado a un color aleatorio");
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
            GUI.Label(new Rect(x, y, 200, 50), "Press E to interact", style);
            Debug.Log("Mostrando mensaje 'Press E to interact' en posición: " + x + ", " + y);
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