using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public string upgradeType;
    public SpriteRenderer E;

    private void OnDestroy()
    {
        if (upgradeType == "health" || upgradeType == "book")
        {
            GameObject.FindGameObjectWithTag("Level").GetComponent<StartLevel>().BossBeaten();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("hello");
        if (collision.gameObject.CompareTag("Player"))
        {
            E.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            E.enabled = false;
        }
    }

    private void Update()
    {
        if (E.enabled && Input.GetButtonDown("Use"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>().Unlock(upgradeType);
            Destroy(gameObject);
        }
    }
}
