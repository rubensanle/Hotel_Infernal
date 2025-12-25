using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasStoryController : MonoBehaviour
{
    [Header("Canvas de la historia")]
    public GameObject[] storyCanvases;

    [Header("Efecto máquina de escribir")]
    public float typeSpeed = 0.03f;
    public AudioSource audioSource;
    public AudioClip typeSound;

    private int currentCanvas = 0;
    private int currentText = 0;

    private TextMeshProUGUI[] textsInCanvas;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private string fullCurrentText = "";

    void Start()
    {
        LoadCanvas(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                CompleteCurrentText();
            }
            else
            {
                NextTextOrCanvas();
            }
        }
    }

    void LoadCanvas(int index)
    {
        for (int i = 0; i < storyCanvases.Length; i++)
            storyCanvases[i].SetActive(i == index);

        textsInCanvas = storyCanvases[index].GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (var t in textsInCanvas)
            t.gameObject.SetActive(false);

        currentText = 0;

        ShowAndType(textsInCanvas[currentText]);
    }

    void ShowAndType(TextMeshProUGUI textUI)
    {
        foreach (var t in textsInCanvas)
            t.gameObject.SetActive(false);

        textUI.gameObject.SetActive(true);

        fullCurrentText = textUI.text;
        textUI.text = "";

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(textUI));
    }

    IEnumerator TypeText(TextMeshProUGUI textUI)
    {
        isTyping = true;

        if (typeSound != null)
        {
            audioSource.clip = typeSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        foreach (char c in fullCurrentText)
        {
            textUI.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        audioSource.Stop();

        isTyping = false;
    }

    void CompleteCurrentText()
    {
        isTyping = false;

        audioSource.Stop();

        TextMeshProUGUI textUI = textsInCanvas[currentText];

        StopCoroutine(typingCoroutine);

        textUI.text = fullCurrentText;
    }

    void NextTextOrCanvas()
    {
        currentText++;

        if (currentText < textsInCanvas.Length)
        {
            ShowAndType(textsInCanvas[currentText]);
        }
        else
        {
            currentCanvas++;

            if (currentCanvas >= storyCanvases.Length)
            {
                Debug.Log("Historia terminada");
                SceneManager.LoadScene("MainMenuControlador");
                return;
            }

            LoadCanvas(currentCanvas);
        }
    }

}
