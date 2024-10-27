using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : CharacterBase
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private  AnimatorManager animatorManager;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private EffectManager effectManager;

    private HealthManager healthManager;
    
    private float horizontalInput;
    private bool isSprinting;
    private bool isGrounded;
    private bool isJumping;
    private bool wasGrounded;
    private bool isAlive = true;
    
    protected override void Start()
    {
        base.Start();
        isJumping = false;
        healthManager = GetComponent<HealthManager>();
    }

    private void Update()
    {
        if (isAlive)
        {
            GetInput();
            HandleInput();
        }
        isGrounded = groundCheck.IsGrounded();
        
        if (isGrounded)
        {
            rb.gravityScale = 2f; 
        }
        
        FlipCharacter(horizontalInput);
        animatorManager.UpdateAnimations(
            Mathf.Abs(rb.velocity.x),
            rb.velocity.y,
            isGrounded,
            isSprinting);
        
        if (isGrounded && isJumping)
        {
            effectManager.PlayLandingEffect();
            isJumping = false;
            animatorManager.SetIdleAnimation();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            UIManager.Instance.TeleportUI(GameManager.Instance.GetActiveCheckpoints());
        }
    }

    public void HandleInput()
    {
        if (InputManager.JumpPressed() && isGrounded)
        {
            Jump(jumpForce);
        }

        if (InputManager.JumpReleased() && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void FixedUpdate()
    {
        Move(horizontalInput);
    }

    private void GetInput()
    {
        horizontalInput = InputManager.GetHorizontalInput();
        if (isGrounded)
        {
            isSprinting = InputManager.IsSprinting();
        };
        
        Debug.Log($"IsGrounded: {isGrounded}");
        
        if (InputManager.JumpPressed() && isGrounded && !isJumping)
        {
            Jump(jumpForce);
            isJumping = true;
            animatorManager.SetJumpAnimation();
        }
    }
    

    public override void Move(float moveInput)
    {
        float speed = isSprinting ? sprintSpeed : walkSpeed;
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    public override void Jump(float jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0,jumpForce),ForceMode2D.Impulse);
        rb.gravityScale = 3.5f;
    }

    private void FlipCharacter(float moveInput)
    {
        if (moveInput > 0 && !isFacingRight || moveInput < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    
    private void Die()
    {
        isAlive = false;
        rb.velocity = Vector2.zero;
    }
    
    public void Revive()
    {
        isAlive = true;
    }
}