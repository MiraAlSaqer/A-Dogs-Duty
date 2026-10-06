using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class DoubleGate : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform leftHinge;
    [SerializeField] private Transform rightHinge;
    [SerializeField] private NavMeshObstacle gateObstacle;

    [Header("Gate Settings")]
    [SerializeField] private Vector3 leftOpenRot;
    [SerializeField] private Vector3 rightOpenRot;
    [SerializeField] private float swingDuration = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gateSound;

    private bool isOpen;
    private Vector3 leftClosedRot;
    private Vector3 rightClosedRot;

    private void Start()
    {
        leftClosedRot = leftHinge.localEulerAngles;
        rightClosedRot = rightHinge.localEulerAngles;
    }

    public string GetPrompt()
    {
        if (isOpen)
        {
            return "Close Gate";
        }
        else
        {
            return "Open Gate";
        }
    }

    public void Interact()
    {
        isOpen = !isOpen;
        gateObstacle.enabled = !isOpen;

        audioSource.PlayOneShot(gateSound);

        leftHinge.DOKill();
        rightHinge.DOKill();

        Vector3 targetLeft;
        Vector3 targetRight;

        if (isOpen)
        {
            targetLeft = leftOpenRot;
            targetRight = rightOpenRot;
        }
        else
        {
            targetLeft = leftClosedRot;
            targetRight = rightClosedRot;
        }

        leftHinge.DOLocalRotate(targetLeft, swingDuration).SetEase(Ease.InOutQuad);
        rightHinge.DOLocalRotate(targetRight, swingDuration).SetEase(Ease.InOutQuad);

        // Task Completion Check
        if (TaskManager.Instance != null && TaskManager.Instance.currentPhase == TaskManager.TaskPhase.Herding)
        {
            // Complete only when the gate is officially closed and all 10 sheep are inside barn
            if (!isOpen)
            {
                SheepCounter counter = Object.FindFirstObjectByType<SheepCounter>();
                if (counter != null && counter.currentSheepInPen >= 10)
                {
                    TaskManager.Instance.CompleteCurrentTask();
                }
            }
        }
    }
}