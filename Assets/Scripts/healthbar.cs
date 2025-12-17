using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthbar : MonoBehaviour
{
    public Image hp;
    public Sprite[]sps;
    SpriteRenderer sr;
    public playerMovement player;
    void Start()
    {
        hp = GetComponent<Image>();
    }

    public void changeSpriteHealth()
    {
        if(player.currentHealth == player.maxHealth) hp.sprite = sps[0];
        if(player.currentHealth <= 0) hp.sprite = sps[player.maxHealth];
        else
        {
            hp.sprite = sps[player.maxHealth - player.currentHealth];
        }
    }
}
