using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerRoll : MonoBehaviour
{
    public int dmg = -3; // cantidad de daño que se quiere hacer

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player")
        {
            collision.gameObject.GetComponent<playerMovement>().changeHealth(dmg);
        }
           
    }





}
