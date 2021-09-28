using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public int holeNum;
    public Transform teleportPoint;
    private bool isAnswer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            if (!isAnswer)
            {
                player.GetComponent<CharacterController2D>().Damage(0.5f);
            }
            if (!transform.parent.GetComponent<Question>().isFinished())
            {
                player.transform.position = teleportPoint.position + player.transform.position - transform.position;
                transform.parent.GetComponent<Question>().changeQuestion();
            }
            
        }
        
    }

    private void FixedUpdate()
    {
        if (holeNum == transform.parent.GetComponent<Question>().getAnswer())
        {
            isAnswer = true;
        }
        else
        {
            isAnswer = false;
        }
    }
}
