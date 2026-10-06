using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SheepAI : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip soundA;
    public AudioClip soundB;
    public float minBleatTime = 4f;
    public float maxBleatTime = 12f;

    [Header("Animation Settings")]
    public string walkAnimation = "walk_forward";
    public string runAnimation = "run_forward";
    public string idleAnimation = "idle";

    [Header("AI Settings")]
    [SerializeField] private float roamRadius = 4f;
    [SerializeField] private float minIdleTime = 2f;
    [SerializeField] private float maxIdleTime = 6f;

    [Header("Herding Settings")]
    [SerializeField] private float fleeDistance = 8f;
    [SerializeField] private float runSpeed = 4.5f;

    private NavMeshAgent agent;
    private Animator anim;
    private float normalSpeed;

    private bool hasExited = false;

    public bool HasExitedPen() { return hasExited; }
    public void MarkExitedPen() { hasExited = true; }
    public void MarkReturnedPen() { hasExited = false; }
    public bool IsLockedInPen() { return isLocked; }

    private bool isExiting = false;
    private bool isHerded = false;
    private bool isLocked = false;

    private enum State { Idle, Walk, Run }
    private State currentState = State.Idle;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        normalSpeed = agent.speed;

        agent.avoidancePriority = Random.Range(10, 99);

        StartCoroutine(InitSheep());
        StartCoroutine(RandomSheepSounds());
    }

    private IEnumerator RandomSheepSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minBleatTime, maxBleatTime));

            AudioClip clipToPlay;
            if (Random.Range(0, 2) == 0)
            {
                clipToPlay = soundA;
            }
            else
            {
                clipToPlay = soundB;
            }

            audioSource.PlayOneShot(clipToPlay);
        }
    }

    private IEnumerator InitSheep()
    {
        yield return null;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(state.fullPathHash, 0, Random.Range(0f, 1f));
        anim.speed = Random.Range(0.85f, 1.15f);

        StartCoroutine(RoamRoutine());
    }

    public void Release(Transform targetExit)
    {
        if (isLocked) return;

        roamRadius = 25f;
        isExiting = true;

        StopAllCoroutines();
        StartCoroutine(RandomSheepSounds());

        Vector3 randomSpread = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        agent.SetDestination(targetExit.position + randomSpread);
    }

    public void HearBark(Transform dogTransform)
    {
        if (isLocked || isHerded) return;

        Vector3 fleeDirection = (transform.position - dogTransform.position).normalized;
        Vector3 targetPosition = transform.position + (fleeDirection * fleeDistance);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, fleeDistance, NavMesh.AllAreas))
        {
            StopAllCoroutines();
            StartCoroutine(RandomSheepSounds());
            isHerded = true;
            agent.speed = runSpeed;
            agent.SetDestination(hit.position);

            StartCoroutine(HerdingRoutine());
        }
    }

    private IEnumerator HerdingRoutine()
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        agent.speed = normalSpeed;
        isHerded = false;
        StartCoroutine(RoamRoutine());
    }

    private void Update()
    {
        UpdateAnimationState();

        if (isLocked) return;

        if (isExiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isExiting = false;
            StartCoroutine(RoamRoutine());
        }
    }

    private void UpdateAnimationState()
    {
        float speed = agent.velocity.magnitude;
        State targetState = State.Idle;

        if (speed > normalSpeed + 0.5f)
        {
            targetState = State.Run;
        }
        else if (speed > 0.1f)
        {
            targetState = State.Walk;
        }

        if (currentState != targetState)
        {
            currentState = targetState;
            if (currentState == State.Run)
            {
                anim.CrossFade(runAnimation, 0.2f);
            }
            else if (currentState == State.Walk)
            {
                anim.CrossFade(walkAnimation, 0.2f);
            }
            else
            {
                anim.CrossFade(idleAnimation, 0.2f);
            }
        }
    }

    private IEnumerator RoamRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));

            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1))
            {
                agent.SetDestination(hit.position);
            }

            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }
        }
    }

    public void LockInPen()
    {
        if (isLocked) return;
        isLocked = true;
        StartCoroutine(DelayedFreezeRoutine());
    }

    private IEnumerator DelayedFreezeRoutine()
    {
        yield return new WaitForSeconds(3f);

        StopAllCoroutines();

        anim.applyRootMotion = false;
        anim.Play(idleAnimation);

        Destroy(agent);
        Destroy(GetComponent<Rigidbody>());
        Destroy(this);
    }
}