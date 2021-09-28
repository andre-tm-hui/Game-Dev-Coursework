using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 35f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D>();
        if (player)
        {
            player.Damage(0.5f);
        }
        Destroy(gameObject);
    }
}
