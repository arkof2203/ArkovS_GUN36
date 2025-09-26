using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Настройки")]
    public float idleTime = 5f;
    public float searchRadius = 10f;
    public float detectRadius = 5f;
    public float collectTime = 2f;

    [Header("Скорости")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("Ссылки")]
    public Animator animator;
    public NavMeshAgent agent;

    [HideInInspector] public float stateTimer;
    [HideInInspector] public Transform targetItem;

    private IEnemyState currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ChangeState(new IdleState(this));
    }

    void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void FindNearestItem()
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



