using UnityEngine;

public class Door : MonoBehaviour
{
    public PlayerHealth playerHealth; 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth.keyCount == 4)
                Destroy(gameObject);
            else
                return;
        }
    }
}
