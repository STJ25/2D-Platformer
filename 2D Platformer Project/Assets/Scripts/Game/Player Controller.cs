using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Rigidbody2D rigidBody;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private CapsuleCollider2D capsuleCollider;

    [Header("Coin References")]
    public CoinCount coinRef;
    public AudioSource coinAudioSource;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float acceleration = 10f;
    public float deceleration = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    [SerializeField]
    private int jumpCount = 0;
    [SerializeField]
    private int noOfJumps = 0;
    [SerializeField]
    private bool isGrounded;
    public AudioSource jumpAudioSource;

    [Header("Wall Jump Settings")]
    public LayerMask wallLayer;
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public float wallJumpForce = 10f;
    public int maxWallJumps = 1;
    [SerializeField]
    private int wallJumpCount = 0;
    [SerializeField]
    private bool isTouchingWall;

    [Header("Wall Slide & Stick Settings")]
    public float wallSlideSpeed = 1.5f;
    public float wallStickTime = 0.25f;
    private float wallStickCounter;
    [SerializeField]
    private bool isWallSliding;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    [SerializeField]
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;

    [Header("Dash Trail Settings")]
    public GameObject afterimagePrefab;
    [ColorUsage(true, true)]
    public Color afterimageColor = new Color(1f, 1f, 1f, 0.5f);
    public float afterimageSpacing = 0.1f;
    public float afterimageLifetime = 0.3f;
    private float lastAfterimageTime;

    [Header("Roll Settings")]
    public float rollSpeed = 10f;
    public float rollDuration = 0.5f;
    public float rollCooldown = 1.0f;
    public bool isRolling = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private float horizontalInput;
    private bool isSprinting;
    private float rollCooldownTimer;

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        
        // ground check
        bool inAir = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        //check if touching wall
        isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right * transform.localScale.x, wallCheckDistance, wallLayer);

        //reset jump
        if (isGrounded)
        {
            jumpCount = 0;
            wallJumpCount = 0;
            isWallSliding = false;
        }

        // Handle wall sliding
        if (isTouchingWall && !isGrounded && rigidBody.linearVelocity.y < 0 && !isRolling)
        {
            isWallSliding = true;
            wallStickCounter = wallStickTime;
        }
        else
        {
            wallStickCounter -= Time.deltaTime;
            if (wallStickCounter <= 0)
            {
                isWallSliding = false;
            }
        }

        // Apply wall slide slow-fall
        if (isWallSliding)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, -wallSlideSpeed);
        }

        // Jump input
        if (Input.GetButtonDown("Jump"))
        {
            if (isWallSliding || (isTouchingWall && !isGrounded))
            {
                // Wall jump always overrides normal jump
                if (wallJumpCount < maxWallJumps)
                {
                    WallJump();
                }
            }
            else if (jumpCount < noOfJumps)
            {
                Jump();
                animator.SetBool("isJump", true);
            }
        }

        // Dash Input --- ( add this line if youy make an attack state and set it as true when attacking && !animator.GetBool("isAttack"))
        if (Input.GetKeyDown(KeyCode.E) && !isDashing && !isRolling && dashCooldownTimer <= 0f)
        {
            StartCoroutine(Dash());
        }

        //on landing
        if (!inAir && isGrounded)
        {
            OnLand();
        }

        //roll input
        if (Input.GetKeyDown(KeyCode.F) && !isRolling && isGrounded && rollCooldownTimer <= 0)
        {
            animator.SetTrigger("Roll");
            animator.SetBool("isRoll",true);
            StartCoroutine(Roll());
        }
        
        if (rollCooldownTimer > 0)
        {
            rollCooldownTimer -= Time.deltaTime;
        }

        //dash cooldown
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (isRolling) return;
        float targetSpeed = horizontalInput * (isSprinting ? sprintSpeed : walkSpeed);
        float speedDifference = targetSpeed - rigidBody.linearVelocity.x;
        float accelerationRate = Mathf.Abs(targetSpeed) > 0.1f ? acceleration : deceleration;
        float movement = speedDifference * accelerationRate;
        //Debug.Log(targetSpeed);
        animator.SetFloat("Walk", Mathf.Abs(targetSpeed));
        animator.SetFloat("Sprint", Mathf.Abs(targetSpeed));

        rigidBody.AddForce(Vector2.right * movement, ForceMode2D.Force);

        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
        }
    }

    private void Jump()
    {
        jumpAudioSource.Play();
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f); // Reset Y velocity for consistent jumps
        rigidBody.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        jumpCount++;
    }

    private void WallJump()
    {
        jumpAudioSource.Play();

        wallJumpCount++;
        jumpCount = 1; // Still allow a double jump after wall jump

        // Flip direction away from wall
        float wallDir = isTouchingWall ? transform.localScale.x : 1f;
        Vector2 jumpDir = new Vector2(-wallDir, 1f).normalized;

        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.AddForce(jumpDir * wallJumpForce, ForceMode2D.Impulse);

        // Flip character facing direction
        transform.localScale = new Vector3(-wallDir, 1, 1);

        isWallSliding = false;
        wallStickCounter = 0;

        animator.SetBool("isJump", true);
    }


    private System.Collections.IEnumerator Roll()
    {
        isRolling = true;

        rollCooldownTimer = rollCooldown;
        float oG_Gravity = rigidBody.gravityScale;
        Physics2D.IgnoreLayerCollision(8, 9, true);
        rigidBody.gravityScale = 0;

        float rollDirection = Mathf.Sign(transform.localScale.x);
        rigidBody.linearVelocity = new Vector2(rollDirection * rollSpeed, 0);

        yield return new WaitForSeconds(rollDuration);

        rigidBody.gravityScale = oG_Gravity;
        Physics2D.IgnoreLayerCollision(8, 9, false);
        isRolling = false;
        animator.SetBool("isRoll",false);
    }

    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        float originalGravity = rigidBody.gravityScale;
        rigidBody.gravityScale = 0f;
        rigidBody.linearVelocity = Vector2.zero;

        float dashDirection = transform.localScale.x;
        rigidBody.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        // Trigger dash animation (optional)
        //animator.SetTrigger("Dash");

        lastAfterimageTime = Time.time;

        float timer = 0f;
        while (timer < dashDuration)
        {
            if (Time.time >= lastAfterimageTime + afterimageSpacing)
            {
                SpawnAfterimage();
                lastAfterimageTime = Time.time;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        rigidBody.gravityScale = originalGravity;
        isDashing = false;
    }

    private void SpawnAfterimage()
    {
        GameObject ghost = Instantiate(afterimagePrefab, transform.position, transform.rotation);
        SpriteRenderer ghostSR = ghost.GetComponent<SpriteRenderer>();
        SpriteRenderer playerSR = GetComponent<SpriteRenderer>();

        if (ghostSR && playerSR)
        {
            ghostSR.sprite = playerSR.sprite;
            ghostSR.flipX = playerSR.flipX;
            ghostSR.color = afterimageColor;                     
            ghost.transform.localScale = transform.localScale;
        }

        Destroy(ghost, afterimageLifetime);
    }

    private void OnLand()
    {
        //Debug.Log("Player has landed.");
        animator.SetBool("isJump",false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            coinAudioSource.Play();
            Destroy(other.gameObject);
            coinRef.coinCount++;
        }
    }
    private void OnDrawGizmosSelected()
    {
        //ground check
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        //wall jump
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * transform.localScale.x * wallCheckDistance);

    }
}