using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 20f;
    public int damage = 50;
    public float lifetime = 3f;
    public float gravity = 0f;
    public float force = 100f;

    private Vector2 direction;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (Vector2)((worldMousePos - transform.position));
        direction.Normalize();
        
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);

        rb.velocity = direction * speed;
        rb.gravityScale = gravity;
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D hitInfo)
    {
        EnemyBasicAI enemy = hitInfo.gameObject.GetComponent<EnemyBasicAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, direction, force);
        }
        Ball ball = hitInfo.gameObject.GetComponent<Ball>();
        if (ball != null)
        {
            ball.Die();
        }
        rb.bodyType = RigidbodyType2D.Static;
        animator.SetBool("Collided", true);
        if (animator.GetBool("Railgun"))
        {
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
