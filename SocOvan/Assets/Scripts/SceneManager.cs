using Mono.Cecil;
using System.Linq.Expressions;
using UnityEngine;
using TMPro;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel = null;
    [SerializeField] private TMP_Text dialogueText = null;
    [SerializeField] private GameObject _player;

    [SerializeField] private bool hasDialogues;

    [Header("Configuración de Diálogos")]
    [SerializeField, TextArea(3, 5)] private string[] startDialogueLines;
    [SerializeField, TextArea(3, 5)] private string[] endDialogueLines;

    private int lineIndex;
    public float typingTime = 0.05f;

    private bool startDialogueNotEnded = true;
    private bool ending = true;
    private bool inputByUser = false;

    private GameObject[] Boxes;
    private GameObject[] Goals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
        Goals = GameObject.FindGameObjectsWithTag("Goal");

        var movementScript = _player.GetComponent<PlayerMovement>();

        movementScript.enabled = false;
        Debug.Log(dialoguePanel != null);

        if (hasDialogues)
        {
            StartDialogue();
        }
        else
        {
            _player.GetComponent<PlayerMovement>().enabled = true;
        }

    }

    void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        lineIndex = 0;
        StartCoroutine(ShowLine());
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        foreach (char ch in startDialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }
        if (dialogueText.text == startDialogueLines[lineIndex])
        {

            yield return new WaitForSeconds(1.0f);
            NextDialogueLine();
        }
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if (lineIndex < startDialogueLines.Length )
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            dialoguePanel.SetActive(false);

            var movementScript = _player.GetComponent<PlayerMovement>();
            movementScript.enabled = true;

            startDialogueNotEnded = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        if (hasDialogues)
        {
            if (startDialogueNotEnded)
            {
                if (Input.GetButtonDown("Fire1") && !inputByUser)
                {
                    inputByUser = true;
                    StopAllCoroutines();
                    dialogueText.text = startDialogueLines[lineIndex];
                }
                else if (Input.GetButtonDown("Fire1"))
                {
                    inputByUser = false;
                    NextDialogueLine();
                }
            }
        }

        int boxesInGoal = 0;

        foreach (var goal in Goals)
        {
            foreach (var box in Boxes)
            {
                if (goal.transform.position.x == box.transform.position.x && goal.transform.position.y == box.transform.position.y)
                {
                    boxesInGoal++; break;
                }
            }
        }

        if (boxesInGoal == Goals.Length)
        {
            if (ending)
            {
                var movementScript = _player.GetComponent<PlayerMovement>();

                movementScript.enabled = false;
                StartFinishingLine();
                ending = false;
            }
        }
    }

    void StartFinishingLine()
    {
        dialoguePanel.SetActive(true);
        lineIndex = 0;
        StartCoroutine(ShowLineFinish());
    }

    private IEnumerator ShowLineFinish()
    {
        dialogueText.text = string.Empty;
        foreach (char ch in endDialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }
        if (dialogueText.text == endDialogueLines[lineIndex])
        {
            yield return new WaitForSeconds(1.0f);
            NextFinishLine();
        }
    }

    private void NextFinishLine()
    {
        lineIndex++;
        if (lineIndex < endDialogueLines.Length)
        {
            StartCoroutine(ShowLineFinish());
        }
        else
        {
            dialoguePanel.SetActive(false);

            var movementScript = _player.GetComponent<PlayerMovement>();
            movementScript.enabled = true;
        }
    }
}
