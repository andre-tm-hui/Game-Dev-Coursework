using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public float worldWidth = 100f;
    public List<GameObject> sides = new List<GameObject>();
    // each side has boundaries at x = +- 72
    private List<Vector3> sidesMoveTo = new List<Vector3>();
    // to be populated with 4
    // one side is facing camera and visible, one side is facing but invisible, other sides should be facing perpendicular to cameras, hence not visible
    // 0 - 1
    // |   |
    // 3 - 2
    public Transform player;

    private int currentSide = 0;

    private float turnAngle = 0f;
    private float turnedAngle = 0f;
    private bool updated = true;

    private int left;
    private int right;

    public GameObject boss;
    private bool bossInit = false;
    private float rotateAngle;
    private GameObject AStar;

    // Start is called before the first frame update
    void Start()
    {
        AStar = GameObject.FindGameObjectWithTag("AStar");
        rotateAngle = 360 / sides.Count;
        for (int i = 1; i <= sides.Count; i++)
        {
            Vector3 position = Vector3.down * 1000 * i;
            sidesMoveTo.Add(position);
            sides[i - 1].transform.position = position;
            sides[i - 1].transform.Rotate(0, (i - 1) * rotateAngle, 0);
        }
        sides[currentSide].transform.position = Vector3.zero;
        sides[currentSide].GetComponent<Side>().shieldDrone.GetComponent<EnemyPathfinding>().enabled = true;
        sides[currentSide].GetComponent<Side>().shieldDrone.GetComponent<PhysicsBossAI>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (turnedAngle != turnAngle)
        {
            //Debug.Log("turning");
            float angle = turnAngle * Time.deltaTime;
            if (Mathf.Abs(turnedAngle + angle) > Mathf.Abs(turnAngle))
            {
                angle = turnAngle - turnedAngle;
            }
            turnedAngle += angle;

           // Debug.Log(turnedAngle);
            foreach (GameObject side in sides)
            {
                if (turnAngle < 0)
                {
                    side.transform.RotateAround(sides[currentSide].GetComponent<Side>().left.position, Vector3.up, angle);
                }
                else
                {
                    side.transform.RotateAround(sides[currentSide].GetComponent<Side>().right.position, Vector3.up, angle);
                }
            }
        }
        else if (!updated)
        {
            Debug.Log(currentSide);
            try
            {
                sides[currentSide].GetComponent<Side>().shieldDrone.GetComponent<EnemyPathfinding>().enabled = false;
                sides[currentSide].GetComponent<Side>().shieldDrone.GetComponent<PhysicsBossAI>().enabled = false;
            }
            catch { }
            sides[currentSide].transform.position = sidesMoveTo[currentSide];
            Debug.Log("moved");

            if (turnAngle < 0)
            {
                if (currentSide == sides.Count - 1)
                {
                    currentSide = 0;
                }
                else
                {
                    currentSide++;
                }
                sides[currentSide].transform.position = Vector3.zero;
                player.position += 144 * Vector3.right;
            }
            else
            {
                if (currentSide == 0)
                {
                    currentSide = sides.Count - 1;
                }
                else
                {
                    currentSide--;
                }
                Debug.Log("movedleft");
                sides[currentSide].transform.position = Vector3.zero;
                player.position += 144 * Vector3.left;
            }
            player.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            try
            {
                sides[currentSide].GetComponent<Side>().shieldDrone.GetComponent<EnemyPathfinding>().enabled = true;
                sides[currentSide].GetComponent<Side>().shieldDrone.GetComponent<PhysicsBossAI>().enabled = true;
            }
            catch { }
            AStar.GetComponent<AstarPath>().Scan();
            updated = true;
        }
    }

    void Updated()
    {
        
    }

    private void FixedUpdate()
    {
        left = currentSide + 1;
        if (left == sides.Count)
        {
            left = 0;
        }
        right = currentSide - 1;
        if (right == -1)
        {
            right = sides.Count - 1;
        }
        if (Mathf.Abs(player.transform.position.x - sides[currentSide].GetComponent<Side>().left.position.x) < 1 && !player.gameObject.GetComponent<CharacterController2D>().m_FacingRight && updated)
        {
            sides[left].transform.position = sides[currentSide].transform.position;//- 72 * Vector3.left + 72 * Vector3.forward;
            Vector3 move = sides[currentSide].GetComponent<Side>().left.position - sides[left].GetComponent<Side>().right.position;
            sides[left].transform.position += move;
            turnAngle = -rotateAngle;
            //Debug.Log(turnAngle);
            turnedAngle = 0;
            player.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            updated = false;

        } else if (Mathf.Abs(player.transform.position.x - sides[currentSide].GetComponent<Side>().right.position.x) < 1 && player.gameObject.GetComponent<CharacterController2D>().m_FacingRight && updated)
        {
            sides[right].transform.position = sides[currentSide].transform.position;//- 72 * Vector3.left + 72 * Vector3.forward;
            Vector3 move = sides[currentSide].GetComponent<Side>().right.position - sides[right].GetComponent<Side>().left.position;
            sides[right].transform.position += move;
            //sides[right].transform.position = sides[currentSide].transform.position + 72 * Vector3.left + 72 * Vector3.forward;
            turnAngle = rotateAngle;
            turnedAngle = 0;
            player.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            updated = false;
        }

        bool cleared = true;
        foreach (GameObject side in sides)
        {
            if (!side.GetComponent<Side>().cleared)
            {
                cleared = false;
            }
        }

        if (cleared && !bossInit)
        {
            boss.GetComponentInChildren<Shatterable>().Die(0);
            boss.GetComponentInChildren<FinalBossAI>().enabled = true;
            //boss.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            boss.GetComponentInChildren<Collider2D>().enabled = true;
            boss.GetComponentInChildren<HUDData>().enabled = true;
            
            bossInit = true;
        }
    }
}
