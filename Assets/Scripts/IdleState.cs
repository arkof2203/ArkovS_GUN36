using DG.Tweening;
using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyAI enemy;

    public IdleState(EnemyAI enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.transform.DOKill(); // Останавливаем все анимации DOTween
        enemy.stateTimer = enemy.idleTime;

        // Сброс анимаций
        enemy.animator.SetBool("isWalking", false);
        enemy.animator.SetBool("isCollecting", false);
    }

    public void Update()
    {
        enemy.stateTimer -= Time.deltaTime;

        // Проверяем предметы
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


