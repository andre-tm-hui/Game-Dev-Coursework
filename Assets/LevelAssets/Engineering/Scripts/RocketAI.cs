using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketAI : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform targ;
    public float speed;
    public float timer;
    private bool fired = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void fire(Transform target, float rocketSpeed, float launchDelay)
    {
        targ = target;
        speed = rocketSpeed;
        timer = Time.time + launchDelay;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (Time.time > timer && !fired)
        {
            Destroy(gameObject, 5f);
            fired = true;
            Vector2 direction = (Vector2) targ.position - rb.position;
            direction = direction.normalized;
            rb.velocity = direction * speed;
            transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        }

        if (!targ)
        {
            try
            {
                targ = GameObject.FindGameObjectWithTag("Player").transform;
            }
            catch { }
        }
    }
}
