using UnityEngine;
using DG.Tweening;

public class CollectState : IEnemyState
{
    private EnemyAI enemy;
    private Tween moveTween;
    private Tween scaleTween;
    private Tween rotateTween;
    private Tween collectPulseTween; // Tween for pulsing effect during collection
    private float trailTimer = 0f; // Timer for spawning trail effect
    private const float TRAIL_INTERVAL = 0.2f; // Interval for spawning trail effect

    public CollectState(EnemyAI enemy) { this.enemy = enemy; }

    public void Enter()
    {
        // Stop all existing DOTween animations
        enemy.transform.DOKill();
        enemy.stateTimer = enemy.collectTime;
        enemy.animator.SetBool("isWalking", true);
        enemy.animator.SetBool("isCollecting", false);

        Debug.Log("Entering CollectState: Moving to collect item.");
        MoveToItem();
    }

    public void Update()
    {
        // Check if target item still exists
        if (enemy.targetItem == null)
        {
            Debug.Log("Target item lost, switching to IdleState.");
            enemy.ChangeState(new IdleState(enemy));
            return;
        }

        // Spawn trail effect during movement
        trailTimer -= Time.deltaTime;
        if (trailTimer <= 0 && moveTween != null && moveTween.IsPlaying())
        {
            SpawnTrail();
            trailTimer = TRAIL_INTERVAL;
        }
    }

    public void Exit()
    {
        // Clean up all tweens to prevent memory leaks
        moveTween?.Kill();
        scaleTween?.Kill();
        rotateTween?.Kill();
        collectPulseTween?.Kill();
        Debug.Log("Exiting CollectState: Stopping collection.");
    }

    private void MoveToItem()
    {
        if (enemy.targetItem == null) return;

        // Calculate distance and duration for smooth movement
        float distance = Vector3.Distance(enemy.transform.position, enemy.targetItem.position);
        float moveDuration = Mathf.Max(distance / enemy.walkSpeed, 0.5f); // Ensure minimum duration

        Debug.Log($"Moving to item at {enemy.targetItem.position}, distance: {distance}, duration: {moveDuration}");

        // Scale oscillation effect during movement
        scaleTween = enemy.transform.DOScale(0.8f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetDelay(0.2f); // Added delay for scale animation
        Debug.Log("Scale tween started with Yoyo loop and 0.2s delay.");

        // Rotate to face the target item
        rotateTween = enemy.transform.DOLookAt(enemy.targetItem.position, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetDelay(0.1f); // Added delay for rotation
        Debug.Log("Rotation tween started to face item with 0.1s delay.");

        // Change color to yellow during movement
        if (enemy.TryGetComponent<Renderer>(out var rend))
        {
            rend.material.DOColor(Color.yellow, moveDuration)
                .SetDelay(0.2f); // Added delay for color change
            Debug.Log("Color tween started to change to yellow with 0.2s delay.");
        }

        // Move to the target item
        moveTween = enemy.transform.DOMove(enemy.targetItem.position, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetDelay(0.3f) // Added delay for movement
            .OnComplete(() =>
            {
                // Reset animations and scale
                enemy.transform.DOKill();
                enemy.transform.localScale = Vector3.one;

                enemy.animator.SetBool("isWalking", false);
                enemy.animator.SetBool("isCollecting", true);

                // Start pulsing effect during collection
                collectPulseTween = enemy.transform.DOScale(1.1f, 0.3f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutQuad)
                    .SetDelay(0.2f); // Added delay for pulsing effect
                Debug.Log("Pulsing scale tween started during collection with 0.2s delay.");

                enemy.stateTimer = enemy.collectTime;

                // Spawn dust effect when reaching the item
                if (enemy.dustPrefab != null)
                {
                    GameObject dust = Object.Instantiate(enemy.dustPrefab, enemy.transform.position, Quaternion.identity);
                    Object.Destroy(dust, 1f);
                    Debug.Log("Dust effect spawned at item position.");
                }

                Debug.Log("Reached item, starting CollectRoutine.");
                enemy.StartCoroutine(CollectRoutine());
            });

        trailTimer = TRAIL_INTERVAL; // Reset trail timer
    }

    private System.Collections.IEnumerator CollectRoutine()
    {
        // Wait for collection time to complete
        while (enemy.stateTimer > 0)
        {
            enemy.stateTimer -= Time.deltaTime;
            yield return null;
        }

        // Destroy the collected item
        if (enemy.targetItem != null)
            Object.Destroy(enemy.targetItem.gameObject);

        Debug.Log("Finished collecting, switching to SearchState.");
        enemy.ChangeState(new SearchState(enemy));
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


