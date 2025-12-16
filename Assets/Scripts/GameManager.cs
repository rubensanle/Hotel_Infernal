using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    public bool lucesEncendidas = false;
    public bool velocidadNormalSeleccionada = false;
    public bool conductosLimpios = false;
    public bool perroSacado = false;
    public bool perroAlimentado = false;
    public bool relojesArreglados = false;
    public bool altavocesArreglados = false;
    public int dificultadSeleccionada = 2; 


    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetGame()
    {
        lucesEncendidas = false;
        velocidadNormalSeleccionada = false;
        conductosLimpios = false;
        perroSacado = false;
        perroAlimentado = false;
        relojesArreglados = false;
        altavocesArreglados = false; 

        // Reinicia aquí cualquier otra variable global que uses
    }
}
