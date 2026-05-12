//using Mono.Cecil;
//using System.Linq.Expressions;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
//using System.Runtime.CompilerServices;

public class SceneManager : MonoBehaviour
{
    public AudioMixer mainMixer;

    [SerializeField] private GameObject dialoguePanel = null;
    [SerializeField] private TMP_Text dialogueText = null;
    [SerializeField] private GameObject _player;

    [SerializeField] private bool hasDialogues;

    [Header("Configuración de Diálogos")]
    [SerializeField, TextArea(3, 5)] private string[] startDialogueLines;
    [SerializeField, TextArea(3, 5)] private string[] endDialogueLines;

    [SerializeField] string nextScene;

    [SerializeField] private GameObject pauseMenuPanel = null;
    private bool isPaused = false;

    private int lineIndex;
    public float typingTime = 0.05f;

    private bool startDialogueNotEnded = true;
    private bool ending = true;
    private bool inputByUser = false;

    private GameObject[] Boxes;
    private GameObject[] Goals;

    public AudioSource sceneAudio;

    public AudioClip allGoalsComplete;

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
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("MusicVol", 1f), 0.0001f)) * 20f);
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("SFXVol", 1f), 0.0001f)) * 20f);

        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
        Goals = GameObject.FindGameObjectsWithTag("Goal");

        var movementScript = _player.GetComponent<PlayerMovement>();

        movementScript.enabled = false;

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        if (hasDialogues)
        {
            StartDialogue();
        }
        else
        {
            startDialogueNotEnded = false;
            _player.GetComponent<PlayerMovement>().enabled = true;
        }

        RecordState();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (isPaused) return;

        var movementScript = _player.GetComponent<PlayerMovement>();
        if (Input.GetKeyDown(KeyCode.R) && !startDialogueNotEnded && movementScript.enabled)
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

        if (Input.GetKeyDown(KeyCode.Q) && !startDialogueNotEnded && movementScript.enabled)
        {
            Undo();
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
                ending = false;
                sceneAudio.PlayOneShot(allGoalsComplete);
                movementScript.enabled = false;
                StartFinishingLine();
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
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }
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
            Debug.Log("AAAA ME DUELE AAA");

            stateHistory.RemoveAt(stateHistory.Count - 1);

            GameState previousState = stateHistory[stateHistory.Count - 1];

            _player.transform.position = previousState.ovanPos;
            for (int i = 0; i < Boxes.Length; i++)
            {
                Boxes[i].transform.position = previousState.boxPos[i];
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        _player.GetComponent<PlayerMovement>().enabled = false;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;

        if (!startDialogueNotEnded && ending)
        {
            _player.GetComponent<PlayerMovement>().enabled = true;
        }
    }

    public void SkipLevel()
    {
        Time.timeScale = 1f; 
        if (!string.IsNullOrEmpty(nextScene))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning("No hay siguiente escena configurada.");
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("Splash");
    }
}
