using UnityEngine;

public class SheepCounter : MonoBehaviour
{
    [Header("Pen Data")]
    public int currentSheepInPen = 10;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip countUpSound;

    private void OnTriggerExit(Collider other)
    {
        if (TaskManager.Instance.currentPhase == TaskManager.TaskPhase.LetSheepOut)
        {
            if (other.CompareTag("Sheep"))
            {
                SheepAI sheep = other.GetComponentInParent<SheepAI>();
                if (!sheep.HasExitedPen())
                {
                    sheep.MarkExitedPen();
                    currentSheepInPen--;

                    if (currentSheepInPen <= 0)
                    {
                        TaskManager.Instance.CompleteCurrentTask();
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TaskManager.Instance.currentPhase == TaskManager.TaskPhase.LetSheepOut)
        {
            if (other.CompareTag("Sheep"))
            {
                SheepAI sheep = other.GetComponentInParent<SheepAI>();
                if (sheep.HasExitedPen())
                {
                    sheep.MarkReturnedPen();
                    currentSheepInPen++;
                }
            }
        }
        else if (TaskManager.Instance.currentPhase == TaskManager.TaskPhase.Herding)
        {
            if (other.CompareTag("Sheep"))
            {
                SheepAI sheep = other.GetComponentInParent<SheepAI>();
                if (!sheep.IsLockedInPen())
                {
                    currentSheepInPen++;

                    audioSource.PlayOneShot(countUpSound);

                    TaskManager.Instance.AddHerdedSheep();
                    sheep.LockInPen();
                }
            }
        }
    }
}