using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EngBossAI : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;

    public GameObject rocketPrefab;
    public int rocketVolume;
    public float rocketRadius;
    public float rocketInterval;
    public float rocketDelay;
    public float rocketSpeed;
    public GameObject flamethrowerPrefab;

    private bool attacking = false;
    private float angleOffset = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = gameObject.GetComponent<EnemyPathfinding>().target;
        angleOffset = Mathf.Deg2Rad * (360 / rocketVolume);

        InvokeRepeating("SelectAttack", 3f, 1f);
    }


    void SelectAttack()
    {
        Debug.Log("Selecting");
        if (!attacking)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < 0.1)
            {
                Debug.Log("Rockets");
                RocketAttack();
            }
            else if (rand < 0.3)
            {
                Debug.Log("Flamethrower");
                FlamethrowerAttack();
            }
        }
        
    }

    void RocketAttack()
    {
        attacking = true;
        rb.bodyType = RigidbodyType2D.Static;
        for (int i = 0; i < rocketVolume; i++)
        {
            Vector2 rocketOffset = new Vector2(rocketRadius * Mathf.Sin(i * angleOffset), rocketRadius * Mathf.Cos(i * angleOffset) + 1.5f);
            GameObject rocket = Instantiate(
                rocketPrefab,
                transform.position + (Vector3) rocketOffset,
                Quaternion.identity
                );
            rocket.GetComponent<RocketAI>().speed = rocketSpeed;
            rocket.GetComponent<RocketAI>().targ = target;
            rocket.GetComponent<RocketAI>().timer = Time.time + (i * rocketInterval) + rocketDelay;
        }
        Invoke("unFreeze", rocketDelay + rocketInterval * rocketVolume);
    }

    void unFreeze()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        attacking = false;
    }

    void FlamethrowerAttack()
    {
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        Vector3 offset = new Vector3(0, 1.5f, 0) + (Vector3)direction * 5;
        GameObject flamethrower = Instantiate(
            flamethrowerPrefab,
            transform.position + offset,
            Quaternion.identity
            );
        flamethrower.transform.parent = transform;
        flamethrower.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        flamethrower.GetComponent<Flamethrower>().offset = offset;
        Destroy(flamethrower, 1f);
    }

    private void FixedUpdate()
    {
        if (!target)
        {
            try
            {
                target = GameObject.FindGameObjectWithTag("Player").transform;
            }
            catch { }
        }
    }
}
