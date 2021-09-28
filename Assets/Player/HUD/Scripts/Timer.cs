using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private GameObject player;
    private TMPro.TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        if (!player)
        {
            try
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            catch { }
        } else
        {
            float timer = player.GetComponent<CharacterController2D>().elapsedTime;
            string minutes = Mathf.Floor(timer / 60).ToString();
            string seconds = Mathf.RoundToInt(timer % 60).ToString("00");
            text.text = string.Format("{0}:{1}", minutes, seconds);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
