using Mono.Cecil;
using System.Linq.Expressions;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject _player;

    [Header("Configuración de Diálogos")]
    [SerializeField, TextArea(3, 5)] private string[] startDialogueLines;
    [SerializeField, TextArea(3, 5)] private string[] endDialogueLines;

    private int lineIndex;
    public float typingTime = 0.05f;

    private bool startDialogueNotEnded = true;
    private bool ending = true;
    private bool inputByUser = false;

    private GameObject[] Obstacles;
    private GameObject[] Boxes;
    private GameObject[] Goals;

    private class GameState
    {
        public Vector3 ovanPos;
        public Vector3[] boxPos;

        public GameState(Vector3 playerPos, Vector3[] boxesPos)
        {
            ovanPos = playerPos;
            boxPos = (Vector3[])boxesPos.Clone();  //Esto es para guardar los valores y no una referencia, mi cara cuando Unity
        }
    }

    private List<GameState> stateHistory = new List<GameState>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Obstacles = GameObject.FindGameObjectsWithTag("Wall");
        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
        Goals = GameObject.FindGameObjectsWithTag("Goal");

        var movementScript = _player.GetComponent<PlayerMovement>();

        movementScript.enabled = false;

        StartDialogue();
        RecordState();
    }

    public void RecordState()
    {
        Vector3[] currentBoxPositions = new Vector3[Boxes.Length];
        for (int i = 0; i < Boxes.Length; i++)
        {
            currentBoxPositions[i] = Boxes[i].transform.position;
        }
        stateHistory.Add(new GameState(_player.transform.position, currentBoxPositions));
    }

    private void Undo()
    {
        
        if (stateHistory.Count > 1)
        {
            
            stateHistory.RemoveAt(stateHistory.Count - 1); 

            GameState previousState = stateHistory[stateHistory.Count - 1];

            _player.transform.position = previousState.ovanPos;
            for (int i = 0; i < Boxes.Length; i++)
            {
                Boxes[i].transform.position = previousState.boxPos[i];
            }
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

        if (Input.GetKeyDown(KeyCode.R) && !startDialogueNotEnded)
        {
            if (stateHistory.Count > 0)
            {
                GameState initialState = stateHistory[0];

                _player.transform.position = initialState.ovanPos;
                for (int i = 0; i < Boxes.Length; i++)
                {
                    Boxes[i].transform.position = initialState.boxPos[i];
                }

                RecordState();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && !startDialogueNotEnded)
        {
            Undo();
        }

        if (startDialogueNotEnded)
        {
            if (Input.GetButtonDown("Fire1") && !inputByUser)
            {
                inputByUser = true;
                StopAllCoroutines();
                dialogueText.text = startDialogueLines[lineIndex];
            }
            else if(Input.GetButtonDown("Fire1"))
            {
                inputByUser = false;
                NextDialogueLine();
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
