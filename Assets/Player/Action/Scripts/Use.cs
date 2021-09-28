using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Use : MonoBehaviour
{
    public bool finalTreasure = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void run()
    {
        Debug.Log("run");
        if (!finalTreasure)
        {
            GameObject.FindGameObjectWithTag("Level").GetComponent<StartLevel>().BossBeaten();
        }
        else
        {
            // change scene to ending scene
            Debug.Log("Winner");
            CharacterController2D player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();
            player.enableTime = false;
            string finalTime = (Mathf.Floor(player.elapsedTime / 60)).ToString() + ":" + (player.elapsedTime % 60).ToString("00");
            GameObject.FindGameObjectWithTag("FinalTime").GetComponent<TMPro.TextMeshProUGUI>().text = finalTime;
            GameObject.FindGameObjectWithTag("WinScreen").GetComponent<Canvas>().enabled = true;

        }
        
    }
}
