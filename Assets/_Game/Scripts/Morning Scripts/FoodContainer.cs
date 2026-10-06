using UnityEngine;

public class FoodContainer : MonoBehaviour
{
    [SerializeField] private GameObject food;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip fillSound;

    private bool isFilled;

    private void Start()
    {
        food.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFilled) return;

        FoodSack sack = other.GetComponentInParent<FoodSack>();
        if (sack == null) return;

        sack.EmptySack();
        food.SetActive(true);
        isFilled = true;

        audioSource.PlayOneShot(fillSound);

        if (TaskManager.Instance != null && TaskManager.Instance.currentPhase == TaskManager.TaskPhase.Feed)
        {
            TaskManager.Instance.CompleteCurrentTask();
        }
    }
}