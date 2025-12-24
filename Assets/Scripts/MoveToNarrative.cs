using UnityEngine;

public class MoveToNarrative : MonoBehaviour
{
    public void ConfirmarSeleccion()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("EscenaHistoria");
    }
}
