using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    public Vector3 offset;
    private Collider2D collider;
    public ContactFilter2D contactFilter;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = transform.parent.position + offset;
        Collider2D[] results = new Collider2D[1];
        Physics2D.OverlapCollider(collider, contactFilter, results);
        if (results[0] != null && results[0].gameObject.layer == 9)
        {
            results[0].gameObject.GetComponent<CharacterController2D>().Damage(0.5f);
        }
    }
}
