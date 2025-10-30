using UnityEngine;

public class DogInteraction : MonoBehaviour
{
    private DogTimer timerScript;
    private bool playerInRange = false;

    void Start()
    {
        timerScript = GetComponent<DogTimer>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            timerScript.ResetTimer();
            Debug.Log("Interacción: Temporizador reiniciado.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
