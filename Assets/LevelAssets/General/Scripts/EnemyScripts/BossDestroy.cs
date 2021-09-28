using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        if (GetComponent<EnemyBasicAI>().health <= 0)
        {
            GameObject.FindGameObjectWithTag("Level").GetComponent<StartLevel>().BossBeaten();
        }
    }
}
