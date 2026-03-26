using Mono.Cecil;
using System.Linq.Expressions;
using UnityEngine;
using TMPro;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject _player;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;
    private int lineIndex;
    public float typingTime = 0.05f;

    private bool ending = true;

    private GameObject[] Obstacles;
    private GameObject[] Boxes;
    private GameObject[] Goals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Obstacles = GameObject.FindGameObjectsWithTag("Wall");
        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
        Goals = GameObject.FindGameObjectsWithTag("Goal");

        var movementScript = _player.GetComponent<PlayerMovement>();

        movementScript.enabled = false;

        StartDialogue();

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
        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }
        if (dialogueText.text == dialogueLines[lineIndex])
        {
            yield return new WaitForSeconds(1.0f);
            NextDialogueLine();
        }
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if (lineIndex < 3 )
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            dialoguePanel.SetActive(false);

            var movementScript = _player.GetComponent<PlayerMovement>();
            movementScript.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var goal in Goals)
        {
            foreach (var box in Boxes)
            {
                if (goal.transform.position.x == box.transform.position.x && goal.transform.position.y == box.transform.position.y)
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
        }
    }

    void StartFinishingLine()
    {
        dialoguePanel.SetActive(true);
        StartCoroutine(ShowLineFinish());
    }

    private IEnumerator ShowLineFinish()
    {
        dialogueText.text = string.Empty;
        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }
        if (dialogueText.text == dialogueLines[lineIndex])
        {
            yield return new WaitForSeconds(1.0f);
            NextFinishLine();
        }
    }

    private void NextFinishLine()
    {
        lineIndex++;
        if (lineIndex < dialogueLines.Length)
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
