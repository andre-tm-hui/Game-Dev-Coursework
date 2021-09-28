using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoom : MonoBehaviour
{
    public int maxEnemies = 5;
    private int enemyCount;
    public GameObject enemyPrefab;
    private List<GameObject> enemies = new List<GameObject>();
    private bool isBossRoom = false;
    private RoomTemplates RoomTemplates;

    public GameObject Blockade;
    private List<GameObject> Blockades = new List<GameObject>();
    private bool blockadesSet = false;
    public float closeTime = 0.1f;
    public float closeDelay = 1f;
    private bool close = false;
    private float Timer;
    // Start is called before the first frame update\

    public bool isEntry = false;
    private GameObject player;

    void Start()
    {
        RoomTemplates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        if (Random.Range(0f, 1f) < 0.4f && !isEntry)
        {
            enemyCount = Random.Range(1, maxEnemies);
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 position = new Vector3(Random.Range(-32.5f, 32.5f), Random.Range(-13.5f, 19f), 0f);
                GameObject enemy = Instantiate(
                    enemyPrefab,
                    transform.position + position,
                    Quaternion.identity
                    );
                enemies.Add(enemy);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RoomTemplates.bossRoom == gameObject && !isBossRoom)
        {
            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }
            isBossRoom = true;
        }
        if (blockadesSet && Timer < closeTime)
        {
            if (!close)
            {
                Timer += Time.deltaTime;
                Blockades[0].transform.position = Vector2.Lerp(
                    Blockades[0].transform.position,
                    transform.position + new Vector3(16, 22.1f, 0),
                    Timer / closeTime
                    );

                Blockades[1].transform.position = Vector2.Lerp(
                    Blockades[1].transform.position,
                    transform.position + new Vector3(-16, -16.6f, 0),
                    Timer / closeTime
                    );

                Blockades[2].transform.position = Vector2.Lerp(
                    Blockades[2].transform.position,
                    transform.position + new Vector3(-35.5f, 5.1f, 0),
                    Timer / closeTime
                    );

                Blockades[3].transform.position = Vector2.Lerp(
                    Blockades[3].transform.position,
                    transform.position + new Vector3(35.5f, 5.1f, 0),
                    Timer / closeTime
                    );
                // remove barricades
            }
            else if (close)
            {
                Timer += Time.deltaTime;
                Blockades[0].transform.position = Vector2.Lerp(
                    Blockades[0].transform.position,
                    transform.position + new Vector3(0, 22.1f, 0),
                    Timer / closeTime
                    );

                Blockades[1].transform.position = Vector2.Lerp(
                    Blockades[1].transform.position,
                    transform.position + new Vector3(0, -16.6f, 0),
                    Timer / closeTime
                    );

                Blockades[2].transform.position = Vector2.Lerp(
                    Blockades[2].transform.position,
                    transform.position + new Vector3(-35.5f, -9f, 0),
                    Timer / closeTime
                    );

                Blockades[3].transform.position = Vector2.Lerp(
                    Blockades[3].transform.position,
                    transform.position + new Vector3(35.5f, -9f, 0),
                    Timer / closeTime
                    );
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            GameObject Camera = GameObject.FindGameObjectWithTag("MainCamera");
            Camera.GetComponent<FollowPlayer>().position = transform.position + new Vector3(0, 2.75f, Camera.transform.position.z);
            Camera.GetComponent<FollowPlayer>().lerpMove = true;
            if ((enemies.Count > 0 || isBossRoom) && !blockadesSet && !isEntry)
            {
                Invoke("GenerateBlockades", closeDelay);
            }
            if (isBossRoom)
            {
                Debug.Log("initBoss");
                Invoke("InitBossBattle", 2f);
            }
        }
    }
    private void FixedUpdate()
    {
        enemyCount = 0;
        foreach (GameObject enemy in enemies)
        {
            if (enemy)
            {
                enemyCount++;
            }
        }
        if (enemyCount == 0 && !isBossRoom)
        {
            Timer = 0;
            close = false;
        }
    }

    void GenerateBlockades()
    {
        Debug.Log("addbaricades");
        // add barricades
        GameObject up = Instantiate(
            Blockade,
            transform.position + new Vector3(16, 22.1f, 0),
            Quaternion.identity
            );
        Blockades.Add(up);

        GameObject down = Instantiate(
            Blockade,
            transform.position + new Vector3(-16, -16.6f, 0),
            Quaternion.identity
            );
        Blockades.Add(down);

        GameObject left = Instantiate(
            Blockade,
            transform.position + new Vector3(-35.5f, 5.1f, 0),
            Quaternion.identity
            );
        left.transform.rotation = Quaternion.Euler(0, 0, 90);
        Blockades.Add(left);

        GameObject right = Instantiate(
            Blockade,
            transform.position + new Vector3(35.5f, 5.1f, 0),
            Quaternion.identity
            );
        right.transform.rotation = Quaternion.Euler(0, 0, 90);
        Blockades.Add(right);

        Timer = 0;

        blockadesSet = true;
        close = true;

        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<PhysicsBossAI>().enabled = true;
        }

        // for left and right, bring down to transform.position + new Vector3(35.5f, -9, 0)
    }

    void InitBossBattle()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        boss.GetComponent<EnemyPathfinding>().enabled = true;
        boss.GetComponent<PhysicsBossAI>().enabled = true;
        boss.GetComponent<EnemyPathfinding>().target = player.transform;
        boss.GetComponent<HUDData>().enabled = true;
    }
}
