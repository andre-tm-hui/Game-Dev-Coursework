using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGenerator : MonoBehaviour
{
    public GameObject rock;
    public float spawnInterval;
    public Transform leftBound;
    public Transform rightBound;
    public float duration;
    public int spawnVolume;
    public int geoNodes = 7;
    private bool finished = false;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("RockSpawner", 0f, spawnInterval);
    }

    void RockSpawner()
    {
        for (int i = 0; i < spawnVolume; i++)
        {
            Vector2 spawnPoint = new Vector2(Random.Range(leftBound.position.x, rightBound.position.x), transform.position.y);
            GameObject spawn = Instantiate(
                rock,
                spawnPoint,
                Quaternion.identity
                );
            Destroy(spawn, duration);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (geoNodes <= 0 && !finished)
        {
            CancelInvoke();
            GameObject.FindGameObjectWithTag("Level").GetComponent<StartLevel>().BossBeaten();
            finished = true;
        }
    }
}
