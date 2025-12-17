using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    float horizontalInput;
    float moveSpeed = 5f;
    bool isFacingRight = false;
    float dir = -1;
    float jumpPower = 5f;
    float rollPower = 4f;
    bool isGrounded = true;
    bool can2J = true;
    bool canMove = true;

    public int maxHealth = 8;
    public int currentHealth;

    public Transform atckPoint;
    public float atckRange = 1;
    public LayerMask enemyLayer;

    float timerA;
    float cdA = 0.5f;
    float timerR;
    float cdR = 0.7f;

    Rigidbody2D rb;
    Animator animator;
    public healthbar hp;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        FlipSprite();
        currentHealth = maxHealth;
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if(canMove) FlipSprite();

        if (timerA > 0) timerA -= Time.deltaTime;
        if (timerR > 0) timerR -= Time.deltaTime;

        // Maybe quitar el isGrounded para doble salto
        if (Input.GetButtonDown("Jump") && canMove)
        {
            if (isGrounded && animator.GetFloat("yVelocity") == 0)
            {
                jump();
            }
            else if(can2J)
            {
                doubleJump();
            }
        }
        if (Input.GetButtonDown("Attack") && isGrounded && canMove)
        {
            attack();                    
        }
        if (Input.GetButtonDown("Roll") && isGrounded && canMove)
        {
            roll();         
        }

    }


/****************************************************MOVEMENT****************************************************/
    private void FixedUpdate()
    {

        if (canMove) { 
        
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);       

            animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
            animator.SetFloat("yVelocity", rb.velocity.y);
        }
    }


/****************************************************FLIP_SPRITE****************************************************/
    void FlipSprite()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
            dir *= -1;
        }
    }


/****************************************************RESET_JUMP****************************************************/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground")
        {
            isGrounded = true;
            can2J = true;
            animator.SetBool("isJumping", !isGrounded);
            animator.SetBool("is2Jumping", !can2J);
        }
    }


/****************************************************JUMP****************************************************/
    public void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        isGrounded = false;
        animator.SetBool("isJumping", !isGrounded);
    }


/****************************************************DOUBLE_JUMP****************************************************/
    public void doubleJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        can2J = false;
        animator.SetBool("is2Jumping", !can2J);
    }


/****************************************************ATTACK****************************************************/
    public void attack()
    {
        if (timerA <= 0)
        {
            animator.SetBool("isAttacking", true);
            canMove = false;
            if (horizontalInput != 0 && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x + (-dir * animator.GetFloat("xVelocity")), rb.velocity.y);
            }

            Collider2D[] enemies = Physics2D.OverlapCircleAll(atckPoint.position, atckRange, enemyLayer);
            if (enemies.Length > 0) enemies[0].GetComponent<enemyDamage>().changeHealthE(-1);

            timerA = cdA;
        }
    }

    public void endAttack()
    {
        animator.SetBool("isAttacking", false);
        canMove = true;
    }


/****************************************************ROLL****************************************************/

    public void roll()
    {
        if (timerR <= 0)
        {
            animator.SetBool("isRolling", true);
            canMove = false;           

            rb.velocity = new Vector2(rb.velocity.x + (dir * (rollPower - animator.GetFloat("xVelocity") / 2)), rb.velocity.y);

            invencibility(true);

            timerR = cdR;
        }
    }
    public void endRoll()
    {
        animator.SetBool("isRolling", false);
        canMove = true;
        invencibility(false);
    }


/****************************************************HEALTH****************************************************/

    public void changeHealth(int amount)
    {
      
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        hp.changeSpriteHealth();
        
        if (currentHealth <= 0)
        {
            canMove = false;
            invencibility(true);
            animator.SetTrigger("isDead");

        }
        else if (amount < 0)
        {
            hit();
        }
        else if (amount > 0)
        {
            heal();
        }  

        Debug.Log("hit " + currentHealth);

    }

    public void endDeath()
    {
       // Destroy(gameObject);
    }

    public void heal()
    {
        // animación de curar
    }

    public void hit()
    {
        animator.SetBool("hit", true);
        canMove = false;
        invencibility(true);

        rb.velocity = Vector2.zero;

        float knockbackForceX = (animator.GetFloat("xVelocity") / 2f) + 1;
        float knockbackForceY = 3f;

        rb.AddForce(
            new Vector2(-dir * knockbackForceX, knockbackForceY),
            ForceMode2D.Impulse
        );

        StartCoroutine(stun(0.5f));
    }

    IEnumerator stun(float time)
    {
        yield return new WaitForSeconds(time);

        canMove = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.SetBool("isRolling", false);
        animator.SetBool("hit", false);
        invencibility(false);
    }


/****************************************************IFRAMES****************************************************/

    public void invencibility(bool b) 
    {
        Physics2D.IgnoreLayerCollision( LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), b);
    }
}
