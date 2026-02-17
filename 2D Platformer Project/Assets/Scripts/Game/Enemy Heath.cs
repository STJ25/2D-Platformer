using UnityEngine;

public class EnemyHeath : MonoBehaviour
{
    public Animator animator;
    public float startingHealth = 100f;
    public float currentHealth;
    private bool dead;
    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float damage)
    {
        //hurt anim
        animator.SetTrigger("Hurt");
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth <= 0)
        {
            if (!dead)
            {
                Die();
                dead = true;
            }
        }
    }

    void Die() 
    {
        //disable movement
        Debug.Log("Enemy Dead");
        //die animation
        animator.SetBool("isDead",true);
        //disable collider
        GetComponent<Collider2D>().enabled = false;
        // disable enemy
        this.enabled = false;
        //destroy after like 3 sec
        Destroy(gameObject, 2f);

    }
}
