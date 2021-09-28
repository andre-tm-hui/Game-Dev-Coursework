using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossAI : MonoBehaviour
{
    public GameObject laserPrefab;
    public SpriteRenderer laserSource;
    public int numOfLasers = 15;
    public int chargeTime = 3;
    public float speed = 20;
    public GameObject reward;

    private float angle;
    // Start is called before the first frame update
    void Start()
    {
        angle = 360 / numOfLasers;

        InvokeRepeating("Attack", 3f, 1.5f * chargeTime);
    }

    void Attack()
    {
        if (Random.Range(0, 1) < 0.3)
        {
            laserSource.enabled = true;
            for (int i = 0; i < Random.Range(1, 4); i++)
            {
                Invoke("Fire", ((float)i / 2) + chargeTime);
            }
            
        }
    }

    void Fire()
    {
        float offsetAngle = Random.Range(0, 360);
        for (int i = 0; i < numOfLasers; i++)
        {
            GameObject laser = Instantiate(
                laserPrefab,
                transform.position,
                Quaternion.identity
                );
            laser.transform.Rotate(0, 0, offsetAngle + i * angle);
            laser.GetComponent<Laser>().speed = speed;
            laser.GetComponent<Laser>().Launch();
        }
        laserSource.enabled = false;
    }

    private void OnDestroy()
    {
        Instantiate(reward, transform.position, Quaternion.identity);
    }
}
