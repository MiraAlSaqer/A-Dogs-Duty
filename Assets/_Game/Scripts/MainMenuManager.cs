using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsCanvas;

    [Header("Scene Loading & Black Fade")]
    [SerializeField] private string firstSceneName = "First Day";
    [SerializeField] private Image fadeScreen;
    [SerializeField] private float fadeDuration = 3.0f;
    [SerializeField] private CanvasGroup buttonGroup;

    [Header("Audio")]
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource menuBGM;

    private bool isStarting = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        optionsCanvas.SetActive(false);

        fadeScreen.gameObject.SetActive(true);
        fadeScreen.raycastTarget = false;
        Color c = fadeScreen.color;
        c.a = 0f;
        fadeScreen.color = c;
    }

    public void OpenOptions()
    {
        PlayClickSound();
        optionsCanvas.SetActive(true);
    }

    public void CloseOptions()
    {
        PlayClickSound();
        optionsCanvas.SetActive(false);
    }

    public void OnPlayButtonClicked()
    {
        if (isStarting) return;
        isStarting = true;

        PlayClickSound();
        StartCoroutine(PlayTransitionRoutine());
    }

    private IEnumerator PlayTransitionRoutine()
    {
        buttonGroup.interactable = false;
        buttonGroup.blocksRaycasts = false;

        fadeScreen.raycastTarget = true;
        fadeScreen.DOFade(1f, fadeDuration);

        menuBGM.DOFade(0f, fadeDuration);

        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(firstSceneName);
    }

    public void OnQuitButtonClicked()
    {
        PlayClickSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PlayClickSound()
    {
        clickSound.Play();
    }
}