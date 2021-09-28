using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Pause");
            togglePause();
        }
    }

    private void togglePause()
    {
        Debug.Log("toggle");
        GameObject.FindGameObjectWithTag("Pause").GetComponent<Canvas>().enabled = !GameObject.FindGameObjectWithTag("Pause").GetComponent<Canvas>().enabled;
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
            foreach(Button b in GetComponentsInChildren<Button>())
            {
                b.interactable = true;
            }
        }
    }

    public void returnToGame()
    {
        GameObject.FindGameObjectWithTag("Pause").GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1;
    }

    public void returnToSelect()
    {
        Time.timeScale = 1;
        GameObject.FindGameObjectWithTag("Level").GetComponent<StartLevel>().ExitLevel();
    }
}
