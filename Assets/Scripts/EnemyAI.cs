using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    public float idleTime = 5f; // Duration of idle state
    public float searchRadius = 10f; // Radius for searching patrol points
    public float detectRadius = 5f; // Radius for detecting items
    public float collectTime = 2f; // Time to collect an item

    [Header("Speeds")]
    public float walkSpeed = 2f; // Speed of walking movement

    [Header("References")]
    public Animator animator; // Reference to the Animator component

    [Header("Patrol Points")]
    public Transform[] patrolPoints; // Array of patrol points for SearchState
    [HideInInspector] public int currentPointIndex = 0; // Current patrol point index

    [HideInInspector] public float stateTimer; // Timer for state duration
    [HideInInspector] public Transform targetItem; // Target item to collect

    [Header("Effects")]
    public GameObject dustPrefab; // Prefab for dust effect
    public GameObject trailPrefab; // Prefab for trail particle effect during movement

    private IEnemyState currentState;

    void Start()
    {
        // Initialize with IdleState
        ChangeState(new IdleState(this));
    }

    void Update()
    {
        // Update the current state
        currentState?.Update();
    }

    public void ChangeState(IEnemyState newState)
    {
        // Exit current state and enter new state
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void FindNearestItem()
    {
        // Find the nearest item within detectRadius
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
        // Draw detection radius in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        // Draw patrol points and lines between them
        Gizmos.color = Color.green;
        if (patrolPoints != null)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);
                if (i + 1 < patrolPoints.Length)
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
        }
    }
}




