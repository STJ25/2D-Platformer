using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    public enum BossState { Idle, Chase, MeleeAttack, RangedAttack }
    public BossState currentState = BossState.Idle;

    [Header("References")]
    public Transform player;
    public Animator animator; // ← animation code
    public SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("Detection Settings")]
    public float detectionRange = 10f;

    [Header("Melee Attack Settings")]
    public float baseAttackDamage = 10f;
    public float meleeRange = 1.5f;
    public float attackRecoveryTime = 1f;
    public Transform attackPoint;
    public LayerMask playerLayer;

    [Header("Ranged Attack Settings")]
    public float rangedAttackRange = 6f;
    public float rangedAttackCooldown = 2f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 5f;
    public float projectileLifetime = 3f;

    [Header("Enraged Modifiers")]
    public float enragedMoveSpeed = 5f;
    public float enragedRecoveryTime = 0.5f;
    public float enragedRangedCooldown = 0.8f;
    public float enragedAttackDamage = 20f;

    private bool isEnraged = false;

    [Header("Visuals")]
    public Color enragedFlashColor = Color.red;
    public float flashInterval = 0.2f;

    private Rigidbody2D rb;
    private Vector2 movementInput = Vector2.zero;
    private bool isRecoveringFromAttack = false;
    private float recoveryEndTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                movementInput = Vector2.zero;

                if (IsPlayerInMeleeRange())
                {
                    EnterMeleeAttackState();
                }
                else if (IsPlayerInRangedRange())
                {
                    DecideRangedOrChase();
                }
                else if (IsPlayerInDetectionRange())
                {
                    currentState = BossState.Chase;
                    //animator.SetBool("isRunning", true); // ← animation code
                }
                break;

            case BossState.Chase:
                if (IsPlayerInMeleeRange())
                {
                    EnterMeleeAttackState();
                }
                else if (IsPlayerInRangedRange())
                {
                    DecideRangedOrChase();
                }
                else if (IsPlayerInDetectionRange())
                {
                    Vector2 direction = (player.position - transform.position).normalized;
                    movementInput = new Vector2(direction.x, 0);
                    //animator.SetBool("isRunning", true); // ← animation code
                    FacePlayer();
                }
                else
                {
                    currentState = BossState.Idle;
                    movementInput = Vector2.zero;
                    //animator.SetBool("isRunning", false); // ← animation code
                }
                break;

            case BossState.MeleeAttack:
            case BossState.RangedAttack:
                movementInput = Vector2.zero;
                FacePlayer();

                if (isRecoveringFromAttack && Time.time >= recoveryEndTime)
                {
                    isRecoveringFromAttack = false;

                    if (IsPlayerInMeleeRange())
                    {
                        EnterMeleeAttackState();
                    }
                    else if (IsPlayerInRangedRange())
                    {
                        DecideRangedOrChase();
                    }
                    else if (IsPlayerInDetectionRange())
                    {
                        currentState = BossState.Chase;
                        //animator.SetBool("isRunning", true); // ← animation code
                    }
                    else
                    {
                        currentState = BossState.Idle;
                        //animator.SetBool("isRunning", false); // ← animation code
                    }
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            Vector2 newPos = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
    }

    void FacePlayer()
    {
        if (player != null)
        {
            spriteRenderer.flipX = player.position.x < transform.position.x;
        }
    }

    bool IsPlayerInDetectionRange()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    bool IsPlayerInMeleeRange()
    {
        return Vector2.Distance(transform.position, player.position) <= meleeRange;
    }

    bool IsPlayerInRangedRange()
    {
        float distance = Mathf.Abs(player.position.x - transform.position.x);
        float heightDiff = Mathf.Abs(player.position.y - transform.position.y);
        return distance <= rangedAttackRange && distance > meleeRange && heightDiff <= 1f;
    }

    void EnterMeleeAttackState()
    {
        currentState = BossState.MeleeAttack;
        isRecoveringFromAttack = true;
        recoveryEndTime = Time.time + attackRecoveryTime;
        //animator.SetBool("isRunning", false); // ← animation code
        animator.SetTrigger("attack"); // ← animation code
    }

    void DecideRangedOrChase()
    {
        if (Random.value > 0.5f)
        {
            EnterRangedAttackState();
        }
        else
        {
            currentState = BossState.Chase;
            //animator.SetBool("isRunning", true); // ← animation code
        }
    }

    void EnterRangedAttackState()
    {
        currentState = BossState.RangedAttack;
        isRecoveringFromAttack = true;
        recoveryEndTime = Time.time + attackRecoveryTime + rangedAttackCooldown;
        //animator.SetBool("isRunning", false); // ← animation code
        animator.SetTrigger("rangedAttack"); // ← animation code
        StartCoroutine(ShootProjectiles());
    }

    IEnumerator ShootProjectiles()
    {
        int projectileCount = isEnraged ? 5 : 3;

        for (int i = 0; i < projectileCount; i++)
        {
            GameObject proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();

            float dir = spriteRenderer.flipX ? -1f : 1f;
            rbProj.linearVelocity = new Vector2(dir * projectileSpeed, 0f);

            // Flip projectile to match facing direction
            Vector3 scale = proj.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir);
            proj.transform.localScale = scale;

            Destroy(proj, projectileLifetime);
            yield return new WaitForSeconds(0.3f); // delay between projectiles
        }

    }

    public void EnterEnragedState()
    {
        if (isEnraged) return;

        isEnraged = true;
        moveSpeed = enragedMoveSpeed;
        attackRecoveryTime = enragedRecoveryTime;
        rangedAttackCooldown = enragedRangedCooldown;
        StartCoroutine(FlashRedEffect());
        // Update other systems like melee damage if needed
        Debug.Log("Boss is now enraged!");
    }

    IEnumerator FlashRedEffect()
    {
        Color originalColor = spriteRenderer.color;

        while (isEnraged)
        {
            spriteRenderer.color = enragedFlashColor;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    // Call via animation event if needed
    public void DamagePlayer()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, meleeRange, playerLayer);
        if (hitPlayer != null)
        {
            float damage = isEnraged ? enragedAttackDamage : baseAttackDamage;
            Debug.Log("Player hit for " + damage + " damage!");

            hitPlayer.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, meleeRange);
        }

        Gizmos.color = Color.cyan;
        if (projectileSpawnPoint != null)
        {
            Gizmos.DrawWireSphere(transform.position, rangedAttackRange);
        }
    }
}
