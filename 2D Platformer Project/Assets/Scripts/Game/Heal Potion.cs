using UnityEngine;

public class HealPotion : MonoBehaviour
{
    [SerializeField]
    private float healValue;
    public AudioSource healSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerHealth>().Heal(healValue);
            healSource.Play();
            Destroy(gameObject); 
        }
    }
}
