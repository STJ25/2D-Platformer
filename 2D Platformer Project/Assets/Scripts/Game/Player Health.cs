using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Details")]
    public float startingHealth = 100f;
    public float currentHealth;
    [SerializeField]
    private bool dead;

    [Header("I - Frames")]
    [SerializeField]
    private float iFramesDuration;
    [SerializeField]
    private float numberOfFlashes;

    [Header("References")]
    public AudioSource hurtSound;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public GameOver gameOverRef;
    

    [Header("Keys")]
    public int keyCount = 0;
    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float damage)
    {
        if (dead)
            return;

            animator.SetTrigger("Hurt");
            hurtSound.Play();
            StartCoroutine(Invulnerability());
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
       
        if (currentHealth <= 0)
        {
            Debug.Log("Player has Died");
            if(!dead)
            {
                dead = true;
                //anim ref for death anim
                animator.SetTrigger("Die");
                //get player movement and disable it
                GetComponent<PlayerController>().enabled = false;
                //get player combat and disable that and disable it
                GetComponent<PlayerCombat>().enabled = false;
                // show the game over screen
                gameOverRef.gameOver();
            }
        }
    }

    public void Heal(float heal)
    {
        currentHealth = Mathf.Clamp(currentHealth + heal, 0, startingHealth);
    }

    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        //i frame duration
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.8f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }

    public void GetKeys()
    {
        keyCount++;
    }
}
