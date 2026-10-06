using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Wolf : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3.5f;

    private NavMeshAgent agent;
    private Transform playerTarget;
    private bool isDead = false;

    [Header("Audio")]
    [SerializeField] private AudioClip whimperSound;
    private AudioSource audioSource;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        agent.speed = walkSpeed;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        var playerScript = Object.FindFirstObjectByType<ithappy.Animals_FREE.MovePlayerInput>();
        playerTarget = playerScript.transform;

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(playerTarget.position);
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(playerTarget.position);
        }
    }

    public void TakeDamage()
    {
        if (isDead) return;
        isDead = true;

        Vector3 soundPos;
        if (Camera.main != null)
        {
            soundPos = Camera.main.transform.position;
        }
        else
        {
            soundPos = transform.position;
        }
        AudioSource.PlayClipAtPoint(whimperSound, soundPos, 1f);

        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Play dying animation and destroy
        Vector3 targetRotation = transform.eulerAngles + new Vector3(0f, 0f, 90f);
        transform.DORotate(targetRotation, 0.4f)
            .SetEase(DG.Tweening.Ease.OutBounce)
            .OnComplete(() =>
            {
                transform.DOMoveY(transform.position.y - 0.2f, 0.5f)
                    .SetDelay(0.3f)
                    .OnComplete(() => Destroy(gameObject));
            });
    }
}