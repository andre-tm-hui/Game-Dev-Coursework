using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 velocity;
    private int collisions = 0;
    public int maxCollisions = 3;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = velocity;
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisions++;
        CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D>();
        if (player)
        {
            player.Damage(0.5f);
            Destroy(gameObject);
        }
        if (collisions > 3)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        
    }

    public void Die()
    {
        maxCollisions--;
        if (maxCollisions == 0)
        {
            Destroy(gameObject);
        }
    }
}
