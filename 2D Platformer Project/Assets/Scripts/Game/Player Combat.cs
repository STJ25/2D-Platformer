using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public LayerMask enemyLayer;

    [Header("Attack 1")]
    public Transform attackPoint_1;
    public float attackRange_1;
    public float attackDamage_1 = 10f;
    public float attackRate_1 = 2f;
    public AudioSource attackSource_1;

    [Header("Attack 2")]
    public Transform attackPoint_2;
    public float attackRange_2;
    public float attackDamage_2 = 20f;
    public float attackRate_2 = 1f;
    public AudioSource attackSource_2;

    private float nextAttackTime = 0f;
    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack_1();
                nextAttackTime = Time.time + 1f/ attackRate_1;
            }

            if (Input.GetMouseButtonDown(1))
            {
                Attack_2();
                nextAttackTime = Time.time + 1f / attackRate_2;
            }
        }
        
    }

    void Attack_1()
    {
        //play animation
        animator.SetTrigger("Attack_1");

        //play Sound
        attackSource_1.Play();

        //detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint_1.position,attackRange_1,enemyLayer);
        foreach(Collider2D enemy in hitEnemies)
        {
            //damage function here
            Debug.Log("enemy hit with attack 1");
            if (enemy.gameObject.CompareTag("Boss"))
                enemy.GetComponent<BossHealth>().TakeDamage(attackDamage_1);
            else
            enemy.GetComponent<EnemyHeath>().TakeDamage(attackDamage_1);
        }
    }

    void Attack_2()
    {
        //play animation
        animator.SetTrigger("Attack_2");

        //play Sound
        attackSource_2.Play();

        //detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint_2.position, attackRange_2, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            //damage function here
            Debug.Log("enemy hit with attack 2");
            if (enemy.gameObject.CompareTag("Boss"))
                enemy.GetComponent<BossHealth>().TakeDamage(attackDamage_2);
            else
            enemy.GetComponent<EnemyHeath>().TakeDamage(attackDamage_2);
        }
    }

    private void OnDrawGizmosSelected()
    {
        //attack 1
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint_1.position, attackRange_1);

        //attack 2
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint_2.position, attackRange_2);
    }
}
