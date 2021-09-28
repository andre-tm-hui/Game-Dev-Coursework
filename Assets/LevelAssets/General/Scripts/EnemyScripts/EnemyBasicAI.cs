using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicAI : MonoBehaviour
{
    public int health = 30;
    public float invulnerabilityTime = 0.05f;
    private float invulnerabilityTimer = 0f;
    public Animation deathEffect;

    private Collider2D collider;
    private Rigidbody2D rb;
    private EnemyPathfinding pathfinder;
    public SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        pathfinder = GetComponent<EnemyPathfinding>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D>();
        if (player != null && collision.gameObject.layer == 9)
        {
            Debug.Log("damage from enemy");
            player.Damage(0.5f);
        }
    }

    private void FixedUpdate()
    {
        if (Time.time > invulnerabilityTimer)
        {
            if (pathfinder)
            {
                pathfinder.m_canMove = true;
            }
            if (sprite)
            {
                sprite.color = Color.white;
            }
        }
    }

    public void TakeDamage(int damage, Vector2 direction, float force)
    {
        Debug.Log("hit");
        if (invulnerabilityTimer < Time.time)
        {
            if (pathfinder)
            {
                pathfinder.m_canMove = false;
            }
            invulnerabilityTimer = Time.time + invulnerabilityTime;
            health -= damage;
            rb.AddForce(new Vector2((direction * force).x, 600f));
            Debug.Log(health);
            if (sprite)
            {
                sprite.color = Color.red;
            }
            GameObject.FindGameObjectWithTag("HUD").GetComponent<AudioSource>().Play();
            if (health <= 0)
            {
                Die();
            }
        }

    }

    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject, 0.3f);
    }
}
