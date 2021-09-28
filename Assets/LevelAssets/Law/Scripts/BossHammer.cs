using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHammer : MonoBehaviour
{
    private float timer;
    private float hammerTime;
    private bool start = false;
    // Start is called before the first frame update

    public void HammerDown(float time)
    {
        Debug.Log("working");
        hammerTime = time;
        timer = 0;
        start = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (start && timer < hammerTime)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                Quaternion.Euler(0, 0, -135),
                timer / hammerTime
                );
        }
    }
}
