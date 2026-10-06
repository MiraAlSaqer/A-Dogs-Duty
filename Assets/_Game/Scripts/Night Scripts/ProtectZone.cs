using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProtectZone : MonoBehaviour
{
    [Header("Defense Setup")]
    [SerializeField] private Transform standPoint;
    [SerializeField] private WolfSpawner spawner;

    [Header("Fade & Failure UI")]
    [SerializeField] private Image fadeScreenImage;
    [SerializeField] private CanvasGroup tryAgainGroup;

    [Header("Gameplay HUD")]
    [SerializeField] private GameObject shootPromptUI;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Settings")]
    [SerializeField] private float waveDuration = 60f;

    private float currentTimer;
    private bool isDefending = false;
    private bool isTransitioning = false;
    private GameObject playerRef;
    private ithappy.Animals_FREE.MovePlayerInput playerMover;
    private Gun playerGun;

    private void Start()
    {
        playerGun = Object.FindFirstObjectByType<Gun>();

        fadeScreenImage.gameObject.SetActive(true);
        Color c = fadeScreenImage.color;
        c.a = 0f;
        fadeScreenImage.color = c;
        fadeScreenImage.raycastTarget = false;

        tryAgainGroup.alpha = 0f;
        tryAgainGroup.blocksRaycasts = false;
        tryAgainGroup.interactable = false;
        tryAgainGroup.gameObject.SetActive(false);

        shootPromptUI.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDefending)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer < 0f) currentTimer = 0f;

            int minutes = Mathf.FloorToInt(currentTimer / 60F);
            int seconds = Mathf.FloorToInt(currentTimer % 60F);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (currentTimer <= 0f)
            {
                StartCoroutine(WinDefenseSequence());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Wolf wolf = other.GetComponentInParent<Wolf>();
        if (wolf != null && isDefending && !isTransitioning)
        {
            TriggerFailure();
            return;
        }

        if (!isDefending && !isTransitioning)
        {
            var mover = other.GetComponentInParent<ithappy.Animals_FREE.MovePlayerInput>();
            if (mover != null)
            {
                playerRef = mover.gameObject;
            }
            else
            {
                playerRef = other.gameObject;
            }

            playerMover = playerRef.GetComponent<ithappy.Animals_FREE.MovePlayerInput>();

            StartCoroutine(EnterDefenseTransition());
        }
    }

    private IEnumerator EnterDefenseTransition()
    {
        isTransitioning = true;

        playerMover.canMove = false;
        playerGun.canShoot = false;

        NightTaskManager.Instance.SetTaskListVisible(false);

        fadeScreenImage.raycastTarget = true;
        yield return fadeScreenImage.DOFade(1f, 1.5f).WaitForCompletion();

        Vector3 targetPos;
        if (standPoint != null)
        {
            targetPos = standPoint.position;
        }
        else
        {
            targetPos = transform.position;
        }

        CharacterController cc = playerRef.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        Rigidbody rb = playerRef.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        playerRef.transform.position = targetPos;
        if (standPoint != null)
        {
            playerRef.transform.rotation = standPoint.rotation;
        }

        if (cc != null) cc.enabled = true;

        NightTaskManager.Instance.PlayCombatMusic();

        yield return fadeScreenImage.DOFade(0f, 1.5f).WaitForCompletion();
        fadeScreenImage.raycastTarget = false;

        isTransitioning = false;
        StartDefenseWaveGameplay();
    }

    private void StartDefenseWaveGameplay()
    {
        playerGun.canShoot = true;

        playerMover.lockCursor = true;
        playerMover.canMove = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentTimer = waveDuration;
        isDefending = true;

        timerText.gameObject.SetActive(true);
        shootPromptUI.SetActive(true);

        spawner.StartDefenseWave();
    }

    public void TriggerFailure()
    {
        isDefending = false;
        isTransitioning = true;

        playerGun.canShoot = false;

        NightTaskManager.Instance.StopMusicImmediately();

        spawner.StopAllCoroutines();
        spawner.ClearAllWolves();

        shootPromptUI.SetActive(false);
        timerText.gameObject.SetActive(false);

        Sequence failSeq = DOTween.Sequence();
        fadeScreenImage.raycastTarget = true;
        failSeq.Append(fadeScreenImage.DOFade(1f, 0.4f).SetEase(Ease.InOutQuad));

        failSeq.AppendCallback(() =>
        {
            tryAgainGroup.gameObject.SetActive(true);
            tryAgainGroup.alpha = 0f;
        });

        failSeq.Append(tryAgainGroup.DOFade(1f, 0.35f));

        failSeq.AppendCallback(() =>
        {
            tryAgainGroup.blocksRaycasts = true;
            tryAgainGroup.interactable = true;

            playerMover.lockCursor = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isTransitioning = false;
        });
    }

    public void OnTryAgainClicked()
    {
        tryAgainGroup.alpha = 0f;
        tryAgainGroup.blocksRaycasts = false;
        tryAgainGroup.interactable = false;
        tryAgainGroup.gameObject.SetActive(false);

        NightTaskManager.Instance.RestartCombatMusic();

        Vector3 targetPos;
        if (standPoint != null)
        {
            targetPos = standPoint.position;
        }
        else
        {
            targetPos = transform.position;
        }
        playerRef.transform.position = targetPos;

        fadeScreenImage.DOFade(0f, 0.4f).OnComplete(() =>
        {
            fadeScreenImage.raycastTarget = false;
            StartDefenseWaveGameplay();
        });
    }

    private IEnumerator WinDefenseSequence()
    {
        isDefending = false;
        isTransitioning = true;

        playerGun.canShoot = false;
        timerText.gameObject.SetActive(false);
        shootPromptUI.SetActive(false);

        spawner.StopAllCoroutines();
        spawner.ClearAllWolves();

        fadeScreenImage.raycastTarget = true;
        yield return fadeScreenImage.DOFade(1f, 1.5f).WaitForCompletion();

        playerMover.SetCameraState(false);
        playerMover.canMove = true;
        playerMover.lockCursor = true;

        playerGun.Drop();

        NightTaskManager.Instance.PlayPostDefenseAmbience();

        yield return fadeScreenImage.DOFade(0f, 1.5f).WaitForCompletion();
        fadeScreenImage.raycastTarget = false;
        isTransitioning = false;

        NightTaskManager.Instance.SetTaskListVisible(true);
        NightTaskManager.Instance.CompleteDefenseTask();

        gameObject.SetActive(false);
    }
}