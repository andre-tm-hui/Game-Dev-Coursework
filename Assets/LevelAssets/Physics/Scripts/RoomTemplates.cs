using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;

    public GameObject closedRoom;
    public List<GameObject> rooms = new List<GameObject>();
    public int maxRooms = 20;

    private bool spawnedBoss = false;
    private bool allSpawned = false;
    private GameObject[] spawnPoints;
    public GameObject bossRoom;
    public GameObject bossPrefab;
    public GameObject AStar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (!allSpawned)
        {
            foreach (GameObject spawnPoint in spawnPoints)
            {
                allSpawned = true;
                if (!spawnPoint.GetComponent<RoomSpawner>().isSpawned())
                {
                    allSpawned = false;
                    break;
                }
            }
        }

        if (allSpawned && !spawnedBoss)
        {
            foreach (GameObject spawnPoint in spawnPoints)
            {
                Destroy(spawnPoint);
            }

            bossRoom = rooms[rooms.Count - 1];
            Tilemap tilemap = bossRoom.GetComponent<Tilemap>();
            for (int i = -38; i < 38; i++)
            {
                for (int j = -17; j < 23; j++)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), null);
                }
            }

            GameObject boss = Instantiate(
                bossPrefab,
                bossRoom.transform.position - new Vector3(20, 0, 0),
                Quaternion.identity
                );
            AStar.GetComponent<AstarPath>().data.gridGraph.center = bossRoom.transform.position + new Vector3 (0, 2.75f, 0);
            AStar.GetComponent<AstarPath>().data.gridGraph.SetDimensions(70, 40, 1);
            Invoke("Scan", 3f);
            spawnedBoss = true;
        }
    }

    void Scan()
    {
        AStar.GetComponent<AstarPath>().Scan();
    }
}
