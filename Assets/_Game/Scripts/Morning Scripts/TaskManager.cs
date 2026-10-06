using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("UI & Audio")]
    public GameObject taskListUI;
    public TMP_Text taskText;
    public GameObject barkPromptUI;
    public AudioSource UIAud;
    public AudioClip newTaskSound;
    public AudioClip completeTaskSound;

    [Header("End Sequence Settings")]
    public Image fadeScreen;
    public AudioSource bgmAud;
    public float fadeOutDuration = 5f;
    [SerializeField] private string nextSceneName = "FirstDayNight";

    public enum TaskPhase { LetSheepOut, Feed, Water, Herding, Done }
    public TaskPhase currentPhase = TaskPhase.LetSheepOut;

    private int sheepHerded = 0;
    private readonly string bullet = "• ";

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        barkPromptUI.SetActive(false);
        taskListUI.SetActive(false);
    }

    public void InitializeTasks()
    {
        taskListUI.SetActive(true);
        UIAud.PlayOneShot(newTaskSound);
        UpdateTaskUI();
    }

    public void CompleteCurrentTask()
    {
        if (currentPhase == TaskPhase.Done) return;
        StartCoroutine(TaskCompletionSequence());
    }

    private IEnumerator TaskCompletionSequence()
    {
        UIAud.PlayOneShot(completeTaskSound);

        taskText.text = $"<color=#888888><s>{taskText.text}</s></color>";

        yield return new WaitForSeconds(3f);

        currentPhase++;

        if (currentPhase == TaskPhase.Done)
        {
            UpdateTaskUI();
            StartCoroutine(MorningEndingSequence());
            yield break;
        }

        UIAud.PlayOneShot(newTaskSound);
        UpdateTaskUI();
    }

    private IEnumerator MorningEndingSequence()
    {
        yield return new WaitForSeconds(3f);

        fadeScreen.gameObject.SetActive(true);
        Color c = fadeScreen.color;
        c.a = 0f;
        fadeScreen.color = c;
        fadeScreen.DOFade(1f, fadeOutDuration);

        bgmAud.DOFade(0f, fadeOutDuration);
        yield return new WaitForSeconds(fadeOutDuration);

        SceneManager.LoadScene(nextSceneName);
    }

    public void UpdateTaskUI()
    {
        if (currentPhase == TaskPhase.Done)
        {
            taskText.text = "Morning Chores Complete!";
            barkPromptUI.SetActive(false);
            return;
        }

        if (currentPhase == TaskPhase.Herding)
        {
            barkPromptUI.SetActive(true);
        }
        else
        {
            barkPromptUI.SetActive(false);
        }

        string mainTask;
        string subTasks;

        if (currentPhase == TaskPhase.LetSheepOut)
        {
            mainTask = "Let the sheep out";
            subTasks = bullet + "Open the gate\n" + bullet + "Wait for all sheep to exit";
        }
        else if (currentPhase == TaskPhase.Feed)
        {
            mainTask = "Put Food";
            subTasks = bullet + "Push the food to the container";
        }
        else if (currentPhase == TaskPhase.Water)
        {
            mainTask = "Fill Water";
            subTasks = bullet + "Bring the bucket\n" + bullet + "Fill with water from the well\n" + bullet + "Fill the container";
        }
        else if (currentPhase == TaskPhase.Herding)
        {
            mainTask = "Bring back sheep";
            subTasks = bullet + "Bark to herd them\n" + bullet + "Close the gate";

            taskText.text = $"{mainTask}\n<size=80%>{subTasks}</size>\n\n<b>Sheep: {sheepHerded}/10</b>";
            return;
        }
        else
        {
            mainTask = "";
            subTasks = "";
        }

        taskText.text = $"{mainTask}\n<size=80%>{subTasks}</size>";
    }

    public void AddHerdedSheep()
    {
        if (currentPhase == TaskPhase.Herding)
        {
            sheepHerded++;
            UpdateTaskUI();
        }
    }
}