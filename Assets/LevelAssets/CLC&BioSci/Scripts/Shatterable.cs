using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatterable : MonoBehaviour
{
    public List<Spawner> spawnPoints;
    public GameObject HeartPrefab;

    private SpriteRenderer render;

    // Use this for initialization
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    public void Die(int BarrelsBroken)
    {
        render.enabled = false;

        foreach (Spawner spawn in spawnPoints)
        {
            spawn.Spawn();
        }

        if ((Random.Range(0, 50) <= 1 || BarrelsBroken == 80) && HeartPrefab)
        {
            GameObject heart = Instantiate(
                HeartPrefab,
                transform.position,
                Quaternion.identity
                );
            heart.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 200f));
        }

        Destroy(gameObject);
    }
}
