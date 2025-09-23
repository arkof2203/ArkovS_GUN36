using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Search, Collect }
    private State currentState;

    [Header("Настройки")]
    public float idleTime = 5f;
    public float searchRadius = 10f;
    public float detectRadius = 5f;
    public float collectTime = 2f;

    [Header("Скорости")]
    public float walkSpeed = 2f;  // скорость ходьбы
    public float runSpeed = 5f;   // скорость бега

    [Header("Ссылки")]
    public Animator animator;
    private NavMeshAgent agent;

    private float stateTimer;
    private Transform targetItem;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Idle;
        stateTimer = idleTime;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleUpdate();
                break;
            case State.Search:
                SearchUpdate();
                break;
            case State.Collect:
                CollectUpdate();
                break;
        }
    }

    // ---------------- Idle ----------------
    void IdleUpdate()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isCollecting", false);

        if (agent.isOnNavMesh)
            agent.ResetPath();

        stateTimer -= Time.deltaTime;

        FindNearestItem();
        if (targetItem != null)
        {
            ChangeState(State.Collect);
        }
        else if (stateTimer <= 0)
        {
            ChangeState(State.Search);
        }
    }

    // ---------------- Search ----------------
    void SearchUpdate()
    {
        FindNearestItem();
        if (targetItem != null)
        {
            ChangeState(State.Collect);
            return;
        }

        // движение по случайной точке
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * searchRadius;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        animator.SetBool("isWalking", true);
        animator.SetBool("isCollecting", false);
    }


    // ---------------- Collect ----------------
    void CollectUpdate()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isCollecting", false);

        if (targetItem == null)
        {
            ChangeState(State.Idle);
            return;
        }

        agent.speed = walkSpeed;
        agent.SetDestination(targetItem.position);

        float distance = Vector3.Distance(transform.position, targetItem.position);
        if (distance <= 1.5f)
        {
            // enemy достиг предмета
            agent.ResetPath();
            animator.SetBool("isWalking", false);
            animator.SetBool("isCollecting", true);

            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                Destroy(targetItem.gameObject);
                ChangeState(State.Search); // идём искать следующий предмет
            }
        }
    }


    // ---------------- Служебные ----------------
    void ChangeState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                stateTimer = idleTime;
                if (agent.isOnNavMesh) agent.ResetPath();
                break;
            case State.Search:
                break;
            case State.Collect:
                stateTimer = collectTime;
                break;
        }
    }

    void FindNearestItem()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius);
        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Item"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.transform;
                }
            }
        }

        targetItem = nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}



