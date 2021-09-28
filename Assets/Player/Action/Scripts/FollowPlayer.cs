using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject Player;
    public bool followPlayer = true;
    public Vector3 position;
    public bool lerpMove = false;
    public float lerpTime = 1f;
    private float Timer = 0;

    public float yOffset = 8f;
    private Vector3 offset;
    // Start is called before the first frame update

    private void Update()
    {
        if (!Player)
        {
            try
            {
                Player = GameObject.FindGameObjectWithTag("Player");
                offset = transform.position - Player.transform.position;
                offset.x = 0;
                offset.y = yOffset;
            }
            catch { }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (followPlayer)
        {
            transform.position = Player.transform.position + offset;
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        } else
        {
            Timer += Time.deltaTime;
            if (Timer <= lerpTime && lerpMove)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    position,
                    Timer / lerpTime
                    );
            } else
            {
                lerpMove = false;
                Timer = 0;
            }
        }
    }
}
