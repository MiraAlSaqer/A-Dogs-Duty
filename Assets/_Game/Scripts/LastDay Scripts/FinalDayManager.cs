using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FinalDayManager : MonoBehaviour
{
    public static FinalDayManager Instance;

    public enum FinalPhase { MorningRoutine, InvestigateTrail, Climax }
    public FinalPhase currentPhase = FinalPhase.MorningRoutine;

    [Header("UI References")]
    [SerializeField] private GameObject taskListUI;
    [SerializeField] private TMP_Text taskText;
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip newTaskSound;

    [Header("Audio")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource dreadAmbienceSource;

    private readonly string bullet = "• ";

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

    private void Start()
    {
        taskListUI.SetActive(false);
    }

    public void InitializeFirstTask()
    {
        StartCoroutine(ShowFirstTaskDelay());
    }

    private IEnumerator ShowFirstTaskDelay()
    {
        yield return new WaitForSeconds(0.8f);

        SetTaskListVisible(true);
        uiAudioSource.PlayOneShot(newTaskSound);
        UpdateTaskUI();
    }

    public void UpdateTaskUI()
    {
        string mainTask = "";
        string subTasks = "";

        switch (currentPhase)
        {
            case FinalPhase.MorningRoutine:
                mainTask = "Let the sheep out";
                subTasks = bullet + "Head over to the barn";
                break;

            case FinalPhase.InvestigateTrail:
                mainTask = "Investigate";
                subTasks = bullet + "Follow the blood trail";
                break;

            case FinalPhase.Climax:
                SetTaskListVisible(false);
                return;
        }

        taskText.text = $"{mainTask}\n<size=80%>{subTasks}</size>";
    }

    public void SilenceBGMAndStartDread()
    {
        bgmAudioSource.DOKill();
        bgmAudioSource.Stop();

        dreadAmbienceSource.DOKill();
        dreadAmbienceSource.loop = true;
        dreadAmbienceSource.volume = 0f;
        dreadAmbienceSource.Play();
        dreadAmbienceSource.DOFade(0.5f, 1.5f).SetUpdate(true);
    }

    public void StartTrailTask()
    {
        currentPhase = FinalPhase.InvestigateTrail;
        SetTaskListVisible(true);

        uiAudioSource.PlayOneShot(newTaskSound);

        UpdateTaskUI();
    }

    public void SetTaskListVisible(bool visible)
    {
        taskListUI.SetActive(visible);
    }
}