using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class menudiario : MonoBehaviour
{
    [Header("Personal System")]
    public int currentPersonal = 90;
    public TMP_Text personalText;
    public int[] taskPersonalCost;

    [Header("UI Panels")]
    public GameObject confirmPopup;
    public GameObject[] minigamePanels;
    public GameObject[] taskCheckmarks;
    public GameObject pocopersonalpopup;

    [Header("Dificultad")]
    public TMP_Dropdown dificultadDropdown; // referencia al dropdown en el inspector

    private int selectedTask = -1;

    void Start()
    {
        // Inicializar UI
        UpdateEnergyUI();
        confirmPopup.SetActive(false);
        pocopersonalpopup.SetActive(false);

        foreach (var mini in minigamePanels)
            mini.SetActive(false);

        // Conectar evento del dropdown
        if (dificultadDropdown != null)
            dificultadDropdown.onValueChanged.AddListener(OnDifficultyChanged);

        // Aplicar dificultad inicial
        OnDifficultyChanged(dificultadDropdown.value);
    }

    void UpdateEnergyUI()
    {
        personalText.text = "Personal: " + currentPersonal;
    }

    public void OnTaskClicked(int taskIndex)
    {
        if (currentPersonal < taskPersonalCost[taskIndex])
        {
            pocopersonalpopup.SetActive(true);
            return;
        }

        selectedTask = taskIndex;
        confirmPopup.SetActive(true);
    }

    public void CancelTask()
    {
        confirmPopup.SetActive(false);
        selectedTask = -1;
    }

    public void StartTask()
    {
        currentPersonal -= taskPersonalCost[selectedTask];
        UpdateEnergyUI();

        confirmPopup.SetActive(false);
        minigamePanels[selectedTask].SetActive(true);
    }

    public void CompleteMinigame()
    {
        minigamePanels[selectedTask].SetActive(false);
        taskCheckmarks[selectedTask].SetActive(true);

        selectedTask = -1;
    }

    public void OnDifficultyChanged(int index)
    {
        switch (index)
        {
            case 0: // Fácil
                currentPersonal = 180;
                break;
            case 1: // Medio
                currentPersonal = 125;
                break;
            case 2: // Difícil
                currentPersonal = 90;
                break;
        }

        UpdateEnergyUI();
    }
}


