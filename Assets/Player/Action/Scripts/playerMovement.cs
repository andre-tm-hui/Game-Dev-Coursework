using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public Rigidbody2D rb;
    public LayerMask layerMask;
    public Collider2D[] playerColliders;
    public LineRenderer line;

    public float runSpeed = 40f;
    public float flySpeed = 40f;

    float horizontalMove = 0f;
    float verticalMove = 0f;

    bool jump = false;
    bool crouch = false;
    bool sprint = false;
    bool fly = false;
    bool grapple = false;
    bool rope = true;

    Vector3 heightOffset = new Vector3(0f, 1.22f, 0f);

    Vector3 startPosition;
    Vector3 endPosition;
    float t;

    private void Start()
    {
        line.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        verticalMove = Input.GetAxisRaw("Vertical") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            fly = true;
        } else if (Input.GetButtonUp("Jump"))
        {
            animator.SetBool("Jump", false);
            fly = false;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        } else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        if (Input.GetButtonDown("Sprint"))
        {
            sprint = true;
        } else if (Input.GetButtonUp("Sprint"))
        {
            sprint = false;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log(endPosition);
            controller.Grapple();
        }

        if (Input.GetButtonDown("Use"))
        {
            Debug.Log("use");
            controller.Pickup();
        }

    }

    public void OnLanding()
    {
        fly = false;
        animator.SetBool("Jump", false);
    }

    // Move our character
    private void FixedUpdate()
    {
        if (!grapple)
        {
            endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPosition = transform.position;
            t = 0;
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, sprint, fly, verticalMove * Time.fixedDeltaTime);
            jump = false;
        }
        else
        {
            GrappleMove(endPosition);
        }

    }


    private void GrappleMove(Vector3 destination)
    {
        Debug.Log(destination);
        t += Time.deltaTime / 0.3f;
        if (rope)
        {
            line.SetPosition(0, transform.position + heightOffset);
            line.SetPosition(1, startPosition + Mathf.Min(t, 1) * (destination - startPosition));
            if (t >= 1)
            {
                foreach (Collider2D collider in playerColliders)
                {
                    collider.enabled = false;
                }
                rope = false;
                startPosition = transform.position;
                t = 0;
            }
        }
        else
        {
            transform.position = Vector2.Lerp(startPosition, destination, Mathf.Min(t, 1));
            line.SetPosition(0, transform.position + heightOffset);
            if (t >= 1)
            {
                grapple = false;
                rb.velocity = new Vector2 (0f, 0f);
                line.enabled = false;
                foreach (Collider2D collider in playerColliders)
                {
                    collider.enabled = true;
                }
            }
        }
        
        // lerp stretch rope from origin to destination 0.3s => lerp player
    }
}
