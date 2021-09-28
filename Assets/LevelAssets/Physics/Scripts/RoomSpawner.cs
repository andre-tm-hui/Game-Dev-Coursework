using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int OpeningDirection;
    // 1 --> need bottom door
    // 2 --> need top door
    // 3 --> need left door
    // 4 --> need right door

    private RoomTemplates templates;
    private int rand;
    private bool spawned = false;

    private void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.1f);
    }

    private void Spawn()
    {
        if (!spawned)
        {
            if (templates.rooms.Count < templates.maxRooms)
            {
                if (OpeningDirection == 1)
                {
                    rand = Random.Range(0, templates.bottomRooms.Length);
                    GameObject room = Instantiate(
                        templates.bottomRooms[rand],
                        transform.position,
                        Quaternion.identity);
                    room.transform.parent = GameObject.FindGameObjectWithTag("Grid").transform;
                    templates.rooms.Add(room);
                }
                else if (OpeningDirection == 2)
                {
                    rand = Random.Range(0, templates.topRooms.Length);
                    GameObject room = Instantiate(
                        templates.topRooms[rand],
                        transform.position,
                        Quaternion.identity);
                    room.transform.parent = GameObject.FindGameObjectWithTag("Grid").transform;
                    templates.rooms.Add(room);
                }
                else if (OpeningDirection == 4)
                {
                    rand = Random.Range(0, templates.leftRooms.Length);
                    GameObject room = Instantiate(
                        templates.leftRooms[rand],
                        transform.position,
                        Quaternion.identity);
                    room.transform.parent = GameObject.FindGameObjectWithTag("Grid").transform;
                    templates.rooms.Add(room);
                }
                else if (OpeningDirection == 3)
                {
                    rand = Random.Range(0, templates.rightRooms.Length);
                    GameObject room = Instantiate(
                        templates.rightRooms[rand],
                        transform.position,
                        Quaternion.identity);
                    room.transform.parent = GameObject.FindGameObjectWithTag("Grid").transform;
                    templates.rooms.Add(room);
                }
            } else
            {
                GameObject room = Instantiate(
                    templates.closedRoom,
                    transform.position,
                    Quaternion.identity);
                room.transform.parent = GameObject.FindGameObjectWithTag("Grid").transform;
                Destroy(gameObject);
            }
            spawned = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SpawnPoint"))
        {
            if (collision.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
                GameObject room = Instantiate(
                    templates.closedRoom,
                    transform.position,
                    Quaternion.identity);
                room.transform.parent = GameObject.FindGameObjectWithTag("Grid").transform;
                Destroy(gameObject);
            }
            spawned = true;
        }
    }

    public bool isSpawned()
    {
        return spawned;
    }
}
