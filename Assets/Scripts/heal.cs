using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heal : MonoBehaviour
{
    public playerMovement player;
    public int hp = 10; // cantidad de vida que se quiere sumar

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player" && player.currentHealth < player.maxHealth) // arreglar el overheal
        {
            collision.gameObject.GetComponent<playerMovement>().changeHealth(hp);
            hp = 0;
        }

    }
}
