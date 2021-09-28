using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCompSci : MonoBehaviour
{
    public Transform topLeft;
    public Transform bottomRight;
    public int numOfNodes = 50;
    public GameObject nodePrefab;
    public GameObject exit;
    private Vector3 direction; // left (0,0,0) down (0,0,-90) right (0,0,180) up (0,0,90)
    // Start is called before the first frame update
    void Start()
    {
        exit.transform.position = new Vector2(Random.Range(bottomRight.position.x, topLeft.position.x), Random.Range(bottomRight.position.y, topLeft.position.y));
        for (int i = 0; i < numOfNodes; i++)
        {
            Vector2 position = new Vector2(0, 0);
            if (Random.Range(0f, 1f) < (bottomRight.position.x - topLeft.position.x) / (bottomRight.position.x - topLeft.position.x + topLeft.position.y - bottomRight.position.y))
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    position = new Vector2(Random.Range(bottomRight.position.x, topLeft.position.x), topLeft.position.y);
                    direction = new Vector3 (0, 0, -90);
                } else
                {
                    position = new Vector2(Random.Range(bottomRight.position.x, topLeft.position.x), bottomRight.position.y);
                    direction = new Vector3(0, 0, 90);
                }
            } else
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    position = new Vector2(topLeft.position.x, Random.Range(bottomRight.position.y, topLeft.position.y));
                    direction = new Vector3(0, 0, 0);
                }
                else
                {
                    position = new Vector2(bottomRight.position.x, Random.Range(bottomRight.position.y, topLeft.position.y));
                    direction = new Vector3(0, 0, 180);
                }
            }

            GameObject node = Instantiate(
                nodePrefab,
                position,
                Quaternion.identity
                );
            node.transform.Rotate(direction);
        }

        // instantiate exit
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
