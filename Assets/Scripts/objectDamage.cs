using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectDamage : MonoBehaviour
{
    public playerMovement player;
    int dmg = -1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player")
        {
            collision.gameObject.GetComponent<playerMovement>().changeHealth(dmg);
        }
           
    }
    
}
