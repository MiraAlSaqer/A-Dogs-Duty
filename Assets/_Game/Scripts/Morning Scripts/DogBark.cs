using UnityEngine;

public class DogBark : MonoBehaviour
{
    [Header("Bark Settings")]
    [SerializeField] private float barkRadius = 10f;
    [SerializeField] private KeyCode barkKey = KeyCode.Mouse0;

    private void Update()
    {
        // Barking gameplay is active only during herding phase
        if (TaskManager.Instance == null || TaskManager.Instance.currentPhase != TaskManager.TaskPhase.Herding)
            return;

        if (Input.GetKeyDown(barkKey))
        {
            Bark();
        }
    }

    private void Bark()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, barkRadius);
        foreach (Collider hit in hits)
        {
            SheepAI sheep = hit.GetComponentInParent<SheepAI>();
            if (sheep != null)
            {
                sheep.HearBark(transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, barkRadius);
    }
}