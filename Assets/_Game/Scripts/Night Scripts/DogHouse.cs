using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DogHouse : MonoBehaviour, IInteractable
{
    [SerializeField] private string promptText = "Sleep";
    [SerializeField] private Image fadeScreen;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private float fadeDuration = 5.0f;

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName = "LastDay";

    private bool isSleeping = false;
    private ithappy.Animals_FREE.MovePlayerInput playerMover;

    private void Start()
    {
        playerMover = Object.FindFirstObjectByType<ithappy.Animals_FREE.MovePlayerInput>();
    }

    public string GetPrompt()
    {
        // Only show prompt if player is on the final task
        if (NightTaskManager.Instance.currentPhase == NightTaskManager.NightPhase.GoToSleep && !isSleeping)
        {
            return promptText;
        }
        return "";
    }

    public void Interact()
    {
        if (isSleeping) return;
        if (NightTaskManager.Instance.currentPhase != NightTaskManager.NightPhase.GoToSleep)
            return;

        StartCoroutine(SleepEndingSequence());
    }

    private IEnumerator SleepEndingSequence()
    {
        isSleeping = true;

        playerMover.canMove = false;

        fadeScreen.gameObject.SetActive(true);
        fadeScreen.raycastTarget = true;
        fadeScreen.DOFade(1f, fadeDuration);

        musicAudioSource.DOFade(0f, fadeDuration);

        yield return new WaitForSeconds(fadeDuration);

        NightTaskManager.Instance.currentPhase = NightTaskManager.NightPhase.Done;
        NightTaskManager.Instance.UpdateTaskUI();

        SceneManager.LoadScene(nextSceneName);
    }
}