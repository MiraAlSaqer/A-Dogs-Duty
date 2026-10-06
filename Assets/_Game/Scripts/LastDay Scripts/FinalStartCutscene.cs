using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using DG.Tweening;
using ithappy.Animals_FREE;

public class FinalStartCutscene : MonoBehaviour
{
    public PlayableDirector timeline;

    [Header("Drag the GameObjects Here")]
    public GameObject dogGameObject;
    public GameObject firstPersonCamera;
    public GameObject houseCover;
    public Collider[] dogColliders;

    [Header("Transition Settings")]
    public Image fadeScreen;
    public float fadeDuration = 1.5f;
    public AudioSource bgmSource;
    public float musicResumeTime = 0f;

    private MovePlayerInput dogMovement;
    private Zoom dogZoom;
    private MonoBehaviour[] cameraScripts;

    private void Start()
    {
        dogMovement = dogGameObject.GetComponent<MovePlayerInput>();
        dogZoom = dogGameObject.GetComponent<Zoom>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        fadeScreen.gameObject.SetActive(false);

        dogMovement.enabled = false;
        dogZoom.enabled = false;

        cameraScripts = firstPersonCamera.GetComponents<MonoBehaviour>();
        foreach (var script in cameraScripts)
        {
            script.enabled = false;
        }

        foreach (Collider col in dogColliders)
        {
            col.enabled = false;
        }

        timeline.stopped += OnCutsceneEnded;
    }

    private void OnCutsceneEnded(PlayableDirector director)
    {
        houseCover.SetActive(true);

        foreach (Collider col in dogColliders)
        {
            col.enabled = true;
        }

        foreach (var script in cameraScripts)
        {
            script.enabled = true;
        }

        Vector3 currentEuler = dogGameObject.transform.eulerAngles;
        currentEuler.x = 0f;
        currentEuler.z = 0f;
        dogGameObject.transform.eulerAngles = currentEuler;

        dogMovement.enabled = true;
        dogMovement.canMove = true;
        dogMovement.lockCursor = true;
        dogMovement.SetCameraState(false);

        dogZoom.enabled = true;

        bgmSource.time = musicResumeTime;
        bgmSource.Play();

        FinalDayManager.Instance.InitializeFirstTask();

        fadeScreen.gameObject.SetActive(true);
        Color c = fadeScreen.color;
        c.a = 1f;
        fadeScreen.color = c;

        fadeScreen.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            fadeScreen.gameObject.SetActive(false);
        });
    }

    private void OnDestroy()
    {
        timeline.stopped -= OnCutsceneEnded;
    }
}