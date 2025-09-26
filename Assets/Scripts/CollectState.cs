using UnityEngine;

public class CollectState : IEnemyState
{
    private EnemyAI enemy;

    public CollectState(EnemyAI enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.stateTimer = enemy.collectTime;
        enemy.agent.speed = enemy.walkSpeed;
        enemy.animator.SetBool("isWalking", true);
        enemy.animator.SetBool("isCollecting", false);
    }

    public void Update()
    {
        if (enemy.targetItem == null)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }

        enemy.agent.SetDestination(enemy.targetItem.position);

        float distance = Vector3.Distance(enemy.transform.position, enemy.targetItem.position);
        if (distance <= 1.5f)
        {
            enemy.agent.ResetPath();
            enemy.animator.SetBool("isWalking", false);
            enemy.animator.SetBool("isCollecting", true);

            enemy.stateTimer -= Time.deltaTime;
            if (enemy.stateTimer <= 0)
            {
                Object.Destroy(enemy.targetItem.gameObject);
                enemy.ChangeState(new SearchState(enemy));
            }
        }
    }

    public void Exit() { }
}

