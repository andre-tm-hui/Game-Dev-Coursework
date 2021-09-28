using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class EnemyPathfinding : MonoBehaviour
{
    public Rigidbody2D rb;

    // Pathfinding components
    public Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    public Collider2D collider;
    public Transform upperLeft;
    public Transform lowerRight;

    public Transform target;

    // Toggle if you want to follow the target
    public bool m_useTarget = true;

    // Toggle if you want to move the pathfinding grid-graph with the object
    public bool m_moveGrid = false;
    public float m_moveGridInterval = 3f;
    public GameObject AStar;

    // Toggle if you want to track enemies offscreen
    public GameObject Pointer;
    public bool m_trackOffScreen = false;
    public float m_edgeBuffer = 0.03f;

    // Movement variables
    public float speed = 200f;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;
    bool m_Grounded = false;
    public bool m_canMove = false;
    private bool canJump = true;
    float k_GroundedRadius = 0.2f;
    bool m_facingRight = true;
    float xDirection = 0;
    public int minJumpHeight = 10;
    public int minJumpLength = 20;
    public float jumpForce = 1500;
    public bool m_canFly = false;
    public Transform m_ceilingCheck;
    private float heightOffset = 3f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        collider = GetComponent<CapsuleCollider2D>();
        if (!collider)
        {
            collider = GetComponent<Collider2D>();
        }

        if (m_moveGrid && AStar != null)
        {
            InvokeRepeating("UpdateGridGraph", 0f, m_moveGridInterval);
        }

        if (m_useTarget)
        {
            InvokeRepeating("UpdateTargetPath", 0f, 1f);
        } else
        {
            Invoke("UpdatePath", 0f);
        }
    }

    void UpdateGridGraph()
    {
        AStar.GetComponent<AstarPath>().data.gridGraph.center = transform.position;
        AStar.GetComponent<AstarPath>().Scan();
    }

    void UpdateTargetPath()
    {
        if (seeker.IsDone() && target)
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void UpdatePath()
    {
        if (upperLeft && lowerRight)
        {
            Vector2 targetPos = new Vector2(Random.Range(upperLeft.position.x, lowerRight.position.x), Random.Range(lowerRight.position.y, upperLeft.position.y));
            if (seeker.IsDone())
            {
                seeker.StartPath(rb.position, targetPos, OnPathComplete);
            }
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 1;
            for (int i = 1; i < path.vectorPath.Count; i++)
            {
                if (collider.OverlapPoint((Vector2)path.vectorPath[i]) || path.vectorPath[i] - path.vectorPath[i - 1] == Vector3.up)
                {
                    currentWaypoint = i + 1;
                    //Debug.Log(path.vectorPath[i] - path.vectorPath[i - 1] == Vector3.up);
                }
                else
                {
                    break;
                }
            }
        }
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
        if (m_trackOffScreen)
        {
            Vector2 pos = transform.position;
            pos = Camera.main.WorldToViewportPoint(pos);

            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
            {
                Pointer.GetComponent<Image>().enabled = true;
                RectTransform CanvasRect = GameObject.FindGameObjectWithTag("HUD").GetComponent<RectTransform>();
                Vector2 direction = (rb.position - (Vector2)target.position).normalized;
                Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target.position + new Vector3 (0, heightOffset, 0));
                Vector2 PointerPosition = new Vector2(
                    (ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f),
                    (ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f));
                //Vector2 PointerPosition = new Vector2(0, m_PointerYOffset);
                while (Mathf.Abs(PointerPosition.x) < 890 && Mathf.Abs(PointerPosition.y) < 470)
                {
                    PointerPosition += direction;
                }
                Pointer.GetComponent<RectTransform>().anchoredPosition = PointerPosition;
                Pointer.GetComponent<RectTransform>().rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
            }
            else
            {
                Pointer.GetComponent<Image>().enabled = false;
            }
        }

        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        if (m_GroundCheck)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    m_Grounded = true;
                    if (!wasGrounded && rb.velocity.y <= 0)
                    {
                        m_canMove = true;
                    }
                }
            }
        }

        if (path != null && rb.bodyType != RigidbodyType2D.Static)
        {
            if ((m_canMove || m_canFly) && currentWaypoint < path.vectorPath.Count)
            {
                Move();
            }

            if (currentWaypoint >= path.vectorPath.Count && !m_useTarget)
            {
                Invoke("UpdatePath", 2f);
            }
        }
    }

    private void Move()
    {
        if (path == null)
        {
            return;
        }

        Vector2 direction = ((Vector2)(path.vectorPath[currentWaypoint] - transform.position)).normalized;

        if (rb.velocity.x > 0.1f && !m_facingRight)
        {
            Flip();
        }
        else if (rb.velocity.x < 0.1f && m_facingRight)
        {
            Flip();
        }

        if (!m_canFly && m_ceilingCheck)
        {
            RaycastHit2D ray = Physics2D.Raycast(m_ceilingCheck.position, transform.up, Mathf.Infinity, m_WhatIsGround);
            Vector2 jumpCheck = path.vectorPath[Mathf.Min(path.vectorPath.Count - 1, minJumpHeight)] - path.vectorPath[0];

            if (!float.IsNaN(direction.x / Mathf.Abs(direction.x)))
            {
                if (ray.distance > minJumpHeight && Mathf.Abs(jumpCheck.x) > minJumpLength)
                {
                    xDirection = direction.x / Mathf.Abs(direction.x);
                    canJump = true;
                }
                else if (ray.distance < minJumpHeight)
                {
                    canJump = false;
                }
                if (jumpCheck.y < minJumpHeight)
                {
                    xDirection = direction.x / Mathf.Abs(direction.x);
                }
            }

            if (xDirection > 0 && !m_facingRight)
            {
                Flip();
            }
            else if (xDirection < 0 && m_facingRight)
            {
                Flip();
            }

            if (m_Grounded && jumpCheck.y >= minJumpHeight && canJump)
            {
                rb.AddForce(new Vector2(0f, jumpForce));
            }
            else if (m_Grounded)
            {
                rb.velocity = new Vector2(xDirection * speed, 0);
            }
            else
            {
                if (transform.position.x >= lowerRight.position.x || transform.position.x <= upperLeft.position.x || transform.position.y >= upperLeft.position.y || transform.position.y <= lowerRight.position.y)
                {
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    rb.velocity = new Vector2(xDirection * speed, rb.velocity.y);
                }
            }
        }
        else
        {
            rb.velocity = direction * speed;
        }

        for (int i = currentWaypoint; i < path.vectorPath.Count; i++)
        {
            if (collider.OverlapPoint(path.vectorPath[i]) || path.vectorPath[i] - path.vectorPath[i - 1] == Vector3.up)
            {
                currentWaypoint = i + 1;
            }
            else
            {
                break;
            }
        }
    }

    void Flip()
    {
        m_facingRight = !m_facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

}
