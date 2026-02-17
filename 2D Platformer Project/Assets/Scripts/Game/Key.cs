using UnityEngine;

public class Key : MonoBehaviour
{ 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerHealth>().GetKeys();
            Destroy(gameObject);
        }
    }
}
