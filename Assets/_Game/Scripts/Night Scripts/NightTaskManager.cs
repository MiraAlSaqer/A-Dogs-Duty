using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class NightTaskManager : MonoBehaviour
{
    public static NightTaskManager Instance;

    public enum NightPhase { ProtectSheep, GoToSleep, Done }

    [Header("Timeline Cutscene")]
    [SerializeField] private PlayableDirector introDirector;

    [Header("Current State")]
    public NightPhase currentPhase = NightPhase.ProtectSheep;

    [Header("UI References")]
    [SerializeField] private GameObject taskListUI;
    [SerializeField] private TMP_Text taskText;
    [SerializeField] private Image fadeScreen;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioClip newTaskSound;
    [SerializeField] private AudioClip completeTaskSound;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip preDefenseAmbience;
    [SerializeField] private AudioClip defenseCombatMusic;
    [SerializeField] private AudioClip postDefenseAmbience;

    [Header("Volume Controls")]
    [Range(0f, 1f)]
    [SerializeField] private float combatMusicVolume = 0.45f;
    [Range(0f, 1f)]
    [SerializeField] private float ambientMusicVolume = 0.6f;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 1.5f;

    private readonly string bullet = "• ";
    private bool hasGun = false;
    private ithappy.Animals_FREE.MovePlayerInput playerMover;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        introDirector.stopped += OnTimelineFinished;
    }

    private void OnDisable()
    {
        introDirector.stopped -= OnTimelineFinished;
    }

    private void Start()
    {
        playerMover = Object.FindFirstObjectByType<ithappy.Animals_FREE.MovePlayerInput>();

        taskListUI.SetActive(false);

        if (introDirector.playOnAwake)
        {
            playerMover.canMove = false;
        }
        else
        {
            StartGameplaySequence();
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        StartGameplaySequence();
    }

    private void StartGameplaySequence()
    {
        playerMover.SetCameraState(false);
        playerMover.canMove = true;
        playerMover.lockCursor = true;

        musicAudioSource.clip = preDefenseAmbience;
        musicAudioSource.loop = true;
        musicAudioSource.volume = ambientMusicVolume;
        musicAudioSource.Play();

        StartCoroutine(ShowFirstTaskDelay());
    }

    private IEnumerator ShowFirstTaskDelay()
    {
        yield return new WaitForSeconds(0.4f);

        SetTaskListVisible(true);
        uiAudioSource.PlayOneShot(newTaskSound);
        UpdateTaskUI();
    }

    public void OnGunPickedUp()
    {
        hasGun = true;
        UpdateTaskUI();
    }

    public void UpdateTaskUI()
    {
        if (currentPhase == NightPhase.Done)
        {
            taskText.text = "Night Complete!";
            return;
        }

        string mainTask = "";
        string subTasks = "";

        switch (currentPhase)
        {
            case NightPhase.ProtectSheep:
                mainTask = "Protect the Sheep";
                if (!hasGun)
                {
                    subTasks = bullet + "Pick up gun next to house\n" + bullet + "Stand in front of the barn";
                }
                else
                {
                    subTasks = "<s>" + bullet + "Pick up gun next to house</s>\n" + bullet + "Stand in front of the barn";
                }
                break;

            case NightPhase.GoToSleep:
                mainTask = "Rest";
                subTasks = bullet + "Return to your doghouse and sleep";
                break;
        }

        taskText.text = mainTask + "\n<size=80%>" + subTasks + "</size>";
    }

    public void CompleteDefenseTask()
    {
        StartCoroutine(DefenseCompletionSequence());
    }

    private IEnumerator DefenseCompletionSequence()
    {
        uiAudioSource.PlayOneShot(completeTaskSound);

        taskText.text = "<color=#888888><s>" + taskText.text + "</s></color>";

        yield return new WaitForSeconds(2.5f);

        currentPhase = NightPhase.GoToSleep;
        uiAudioSource.PlayOneShot(newTaskSound);

        UpdateTaskUI();
    }

    public void PlayCombatMusic()
    {
        musicAudioSource.DOKill();
        musicAudioSource.DOFade(0f, 0.4f).OnComplete(() =>
        {
            musicAudioSource.clip = defenseCombatMusic;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
            musicAudioSource.DOFade(combatMusicVolume, 0.4f);
        });
    }

    public void StopMusicImmediately()
    {
        musicAudioSource.DOKill();
        musicAudioSource.Stop();
    }

    public void RestartCombatMusic()
    {
        musicAudioSource.DOKill();
        musicAudioSource.clip = defenseCombatMusic;
        musicAudioSource.loop = true;
        musicAudioSource.volume = combatMusicVolume;
        musicAudioSource.Play();
    }

    public void PlayPostDefenseAmbience()
    {
        musicAudioSource.DOKill();
        musicAudioSource.DOFade(0f, 1.0f).OnComplete(() =>
        {
            musicAudioSource.clip = postDefenseAmbience;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
            musicAudioSource.DOFade(ambientMusicVolume, 1.5f);
        });
    }

    public void SetTaskListVisible(bool visible)
    {
        taskListUI.SetActive(visible);
    }
}