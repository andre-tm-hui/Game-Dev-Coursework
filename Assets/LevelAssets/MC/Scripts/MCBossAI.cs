using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCBossAI : MonoBehaviour
{
    public Transform target;
    public Sprite[] balls;
    public GameObject ballPrefab;
    public float ballSpeed = 20f;
    public Transform ballThrowPoint;

    private Collider2D[] colliders;
    private EnemyPathfinding main;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool targetAcquired = false;
    private bool charge = false;
    public float chargeDelay = 0.6f;
    public float chargeSpeed = 50f;
    private float chargeTime = 0.1f;
    private float delayTimer = 0f;
    private float chargeTimer = 0f;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
        main = GetComponent<EnemyPathfinding>();

        InvokeRepeating("SelectAttack", 3f, 1f);
    }

    void SelectAttack()
    {
        float rand = Random.Range(0f, 1f);
        if (rand < 0.1)
        {
            Debug.Log("Charge");
            ChargeAttack();
        } else if (rand < 0.8 && !charge)
        {
            Debug.Log("Ball");
            BallAttack();
        }
    }

    void BallAttack()
    {
        int rand = Mathf.RoundToInt(Random.Range(0f, balls.Length - 1));
        GameObject ball = Instantiate(
            ballPrefab,
            ballThrowPoint.position,
            Quaternion.identity);
        ballPrefab.GetComponent<SpriteRenderer>().sprite = balls[rand];
        Vector2 direction = ((Vector2)(target.position - ball.transform.position)).normalized;
        //Debug.Log(direction);
        ball.GetComponent<Ball>().velocity = direction * ballSpeed;
    }

    void ChargeAttack()
    {
        charge = true;
        main.m_canMove = false;
        //toggleColliders();
        rb.bodyType = RigidbodyType2D.Static;
        startPosition = transform.position;
        chargeTime = Vector2.Distance(startPosition, target.position) / chargeSpeed;
        delayTimer = Time.time + chargeDelay;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!target)
        {
            try
            {
                target = GameObject.FindGameObjectWithTag("Player").transform;
            }
            catch { }
        }
        if (charge && Time.time > delayTimer && target)
        {
            if (!targetAcquired)
            {
                targetAcquired = true;
                targetPosition = target.position;
            }
            chargeTimer += Time.deltaTime / chargeTime;
            transform.position = Vector2.Lerp(startPosition, targetPosition, chargeTimer);
            if (chargeTimer >= 1)
            {
                charge = false;
                //toggleColliders();                
            }
        } else if (!charge)
        {
            chargeTimer = 0;
            main.m_canMove = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            targetAcquired = false;
        }
    }

    void toggleColliders()
    { 
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = !collider.enabled;
        }
    }
}
