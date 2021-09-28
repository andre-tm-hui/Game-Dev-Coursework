using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LawBossAI : MonoBehaviour
{
    private GameObject target;
    private Collider2D[] colls;
    public List<Transform> positions = new List<Transform>();
    public GameObject hammerPrefab;
    public GameObject projectilePrefab;
    private Vector2 heightOffset = new Vector2(0, -13f);

    public float actionInterval = 8f;
    public float specialAttackFreq = 0.2f;
    private float timer = 0f;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool facingRight = true;
    private bool canFlip = true;

    public float maxProjectileForce = 1000f;
    // Start is called before the first frame update
    void Start()
    {
        timer = actionInterval + 1;
        target = GameObject.FindGameObjectWithTag("Player");
        colls = GetComponents<Collider2D>();

        InvokeRepeating("Dive", 2f, 8f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!target)
        {
            try
            {
                target = GameObject.FindGameObjectWithTag("Player");
            }
            catch { }
        }
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer < actionInterval / 8)
        {
            transform.position = Vector2.Lerp(startPosition, endPosition, timer / (actionInterval / 8));
        }

        if (target.transform.position.x > transform.position.x && !facingRight)
        {
            Flip();
        } else if (target.transform.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    void Dive()
    {
        canFlip = true;
        startPosition = transform.position;
        endPosition = (Vector2) transform.position + heightOffset;
        timer = 0;
        SetColliders(false);
        Invoke("EnableColliders", 3 * actionInterval / 8);
        Invoke("Move", actionInterval / 4);
    }

    void EnableColliders()
    {
        SetColliders(true);
    }
    void SetColliders(bool b)
    {
        foreach (Collider2D coll in colls)
        {
            coll.enabled = b;
        }
    }

    void Move()
    {
        startPosition = (Vector2) positions[Random.Range(0, positions.Count)].position + heightOffset;
        endPosition = startPosition - heightOffset;
        timer = 0;
        Invoke("Attack", actionInterval / 4);
    }

    void Attack()
    {
        if (Random.Range(0f, 1f) < specialAttackFreq)
        {
            StartCoroutine("Special");
        } else
        {
            canFlip = false;
            Invoke("Projectile", 0f);
            Invoke("Projectile", actionInterval / 8);
            Invoke("Projectile", actionInterval / 4);
        }
    }

    IEnumerator Special()
    {
        yield return new WaitForSeconds(actionInterval / 8);
        GameObject hammer = Instantiate(
            hammerPrefab,
            transform.position,
            Quaternion.identity
            );
        hammer.transform.parent = transform;
        hammer.transform.localRotation = Quaternion.Euler(0, 0, -45);
        hammer.transform.localPosition = new Vector3(1.23f, 5.65f, 0f);
        // spawn hammer prefab;
        yield return new WaitForSeconds(actionInterval / 8);
        canFlip = false;
        hammer.GetComponent<BossHammer>().HammerDown(actionInterval / 8);
        // rotate hammer prefab; -45 to -115
        yield return new WaitForSeconds(actionInterval / 8);
        Destroy(hammer);
        // destroy hammer prefab;
    }

    void Projectile()
    {
        Vector2 direction = target.transform.position - transform.position;
        Vector2 projectileDirection = new Vector2 (direction.x / Mathf.Abs(direction.x), Random.Range(0.5f, 2));
        GameObject projectile = Instantiate(
            projectilePrefab,
            transform.position + new Vector3(0f, 5.65f, 0f),
            Quaternion.identity
            );
        projectile.GetComponent<Rigidbody2D>().AddForce(projectileDirection * Random.Range(100, maxProjectileForce));
    }

    void Flip()
    {
        if (canFlip)
        {
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
        }
    }
}
