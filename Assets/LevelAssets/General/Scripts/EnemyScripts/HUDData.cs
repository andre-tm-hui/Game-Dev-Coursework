using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDData : MonoBehaviour
{
    private GameObject healthbar;
    private GameObject bossname;
    private EnemyBasicAI main;
    private float maxHealth;
    private float maxWidth;

    public string bname;
    void Start()
    {
        healthbar = GameObject.FindGameObjectWithTag("BossHealth");
        bossname = GameObject.FindGameObjectWithTag("BossName");
        main = GetComponent<EnemyBasicAI>();

        Debug.Log(bossname.name);
        bossname.GetComponent<TMPro.TextMeshProUGUI>().text = bname;

        maxHealth = main.health;
        maxWidth = healthbar.GetComponent<RectTransform>().localScale.x;
        Debug.Log(maxWidth);

        healthbar.GetComponent<Image>().enabled = true;
        bossname.GetComponent<TMPro.TextMeshProUGUI>().enabled = true;
    }

    private void FixedUpdate()
    {
        if (maxHealth == 0)
        {
            maxHealth = main.health;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float xScale = maxWidth * main.health / maxHealth;
        if (xScale < 0)
        {
            xScale = 0;
        }
        healthbar.GetComponent<RectTransform>().localScale = new Vector3(xScale, 1, 1);
    }
}
