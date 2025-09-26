using UnityEngine;
using UnityEngine.AI;

public class SearchState : IEnemyState
{
    private EnemyAI enemy;

    public SearchState(EnemyAI enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.animator.SetBool("isWalking", true);
        enemy.animator.SetBool("isCollecting", false);
        enemy.agent.speed = enemy.walkSpeed;
    }

    public void Update()
    {
        enemy.FindNearestItem();
        if (enemy.targetItem != null)
        {
            enemy.ChangeState(new CollectState(enemy));
            return;
        }

        if (!enemy.agent.hasPath || enemy.agent.remainingDistance < 0.5f)
        {
            Vector3 randomPoint = enemy.transform.position + Random.insideUnitSphere * enemy.searchRadius;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, enemy.searchRadius, NavMesh.AllAreas))
            {
                enemy.agent.SetDestination(hit.position);
            }
        }
    }

    public void Exit() { }
}
