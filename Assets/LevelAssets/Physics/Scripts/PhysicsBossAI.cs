using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBossAI : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private LineRenderer line;
    private Vector2 direction;
    private bool attacking = false;
    private Vector2 offset;

    public float damage = 1f;
    public float range = 100f;
    public float chargeTime = 2f;
    public float yOffset = 3f;
    public float freq = 0.2f;
    public LayerMask layermask;
    public Transform firePoint;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
        offset = new Vector2(0, yOffset);
        if (!firePoint)
        {
            firePoint = transform;
        }

        InvokeRepeating("Attack", 0, 1f);
    }

    void Attack()
    {
        if (Random.Range(0f, 1f) < freq && !attacking && target)
        {
            rb.bodyType = RigidbodyType2D.Static;
            attacking = true;
            direction = ((Vector2)(target.position + (Vector3) offset - firePoint.position)).normalized;
            line.SetPosition(0, firePoint.position);
            line.SetPosition(1, (Vector2) firePoint.position + direction * range);
            line.startWidth = 0.2f;
            line.endWidth = 0.2f;
            line.enabled = true;
            Invoke("Damage", chargeTime);
        }
    }

    void Damage()
    {
        line.startWidth = 0.8f;
        line.endWidth = 0.8f;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, range, layermask);
        if (hit)
        {
            CharacterController2D player = hit.collider.gameObject.GetComponent<CharacterController2D>();
            if (player)
            {
                player.Damage(damage);
            }
        }
        Invoke("EndAttack", 1.5f * chargeTime);
    }

    void EndAttack()
    {
        line.enabled = false;
        attacking = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

    }

    // Update is called once per frame
    void Update()
    {
        if (!target)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
