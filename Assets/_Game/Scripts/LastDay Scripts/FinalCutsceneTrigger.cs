using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using ithappy.Animals_FREE;

public class FinalCutsceneTrigger : MonoBehaviour
{
    [Header("Timeline & Set")]
    [SerializeField] private PlayableDirector cutsceneTimeline;
    [SerializeField] private GameObject cinematicSetRoot;
    [SerializeField] private GameObject dog;

    [Header("Cutscene Audio")]
    [SerializeField] private AudioSource cutsceneBGM;
    [SerializeField] private float musicFadeOutDuration = 2.0f;

    [Header("Transition Screen")]
    [SerializeField] private Image fadeScreen;
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField]
    private string[] wolfDialogue = new string[]
    {
        "You protected the flock well that night, little sheepdog.",
        "You took blood from our pack to save theirs.",
        "Now their blood paints your grass... and yours belongs to us."
    };

    [Header("End Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        var mover = other.GetComponentInParent<MovePlayerInput>();
        if (mover != null || other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(StartCutsceneSequence(mover));
        }
    }

    private IEnumerator StartCutsceneSequence(MovePlayerInput mover)
    {
        mover.canMove = false;
        mover.enabled = false;

        FinalDayManager.Instance.SetTaskListVisible(false);

        fadeScreen.gameObject.SetActive(true);
        fadeScreen.raycastTarget = true;
        yield return fadeScreen.DOFade(1f, fadeDuration).WaitForCompletion();

        dog.SetActive(false);
        cinematicSetRoot.SetActive(true);

        cutsceneBGM.loop = true;
        cutsceneBGM.volume = 1f;
        cutsceneBGM.Play();

        cutsceneTimeline.Play();
        StartCoroutine(MonitorCutsceneEnd());

        yield return fadeScreen.DOFade(0f, fadeDuration).WaitForCompletion();
        fadeScreen.raycastTarget = false;

        cutsceneTimeline.stopped += OnTimelineFinished;
    }

    private IEnumerator MonitorCutsceneEnd()
    {
        bool dimmed = false;

        while (cutsceneTimeline.state == PlayState.Playing || cutsceneTimeline.state == PlayState.Paused)
        {
            double remainingTime = cutsceneTimeline.duration - cutsceneTimeline.time;

            if (!dimmed && remainingTime <= musicFadeOutDuration)
            {
                dimmed = true;
                cutsceneBGM.DOFade(0f, musicFadeOutDuration).SetUpdate(true);
            }

            yield return null;
        }
    }

    // Called using Timeline Signal Emitter
    public void PauseForWolfDialogue()
    {
        cutsceneTimeline.Pause();

        DialogueManager.Instance.StartDialogue(wolfDialogue);
        StartCoroutine(WaitForDialogueToFinish());
    }

    private IEnumerator WaitForDialogueToFinish()
    {
        yield return null;

        while (DialogueManager.Instance.dialogueBox.activeInHierarchy)
        {
            yield return null;
        }

        cutsceneTimeline.Play();
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        if (mainMenuSceneName != "")
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private void OnDestroy()
    {
        cutsceneTimeline.stopped -= OnTimelineFinished;
    }
}