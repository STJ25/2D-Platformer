using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    public enum State { Idle, Attacking }
    private State currentState = State.Idle;

    [Header("References")]
    public Transform player;
    public Animator animator; // ← animation code
    public Transform projectileSpawnPoint;
    public GameObject projectilePrefab;
    public SpriteRenderer spriteRenderer;

    [Header("Attack Settings")]
    public float attackRange = 8f;
    public float timeBetweenAttacks = 2f;
    public float projectileSpeed = 7f;
    public float projectileLifetime = 3f;

    private float attackTimer;

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (player != null)
            FacePlayer();

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attacking;
                    animator.SetTrigger("attack"); // ← animation code
                }
                break;

            case State.Attacking:
                attackTimer += Time.deltaTime;

                if (attackTimer >= timeBetweenAttacks)
                {
                    if (distanceToPlayer <= attackRange)
                    {
                        animator.SetTrigger("attack"); // ← animation code
                    }
                    else
                    {
                        currentState = State.Idle;
                    }

                    attackTimer = 0f;
                }
                break;
        }
    }

    void FacePlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }

    // This should be called via an Animation Event
    public void SpawnProjectile()
    {
        if (player == null) return;

        Vector2 direction = (player.position - projectileSpawnPoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;

        // Flip projectile to face the player
        Vector3 scale = proj.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
        proj.transform.localScale = scale;

        Destroy(proj, projectileLifetime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
