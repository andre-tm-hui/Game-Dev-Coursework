using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public ContactFilter2D contactFilter;

    public GameObject projectilePrefab;
    public GameObject firewallPrefab;
    private GameObject firewall;
    public float firewallTime = 2f;
    private float timer = 0;
    private bool broken = false;
    public float firewallBrokenTime = 2f;
    private float brokenTimer;
    public List<string> weaponNames = new List<string>();

    public CharacterController2D controller;

    public int weaponSelect = 0;
    
    private float nextFire = 0f;
    private float fireRate = 0.3f;
    private bool firstMelee = true;

    private Vector2 HammerPoint = new Vector2(3.5f, 2.5f);
    private Vector2 StickPoint = new Vector2(3.5f, 2f);

    private int barrelsBroken = 0;

    private void Start()
    {
        //AddWeapon("Railgun");
        //AddWeapon("Hammer");
        //AddWeapon("Stick");
        //AddWeapon("Book");
        //AddWeapon("Firewall");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire && weaponNames.Count > 0)
        {
            controller.m_ignoreMoveDirection = true;
            Attack();
            nextFire = Time.time + fireRate;
        } else if (Input.GetButtonUp("Fire1"))
        {
            transform.Find("AttackPoint").GetComponent<Collider2D>().enabled = false;
            controller.m_ignoreMoveDirection = false;
            animator.SetBool("Hammer", false);
            animator.SetBool("Stick", false);
            firstMelee = true;
        }

        if (firewall && Time.time > timer)
        {
            broken = true;
            Destroy(firewall);
            brokenTimer = Time.time + firewallBrokenTime;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            weaponSelect = (weaponSelect + 1) % 5;
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            weaponSelect--;
            if (weaponSelect < 0)
            {
                weaponSelect += 5;
            }
        }

        if (Input.GetKeyDown("1") && weaponNames.Contains("Railgun"))
        {
            weaponSelect = 0;
        } else if (Input.GetKeyDown("2") && weaponNames.Contains("Book"))
        {
            weaponSelect = 1;
        }
        else if (Input.GetKeyDown("3") && weaponNames.Contains("Stick"))
        {
            weaponSelect = 2;
        }
        else if (Input.GetKeyDown("4") && weaponNames.Contains("Hammer"))
        {
            weaponSelect = 3;
        }
        else if (Input.GetKeyDown("5") && weaponNames.Contains("Firewall"))
        {
            weaponSelect = 4;
        }

        if (Time.time > brokenTimer)
        {
            broken = false;
        }
    }

    void Attack()
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (Vector2)((worldMousePos - firePoint.position));
        direction.Normalize();

        if (direction.x < 0 && controller.m_FacingRight)
        {
            controller.Flip();
        } else if (direction.x > 0 && !controller.m_FacingRight)
        {
            controller.Flip();
        }

        if (weaponSelect == 0 && weaponNames.Contains("Railgun")) // railgun
        {
            GameObject projectile = Instantiate(
                projectilePrefab,
                firePoint.position + (Vector3)(direction * 0.1f),
                Quaternion.identity
                );

            projectile.GetComponent<Animator>().SetBool("Railgun", true);
            projectile.GetComponent<Bullet>().speed = 30f;
            projectile.GetComponent<Bullet>().lifetime = 3f;
            projectile.GetComponent<Bullet>().damage = 1;
            projectile.GetComponent<Bullet>().gravity = 0f;
            projectile.GetComponent<Bullet>().force = 100f;

        } else if (weaponSelect == 3 && weaponNames.Contains("Hammer")) // hammer
        {
            transform.Find("AttackPoint").GetComponent<Collider2D>().enabled = true;
            animator.SetBool("Hammer", true);
            if (controller.m_FacingRight)
            {
                attackPoint.position = transform.position + (new Vector3(HammerPoint.x, HammerPoint.y, 0));
            }
            else
            {
                attackPoint.position = transform.position + (new Vector3(-HammerPoint.x, HammerPoint.y, 0));
            }

            attackPoint.GetComponent<CapsuleCollider2D>().size = new Vector2 (8, 10);
            attackPoint.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Vertical;

            Collider2D[] hitEnemies = new Collider2D[10];
            Physics2D.OverlapCollider(attackPoint.GetComponent<Collider2D>(), contactFilter, hitEnemies);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy)
                {
                    EnemyBasicAI enemyAI = enemy.gameObject.GetComponent<EnemyBasicAI>();
                    if (enemyAI != null && !firstMelee)
                    {
                        enemyAI.TakeDamage(5, direction, 2000);
                    }
                    Ball ball = enemy.gameObject.GetComponent<Ball>();
                    if (ball != null && !firstMelee)
                    {
                        ball.Die();
                    }
                    Shatterable obj = enemy.gameObject.GetComponent<Shatterable>();
                    if (obj != null && !firstMelee)
                    {
                        barrelsBroken++;
                        obj.Die(barrelsBroken);
                    }
                }
            }
            firstMelee = false;

        } else if (weaponSelect == 1 && weaponNames.Contains("Book")) // book
        {
            GameObject projectile = Instantiate(
                projectilePrefab,
                firePoint.position + (Vector3)(direction * 0.1f),
                Quaternion.identity
                );

            projectile.GetComponent<Animator>().SetBool("Railgun", false);
            projectile.transform.localScale += new Vector3(2f, 2f, 2f);
            projectile.GetComponent<Bullet>().speed = 5f;
            projectile.GetComponent<Bullet>().lifetime = 20f;
            projectile.GetComponent<Bullet>().damage = 3;
            projectile.GetComponent<Bullet>().gravity = 0.2f;
            projectile.GetComponent<Bullet>().force = 400f;

        } else if (weaponSelect == 2 && weaponNames.Contains("Stick")) // hockeystick
        {
            transform.Find("AttackPoint").GetComponent<Collider2D>().enabled = true;
            animator.SetBool("Stick", true);
            if (controller.m_FacingRight)
            {
                attackPoint.position = transform.position + (new Vector3(StickPoint.x, StickPoint.y, 0));
            }
            else
            {
                attackPoint.position = transform.position + (new Vector3(-StickPoint.x, StickPoint.y, 0));
            }

            attackPoint.GetComponent<CapsuleCollider2D>().size = new Vector2(10, 7);
            attackPoint.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;

            Collider2D[] hitEnemies = new Collider2D[10];
            Physics2D.OverlapCollider(attackPoint.GetComponent<Collider2D>(), contactFilter, hitEnemies);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy)
                {
                    EnemyBasicAI enemyAI = enemy.gameObject.GetComponent<EnemyBasicAI>();
                    if (enemyAI != null && !firstMelee)
                    {
                        enemyAI.TakeDamage(1, direction, 400);
                    }

                    Ball ball = enemy.gameObject.GetComponent<Ball>();
                    if (ball != null && !firstMelee)
                    {
                        ball.Die();
                    }
                    Shatterable obj = enemy.gameObject.GetComponent<Shatterable>();
                    if (obj != null && !firstMelee)
                    {
                        barrelsBroken++;
                        obj.Die(barrelsBroken);
                    }
                }
            }
            firstMelee = false;
        } else if (weaponSelect == 4 && weaponNames.Contains("Firewall"))
        {
            if (!firewall && !broken)
            {
                timer = Time.time + firewallTime;
                firewall = Instantiate(
                    firewallPrefab,
                    transform.position + new Vector3(0, 2, 0),
                    Quaternion.identity
                    );
                firewall.transform.parent = transform;
            } else if (Time.time > timer & !broken)
            {
                broken = true;
                Destroy(firewall);
                brokenTimer = Time.time + firewallBrokenTime;
            }
        }
    }

    public void AddWeapon(string weaponName)
    {
        weaponNames.Add(weaponName);
    }

    public void RemoveWeapon(string weaponName)
    {
        weaponNames.Remove(weaponName);
    }

    private void FixedUpdate()
    {
        if (firewall)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)(transform.position + new Vector3(0, 3, 0))).normalized;
            firewall.transform.position = transform.position + new Vector3(0, 3, 0) + ((Vector3)direction * 3.5f);
            firewall.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        }
    }
}
