using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUpdate : MonoBehaviour
{
    public GameObject player;
    public Sprite[] HeartSprites3;
    public Sprite[] HeartSprites4;
    public Sprite[] HeartSprites5;
    public Image heartImage;

    private float health;
    private float lastHealth;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!player)
        {
            try
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            catch { }
        } else
        {
            health = player.GetComponent<CharacterController2D>().health;
            if (health != lastHealth && player)
            {
                lastHealth = health;
                if (player.GetComponent<CharacterController2D>().maxHealth == 3)
                {
                    heartImage.sprite = HeartSprites3[Mathf.RoundToInt(health * 2)];
                }
                else if (player.GetComponent<CharacterController2D>().maxHealth == 4)
                {
                    heartImage.sprite = HeartSprites4[Mathf.RoundToInt(health * 2)];
                }
                else if (player.GetComponent<CharacterController2D>().maxHealth == 5)
                {
                    heartImage.sprite = HeartSprites5[Mathf.RoundToInt(health * 2)];
                }

            }
        }
    }
}
