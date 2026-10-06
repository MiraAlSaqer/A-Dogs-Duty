using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ithappy.Animals_FREE;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.04f;
    public MovePlayerInput dogMovement;

    private Queue<string> sentences = new Queue<string>(); // Initialize the queue of dialogue
    private bool isTyping;
    private string currentSentence;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        dialogueBox.SetActive(false);
    }

    private void Update()
    {
        if (!dialogueBox.activeInHierarchy || !Input.GetKeyDown(KeyCode.Space)) return;

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentSentence;
            isTyping = false;
        }
        else
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(string[] dialogueLines)
    {
        dialogueBox.SetActive(true);
        dogMovement.canMove = false; // Disable dog movement while dialogue runs

        sentences.Clear();
        // Load all the lines into the queue
        foreach (string line in dialogueLines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // If there are no more sentences, close the box
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentSentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void EndDialogue()
    {
        dialogueBox.SetActive(false);
        dogMovement.canMove = true; // Re-enable player movement
    }
}