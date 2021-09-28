using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
    [SerializeField] private float m_FlyForce = 40f;
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(1, 5)] [SerializeField] private float m_SprintSpeed = 2f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	public bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

    public float maxHealth = 3f;
    public float health = 3f;
    public float invulnerabilityTime = 3f;
    private float invulnerabilityTimer = 0f;
    public float freezeTime = 0.4f;
    private float freezeTimer = 0f;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;
    private bool m_canMove = true;
    public Animator animator;
    public bool m_canFly = false;

    public bool m_canGrapple = false;
    public LineRenderer line;
    private bool drawingRope = false;
    private bool grappling = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;

    public bool m_ignoreMoveDirection = false;

    public ContactFilter2D pickupFilter;
    public Collider2D[] playerColliders;
    public bool topDown = false;

    public int currentLevel = -1;
    public List<int> finishedLevels = new List<int>();

    public float elapsedTime = 0f;
    public bool enableTime = false;

    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
                if (!wasGrounded && m_Rigidbody2D.velocity.y <= 0)
                {
                    m_canMove = true;
                    OnLandEvent.Invoke();
                }
			}
		}

        if (invulnerabilityTimer > Time.time && ((invulnerabilityTimer - Time.time) / 0.2) % 2 < 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f, 1f);
        } else
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
        if (freezeTimer < Time.time)
        {
            m_canMove = true;
        }

        if (!grappling)
        {
            endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPosition = transform.position;
            t = 0;
        }
        else
        {
            GrappleMove(endPosition);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DamageSource") || collision.gameObject.CompareTag("EnemyProjectile"))
        {
            Debug.Log("damage taken");
            Damage(0.5f);
        }
    }

    public void Move(float move, bool crouch, bool jump, bool sprint, bool fly, float moveY)
	{
        if (m_Rigidbody2D.bodyType != RigidbodyType2D.Static)
        {
            if (m_canMove && !topDown)
            {
                m_Rigidbody2D.gravityScale = 3;
                if (sprint)
                {
                    move *= m_SprintSpeed;
                }
                if (fly && m_canFly)
                {
                    animator.SetBool("Jump", true);
                    float xForce = m_FlyForce;
                    if (move < 0)
                    {
                        xForce = -xForce;
                    }
                    else if (move == 0)
                    {
                        xForce = 0f;
                    }
                    if (m_Rigidbody2D.velocity.y < 8)
                    {
                        m_Rigidbody2D.AddForce(new Vector2(0f, m_FlyForce));
                    }
                    if (Mathf.Abs(m_Rigidbody2D.velocity.x) < 12)
                    {
                        m_Rigidbody2D.AddForce(new Vector2(xForce, 0f));
                    }

                }
                // If crouching, check to see if the character can stand up
                if (!crouch)
                {
                    // If the character has a ceiling preventing them from standing up, keep them crouching
                    if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                    {
                        crouch = true;
                    }
                }

                //only control the player if grounded or airControl is turned on
                if (m_Grounded || m_AirControl)
                {

                    // If crouching
                    if (crouch)
                    {
                        if (!m_wasCrouching)
                        {
                            m_wasCrouching = true;
                            OnCrouchEvent.Invoke(true);
                        }

                        // Reduce the speed by the crouchSpeed multiplier
                        move *= m_CrouchSpeed;

                        // Disable one of the colliders when crouching
                        if (m_CrouchDisableCollider != null)
                            m_CrouchDisableCollider.enabled = false;
                    }
                    else
                    {
                        // Enable the collider when not crouching
                        if (m_CrouchDisableCollider != null)
                            m_CrouchDisableCollider.enabled = true;

                        if (m_wasCrouching)
                        {
                            m_wasCrouching = false;
                            OnCrouchEvent.Invoke(false);
                        }
                    }

                    if (!fly)
                    {
                        // Move the character by finding the target velocity
                        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                        // And then smoothing it out and applying it to the character
                        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                    }

                    // If the input is moving the player right and the player is facing left...
                    if (move > 0 && !m_FacingRight && !m_ignoreMoveDirection)
                    {
                        // ... flip the player.
                        Flip();
                    }
                    // Otherwise if the input is moving the player left and the player is facing right...
                    else if (move < 0 && m_FacingRight && !m_ignoreMoveDirection)
                    {
                        // ... flip the player.
                        Flip();
                    }
                }
                // If the player should jump...
                if (m_Grounded && jump)
                {
                    // Add a vertical force to the player.
                    m_Grounded = false;
                    m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                }
            }
            else if (topDown)
            {
                m_Rigidbody2D.gravityScale = 0;
                if (sprint)
                {
                    move *= m_SprintSpeed;
                    moveY *= m_SprintSpeed;
                }
                if (crouch && !sprint)
                {
                    move *= m_CrouchSpeed;
                    moveY *= m_CrouchSpeed;
                }
                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight && !m_ignoreMoveDirection)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight && !m_ignoreMoveDirection)
                {
                    // ... flip the player.
                    Flip();
                }

                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 5f, moveY * 5f);
                // And then smoothing it out and applying it to the character
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            }
        }
	}

	public void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        transform.Rotate(0f, 180f, 0f);
	}

    public void Damage(float damage)
    {
        if (invulnerabilityTimer < Time.time)
        {
            health -= damage;
            invulnerabilityTimer = Time.time + invulnerabilityTime;
            freezeTimer = Time.time + freezeTime;
            m_canMove = false;
            //m_Grounded = false;
            float knockback = 400f;
            if (m_FacingRight)
            {
                knockback = -knockback;
            }
            m_Rigidbody2D.AddForce(new Vector2(knockback, 400f));
            GetComponent<AudioSource>().Play();
        }
        if (health == 0f)
        {
            health--;
            GameObject.FindGameObjectWithTag("Level").GetComponent<StartLevel>().ExitLevel();
        }
    }


    public void Grapple()
    {
        if (Vector2.Distance(endPosition, startPosition) < 50 && m_canGrapple)
        {
            line.SetPosition(0, startPosition + new Vector3 (0f, 1.22f, 0f));
            line.SetPosition(1, startPosition);

            Collider2D[] colliders = Physics2D.OverlapPointAll(endPosition);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.CompareTag("Grapple"))
                {
                    drawingRope = true;
                    grappling = true;
                    line.enabled = true;
                    endPosition = colliders[i].transform.position;
                }
            }
            Debug.Log(Camera.main.ScreenToWorldPoint(endPosition));
        }
    }

    private void GrappleMove(Vector3 destination)
    {
        Debug.Log(destination);
        t += Time.deltaTime / 0.3f;
        if (drawingRope)
        {
            line.SetPosition(0, transform.position + new Vector3(0f, 1.22f, 0f));
            line.SetPosition(1, startPosition + Mathf.Min(t, 1) * (destination - startPosition));
            if (t >= 1)
            {
                foreach (Collider2D collider in playerColliders)
                {
                    collider.enabled = false;
                }
                drawingRope = false;
                startPosition = transform.position;
                t = 0;
            }
        }
        else
        {
            transform.position = Vector2.Lerp(startPosition, destination, Mathf.Min(t, 1));
            line.SetPosition(0, transform.position + new Vector3(0f, 1.22f, 0f));
            if (t >= 1)
            {
                grappling = false;
                m_Rigidbody2D.velocity = new Vector2(0f, 0f);
                line.enabled = false;
                foreach (Collider2D collider in playerColliders)
                {
                    collider.enabled = true;
                }
            }
        }

        // lerp stretch rope from origin to destination 0.3s => lerp player
    }


    public void Pickup()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach(Collider2D collider in colliders)
        {
            Collider2D[] overlaps = new Collider2D[1];
            collider.OverlapCollider(pickupFilter, overlaps);
            if (overlaps[0] != null)
            {
                Debug.Log("item found");
                Debug.Log(overlaps[0].gameObject.name);
                try
                {
                    Unlock(overlaps[0].gameObject.GetComponent<Upgrade>().upgradeType);
                }
                catch
                {
                    overlaps[0].gameObject.GetComponent<Use>().run();
                }
                
                Destroy(overlaps[0].gameObject);
            }
        }
    }

    public void Unlock(string upgrade)
    {
        Debug.Log(upgrade);
        if (upgrade == "health")
        {
            maxHealth += 1;
            health = maxHealth;
            // Do health upgrades
        } else if (upgrade == "fly")
        {
            m_canFly = true;
        } else if (upgrade == "grapple")
        {
            m_canGrapple = true;
        } else if (upgrade == "stick")
        {
            GetComponent<Weapon>().AddWeapon("Stick");
        } else if (upgrade == "hammer")
        {
            GetComponent<Weapon>().AddWeapon("Hammer");
        } else if (upgrade == "railgun")
        {
            GetComponent<Weapon>().AddWeapon("Railgun");
        } else if (upgrade == "book")
        {
            GetComponent<Weapon>().AddWeapon("Book");
        } else if (upgrade == "firewall")
        {
            // Add reset switch functionality
            GetComponent<Weapon>().AddWeapon("Firewall");
        }
    }

    public void Lock(string upgrade)
    {
        if (upgrade == "health")
        {
            maxHealth -= 1;
            health = maxHealth;
            // Do health upgrades
        }
        else if (upgrade == "fly")
        {
            m_canFly = false;
        }
        else if (upgrade == "grapple")
        {
            m_canGrapple = false;
        }
        else if (upgrade == "stick")
        {
            GetComponent<Weapon>().RemoveWeapon("Stick");
        }
        else if (upgrade == "hammer")
        {
            GetComponent<Weapon>().RemoveWeapon("Hammer");
        }
        else if (upgrade == "railgun")
        {
            GetComponent<Weapon>().RemoveWeapon("Railgun");
        }
        else if (upgrade == "book")
        {
            GetComponent<Weapon>().RemoveWeapon("Book");
        }
        else if (upgrade == "firewall")
        {
            // Add reset switch functionality
            GetComponent<Weapon>().RemoveWeapon("Firewall");
        }
    }

    public void Heal()
    {
        health = maxHealth;
    }


    private void Update()
    {
        if (enableTime)
        {
            elapsedTime += Time.deltaTime;
        }
    }
}
