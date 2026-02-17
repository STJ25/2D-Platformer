using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField]
    private float currentHealth;

    [Header("Animation")]
    public Animator animator; // ← animation code

    [Header("Enrage Settings")]
    public bool enraged = false;
    public float enragedThreshold = 30f;
    public BossAI bossAI;

    [Header("UI Elements")]
    public Image healthFillImage;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthUI();

        if (!enraged && currentHealth <= enragedThreshold)
        {
            EnterEnragedState();
        }

        if (currentHealth > 0)
        {
            animator.SetTrigger("hurt"); // ← animation code
        }
        else
        {
            Die();
        }

        
    }

    void EnterEnragedState()
    {
        enraged = true;
        animator.SetTrigger("enraged"); // ← animation code
        bossAI.EnterEnragedState();
    }

    void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("death"); // ← animation code

        MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            if (script != this)
                script.enabled = false;
        }

        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            col.enabled = false;
        }

        Destroy(gameObject, 3f);
    }
}
