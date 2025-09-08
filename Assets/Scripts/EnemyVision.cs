using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("ֿאנאלוענû מבחמנא")]
    [SerializeField] private float viewRadius = 5f;
    private Transform enemyTransform;

    void Start()
    {
        enemyTransform = transform;
    }

    void OnDrawGizmosSelected()
    {
        if (enemyTransform == null)
            enemyTransform = transform;
        Vector3 viewCenter = enemyTransform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(viewCenter, viewRadius);
    }
}
