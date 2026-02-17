using UnityEngine;

public class BossUITrigger : MonoBehaviour
{
    public GameObject boss;
    public GameObject UI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (boss == null)
                UI.SetActive(false);

            else if (boss != null)
                UI.SetActive(true);
        }
    }
}
