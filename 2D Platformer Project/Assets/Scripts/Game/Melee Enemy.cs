using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Properties")]
    [SerializeField] private float attackCoolDown;
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;

    [Header("References")]
    [SerializeField] private CapsuleCollider2D capsuleCollider;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator;
    private EnemyPatrol enemyPatrol;
    private float coolDowntimer = Mathf.Infinity;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }
    // Update is called once per frame
    void Update()
    {
        coolDowntimer += Time.deltaTime;
        if(PlayerInSight())
        {
            if(coolDowntimer >= attackCoolDown)
            {
                //attack
                coolDowntimer = 0;
                animator.SetTrigger("Attack");
            }
        }

        //enemy patrol logic
        if(enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(capsuleCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
           new Vector3(capsuleCollider.bounds.size.x * range,capsuleCollider.bounds.size.y,capsuleCollider.bounds.size.z),
            0, Vector2.left,0,playerLayer);

        if(hit.collider != null)
            playerHealth = hit.transform.GetComponent<PlayerHealth>();

        return hit.collider != null;
    }

    void DamagePlayer()
    {
        if (PlayerInSight())
            playerHealth.TakeDamage(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(capsuleCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(capsuleCollider.bounds.size.x * range, capsuleCollider.bounds.size.y, capsuleCollider.bounds.size.z));
    }
}
