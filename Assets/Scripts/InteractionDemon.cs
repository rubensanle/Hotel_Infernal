using UnityEngine;

public class InteractionDemon : MonoBehaviour
{
    public float tiempoLimite = 60f;
    public float tiempoFinal = 90f;
    public DemonBehaviour demonio;
    public Renderer cuboRenderer;

    private float tiempoActual = 0f;
    private bool interactuado = false;
    private bool faseFinal = false;

    void Start()
    {
        tiempoActual = 0f;
        cuboRenderer.material.color = Color.white;
    }

    void Update()
    {
        if (!interactuado)
        {
            tiempoActual += Time.deltaTime;

            if (tiempoActual >= tiempoLimite && !faseFinal)
            {
                demonio.Enfadar();
            }

            if (tiempoActual >= tiempoFinal && !faseFinal)
            {
                faseFinal = true;
                demonio.Teletransportarse();
            }
        }
    }

    void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("Player"))
        {
            cuboRenderer.material.color = Color.green;

            if (!faseFinal)
            {
                interactuado = true;
                demonio.Calmar();
                Debug.Log("Cubo tocado, demonio calmado.");
            }
            else
            {
                Debug.Log("Cubo tocado, pero el demonio ya no se calma.");
            }
        }
    }

    public bool EstaEnFaseFinal()
    {
        return faseFinal;
    }
}
