using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyAI enemy;

    public IdleState(EnemyAI enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.stateTimer = enemy.idleTime;
        if (enemy.agent.isOnNavMesh)
            enemy.agent.ResetPath();

        enemy.animator.SetBool("isWalking", false);
        enemy.animator.SetBool("isCollecting", false);
    }

    public void Update()
    {
        enemy.stateTimer -= Time.deltaTime;

        enemy.FindNearestItem();
        if (enemy.targetItem != null)
        {
            enemy.ChangeState(new CollectState(enemy));
        }
        else if (enemy.stateTimer <= 0)
        {
            enemy.ChangeState(new SearchState(enemy));
        }
    }

    public void Exit() { }
}

