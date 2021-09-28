using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float rateOfFire = 0.5f;
    public float delay = 2f;
    public List<GameObject> prefabs = new List<GameObject>();
    public float speed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Fire", delay, rateOfFire);
    }

    void Fire()
    {
        GameObject num = Instantiate(
            prefabs[Random.Range(0, prefabs.Count)],
            transform.position,
            transform.rotation
            );
        num.transform.parent = transform;
        num.GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
