using Unity.VisualScripting;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;

    [Header("Enemy Parameters")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Animator animator;

    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float idleDuration;
    private float idleTimer;
    private Vector3 initialScale;
    private bool movingLeft;

    private void Awake()
    {
        initialScale = enemy.localScale;
    }
    private void Update()
    {
        if (enemy == null)
        {
            this.enabled = false;
            return;
        }

        else
        {
            if (movingLeft)
            {
                if (enemy.position.x >= left.position.x)
                    MoveInDirection(-1);
                else
                    DirectionChange();
            }
            else
            {
                if (enemy.position.x <= right.position.x)
                    MoveInDirection(1);
                else
                    DirectionChange();
            }
        }
    }

    void DirectionChange()
    {
        animator.SetBool("Moving", false);

        idleTimer += Time.deltaTime;
        if (idleTimer > idleDuration)
            movingLeft = !movingLeft;
        
    }

    void MoveInDirection (int direction)
    {
        idleTimer = 0;
        // animate enemy 
        animator.SetBool("Moving",true);
        //face direction
        enemy.localScale = new Vector3 (Mathf.Abs(initialScale.x) * direction, initialScale.y, initialScale.z);

        //move in that direction
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * direction * speed, enemy.position.y,enemy.position.z);
    }

    private void OnDisable()
    {
        if (animator == null)
            return;

        animator.SetBool("Moving", false);
    }
}
