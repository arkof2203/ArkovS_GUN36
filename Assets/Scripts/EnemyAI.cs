using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Обзор")]
    [SerializeField] private float viewRadius = 5f; 
    [SerializeField] private LayerMask targetMask; 
    [SerializeField] private LayerMask obstacleMask; 

    [Header("Движение")]
    [SerializeField] private float moveSpeed = 3f;
    private Transform player;
    private Vector3 viewCenter => transform.position;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(viewCenter, player.position);

        if (distanceToPlayer <= viewRadius)
        {
            Vector2 dirToPlayer = (player.position - viewCenter).normalized;
            RaycastHit2D hit = Physics2D.Raycast(viewCenter, dirToPlayer, viewRadius, obstacleMask);

            if (hit.collider == null)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

                float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 0f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(viewCenter, viewRadius);
    }
}


