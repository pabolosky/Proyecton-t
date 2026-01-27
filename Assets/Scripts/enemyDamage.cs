using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class enemyDamage : MonoBehaviour
{
    public enemyMove enemyM;
    public int currentHealth;
    public int maxHealth = 3;

    public int dmg = -1;

    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void changeHealthE(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentHealth <= 0)
        {
            enemyM.canMove = false;
            enemyM.animator.SetTrigger("isDeath");
            enemyM.rb.velocity = Vector2.zero;
        }
        else if (amount < 0)
        {
            hit();
        }
    }

    public void hit()
    {
        enemyM.animator.SetTrigger("isHit");
        enemyM.animator.SetBool("hit", true);
        enemyM.canMove = false;
        enemyM.rb.velocity = Vector2.zero;

        float knockbackForceX = 20f;
        float knockbackForceY = 4f;

        enemyM.rb.AddForce(
            new Vector2(-enemyM.direction * knockbackForceX, knockbackForceY),
            ForceMode2D.Impulse
        );
       
        StartCoroutine(stun(0.5f));

    }

    IEnumerator stun(float time)
    {
        yield return new WaitForSeconds(time);
        enemyM.animator.SetBool("hit", false);
        enemyM.canMove = true;
        
    }

    public void endeDeath()
    {
        Destroy(gameObject);
    }



}
