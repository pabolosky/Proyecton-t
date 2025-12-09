using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float horizontalInput;
    float moveSpeed = 5f;
    bool isFacingRight = false;
    
    float rollPower = 1f;
    bool isRolling = false;

    float jumpPower = 5f;
    bool isGrounded = false;

    Rigidbody2D rb;
    Animator animator;
    bool isBusy;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        FlipSprite();

        if (isBusy) return;

        // Maybe quitar el isGrounded para doble salto
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isGrounded = false;
            animator.SetBool("isJumping", !isGrounded);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            isRolling = true;
            animator.SetTrigger("Roll");
            rb.velocity = new Vector2((isFacingRight ? 1 : -1) * rollPower, rb.velocity.y);
        }
    }

    private void FixedUpdate()
    {
        if (!isRolling)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }

        animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    void FlipSprite()
    {
        if (isRolling) return;

        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true;
        animator.SetBool("isJumping", !isGrounded);
    }

    public void EndRoll()
    {
        isRolling = false;
    }

}
