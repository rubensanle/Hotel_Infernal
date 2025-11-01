using UnityEngine;
using System.Collections.Generic;

public class InteractionScript : MonoBehaviour
{
    public float distancia = 2f;
    public Camera playerCamera;
    private GameObject objetoInter;
    private Renderer cubeRenderer;
    private CubeBehaviorScript cubeBehavior;
    private bool cerca = false;

    private float timer = 60f; 
    private bool isTimerStopped = false;

    private class CubeState
    {
        public Color initialColor;
        public int interactionCount = 0;
        public bool isCubeAngry = false;
    }

    private Dictionary<GameObject, CubeState> cubeStates = new Dictionary<GameObject, CubeState>();

    void Start()
    {
        if (playerCamera == null)
            Debug.LogError("Asigna la cámara del jugador en el inspector.");

        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Inter");

        foreach (GameObject inter in interactables)
        {
            if (inter.TryGetComponent(out Renderer rend))
            {
                Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                rend.material.color = color;

                cubeStates[inter] = new CubeState
                {
                    initialColor = color
                };

                if (!inter.TryGetComponent(out CubeBehaviorScript behavior))
                    inter.AddComponent<CubeBehaviorScript>();
            }
        }
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, distancia))
        {
            if (hit.collider.CompareTag("Inter"))
            {
                objetoInter = hit.collider.gameObject;
                cubeRenderer = objetoInter.GetComponent<Renderer>();
                cubeBehavior = objetoInter.GetComponent<CubeBehaviorScript>();
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

        if (!isTimerStopped)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0;
                isTimerStopped = true;

                foreach (var pair in cubeStates)
                {
                    GameObject cube = pair.Key;
                    CubeState state = pair.Value;

                    if (state.interactionCount < 2 && !state.isCubeAngry)
                    {
                        state.isCubeAngry = true;
                        cube.GetComponent<Renderer>().material.color = Color.red;
                        cube.GetComponent<CubeBehaviorScript>().StartJumping();
                        Debug.Log($"El cubo {cube.name} se ha enfadado!");
                    }
                }
            }
        }

        if (cerca && Input.GetKeyDown(KeyCode.E))
        {
            if (cubeRenderer != null && cubeStates.ContainsKey(objetoInter))
            {
                CubeState state = cubeStates[objetoInter];

                if (!state.isCubeAngry)
                {
                    cubeRenderer.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    state.interactionCount++;

                    Debug.Log($"Cubo {objetoInter.name}: {state.interactionCount} interacciones.");

                    bool allCubesDone = true;
                    foreach (var s in cubeStates.Values)
                    {
                        if (s.interactionCount < 3)
                        {
                            allCubesDone = false;
                            break;
                        }
                    }

                    if (allCubesDone)
                    {
                        isTimerStopped = true;
                        Debug.Log("Todos los cubos fueron interactuados 3 veces. Temporizador detenido.");
                    }
                }
                else
                {
                    // Calmar cubo
                    state.isCubeAngry = false;
                    cubeRenderer.material.color = state.initialColor;
                    cubeBehavior.StopJumping();
                    Debug.Log($"Cubo {objetoInter.name} se ha calmado.");
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

        GUIStyle timerStyle = new GUIStyle();
        timerStyle.fontSize = 16;
        timerStyle.normal.textColor = (timer > 0) ? Color.white : Color.red;
        GUI.Label(new Rect(10, 10, 250, 30), "Tiempo: " + Mathf.Ceil(timer).ToString() + "s", timerStyle);
    }

    public float GetRemainingTime() => timer;
    public bool IsTimerStopped() => isTimerStopped;

}

