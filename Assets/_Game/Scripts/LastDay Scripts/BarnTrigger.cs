using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using DG.Tweening;
using ithappy.Animals_FREE;

public class BarnTrigger : MonoBehaviour
{
    [Header("Cinematic Timeline")]
    [SerializeField] private PlayableDirector panTimeline;
    [SerializeField] private GameObject barnCameraObject;

    [Header("Fade Screen Settings")]
    [SerializeField] private Image fadeScreen;
    [SerializeField] private float fadeDuration = 1.5f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        var mover = other.GetComponentInParent<MovePlayerInput>();
        if (mover != null || other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(DiscoveryRoutine(mover));
        }
    }

    private IEnumerator DiscoveryRoutine(MovePlayerInput mover)
    {
        mover.canMove = false;

        FinalDayManager.Instance.SetTaskListVisible(false);

        fadeScreen.gameObject.SetActive(true);
        fadeScreen.raycastTarget = true;
        Color c = fadeScreen.color;
        c.a = 0f;
        fadeScreen.color = c;

        yield return fadeScreen.DOFade(1f, fadeDuration).WaitForCompletion();

        FinalDayManager.Instance.SilenceBGMAndStartDread();

        barnCameraObject.SetActive(true);

        fadeScreen.gameObject.SetActive(false);

        panTimeline.Play();
        yield return new WaitForSeconds((float)panTimeline.duration);

        barnCameraObject.SetActive(false);

        fadeScreen.gameObject.SetActive(true);
        c = fadeScreen.color;
        c.a = 1f;
        fadeScreen.color = c;

        yield return fadeScreen.DOFade(0f, fadeDuration).WaitForCompletion();
        fadeScreen.raycastTarget = false;
        fadeScreen.gameObject.SetActive(false);

        mover.canMove = true;
        FinalDayManager.Instance.StartTrailTask();
        gameObject.SetActive(false);
    }
}