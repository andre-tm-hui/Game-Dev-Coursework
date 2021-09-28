using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Side : MonoBehaviour
{
    public Transform left;
    public Transform right;
    public GameObject shieldDrone;
    public GameObject shield;
    public bool cleared = false;

    // Update is called once per frame
    void Update()
    {
        if (!shieldDrone && !cleared)
        {
            Invoke("BreakShield", 1f);
        }
    }

    void BreakShield()
    {
        try
        {
            shield.GetComponent<Shatterable>().Die(0);
            cleared = true;
        }
        catch { }
    }
}
