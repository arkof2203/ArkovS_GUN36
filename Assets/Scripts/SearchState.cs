using UnityEngine;
using DG.Tweening;

public class SearchState : IEnemyState
{
    private EnemyAI enemy;
    private Tween moveTween;
    private Tween scaleTween;
    private float checkItemTimer = 0f;
    private const float CHECK_ITEM_INTERVAL = 0.5f; // Interval for checking nearby items
    private float trailTimer = 0f; // Timer for spawning trail effect
    private const float TRAIL_INTERVAL = 0.2f; // Interval for spawning trail effect

    public SearchState(EnemyAI enemy) { this.enemy = enemy; }

    public void Enter()
    {
        // Set up animator for walking state
        enemy.animator.SetBool("isWalking", true);
        enemy.animator.SetBool("isCollecting", false);

        Debug.Log("Entering SearchState: Enemy starts patrolling.");
        MoveToNextPoint();
    }

    public void Update()
    {
        // Update timer for checking nearby items
        checkItemTimer -= Time.deltaTime;
        if (checkItemTimer <= 0)
        {
            // Check for the nearest item to collect
            enemy.FindNearestItem();
            if (enemy.targetItem != null)
            {
                Debug.Log("Target item found, switching to CollectState.");
                enemy.ChangeState(new CollectState(enemy));
            }
            checkItemTimer = CHECK_ITEM_INTERVAL;
        }

        // Spawn trail effect at regular intervals during movement
        trailTimer -= Time.deltaTime;
        if (trailTimer <= 0 && moveTween != null && moveTween.IsPlaying())
        {
            SpawnTrail();
            trailTimer = TRAIL_INTERVAL;
        }
    }

    public void Exit()
    {
        // Clean up tweens to prevent memory leaks
        moveTween?.Kill();
        scaleTween?.Kill();
        Debug.Log("Exiting SearchState: Stopping patrolling.");
    }

    private void MoveToNextPoint()
    {
        // Validate references to avoid null errors
        if (enemy == null || enemy.transform == null) return;
        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0) return;

        Transform target = enemy.patrolPoints[enemy.currentPointIndex];
        if (target == null) return;

        // Calculate distance and duration for smooth movement
        float distance = Vector3.Distance(enemy.transform.position, target.position);
        float moveDuration = Mathf.Max(distance / enemy.walkSpeed, 0.5f); // Ensure minimum duration of 0.5 seconds

        Debug.Log($"Moving to patrol point {enemy.currentPointIndex} at {target.position}, distance: {distance}, duration: {moveDuration}");

        // Scale oscillation effect with a slight delay for smoother start
        scaleTween?.Kill();
        scaleTween = enemy.transform.DOScale(0.8f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetDelay(0.2f); // Added delay for scale animation
        Debug.Log("Scale tween started with Yoyo loop and 0.2s delay.");

        // Move to the next patrol point with a linear easing
        moveTween = enemy.transform.DOMove(target.position, moveDuration)
            .SetEase(Ease.Linear)
            .SetDelay(0.3f) // Added delay for movement start
            .OnComplete(() =>
            {
                // Spawn dust effect when reaching the point
                if (enemy.dustPrefab != null)
                {
                    GameObject dust = Object.Instantiate(enemy.dustPrefab, enemy.transform.position, Quaternion.identity);
                    Object.Destroy(dust, 1f);
                    Debug.Log("Dust effect spawned at patrol point.");
                }

                // Move to the next patrol point in the array
                enemy.currentPointIndex = (enemy.currentPointIndex + 1) % enemy.patrolPoints.Length;
                Debug.Log($"Reached point {enemy.currentPointIndex}, moving to next.");
                MoveToNextPoint();
            });

        trailTimer = TRAIL_INTERVAL; // Reset trail timer
    }

    private void SpawnTrail()
    {
        // Spawn trail particle effect if prefab is assigned
        if (enemy.trailPrefab != null)
        {
            // Position the trail slightly below the enemy (e.g., at ground level)
            Vector3 trailPosition = enemy.transform.position + Vector3.down * 0.1f; // Adjust offset as needed
            GameObject trail = Object.Instantiate(enemy.trailPrefab, trailPosition, Quaternion.identity);

            // Get Particle System component and destroy after its duration
            ParticleSystem ps = trail.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                float duration = ps.main.duration;
                Object.Destroy(trail, duration); // Destroy after particle system duration
            }
            else
            {
                Object.Destroy(trail, 1f); // Fallback duration if no Particle System
            }

            Debug.Log("Trail particle effect spawned during movement.");
        }
    }
}

