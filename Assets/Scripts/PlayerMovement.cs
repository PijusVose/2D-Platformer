using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float horizontalSpeed = 5f;

    [SerializeField]
    private float jumpPower = 25f;
    [SerializeField]
    private float attackMinDelay = 0.25f;
    [SerializeField]
    private float attackMaxDelay = 0.5f;

    public LayerMask groundMask;

    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float horizontalAxis;
    private float distToGround;
    private float lastAttackTime;
    private int attackCount = 0;
    private bool isBlocking;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        distToGround = (col.size.y / 2) + 0.02f;
    }

    public void OnMovement(InputValue value)
    {
        horizontalAxis = value.Get<float>();
    }

    public void OnJump() 
    {
        if (IsGrounded())
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);

            animator.SetTrigger("Jump");
        }
    }

    private bool IsGrounded()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, distToGround, groundMask, -1f, 1f)) 
        {
            return true;
        }

        return false;
    }

    private void Animate() {
        animator.SetFloat("AirSpeedY", rb.velocity.y);

        // Check for horizontal movement.
        if (horizontalAxis != 0f) {
            animator.SetInteger("AnimState", 1);

            if (horizontalAxis > 0f)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
        else 
        {
            animator.SetInteger("AnimState", 0);
        }

        // Check if grounded.
        if (IsGrounded())
        {
            animator.SetBool("Grounded", true);
        }
        else
        {
            animator.SetBool("Grounded", false);
        }
    }

    public void OnAttack() {
        float timeDiff = Time.time - lastAttackTime;
        if (timeDiff >= attackMinDelay)
        {
            lastAttackTime = Time.time;

            if (timeDiff <= attackMaxDelay)
            {
                attackCount++;
                if (attackCount > 3)
                {
                    attackCount = 1;
                }
            }
            else 
            {
                attackCount = 1;
            }
            
            animator.SetTrigger("Attack" + attackCount.ToString());
        } 
    }

    public void OnBlock() 
    {
        isBlocking = !isBlocking;

        if (isBlocking)
        {
            animator.SetTrigger("Block");
        }

        animator.SetBool("IdleBlock", isBlocking);
    }

    private void FixedUpdate() 
    {
        rb.velocity = new Vector2(horizontalAxis * horizontalSpeed, rb.velocity.y);
    }

    private void Update() {
        Animate();
    }
}
