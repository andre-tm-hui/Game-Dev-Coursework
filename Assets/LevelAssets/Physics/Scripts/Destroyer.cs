using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private int destroyed = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        destroyed++;
        if (destroyed == 5)
        {
            Destroy(gameObject);
        }
    }
}
