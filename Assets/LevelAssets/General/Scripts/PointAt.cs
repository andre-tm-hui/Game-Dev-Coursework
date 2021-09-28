using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointAt : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform target;
    public GameObject Pointer;
    public bool m_trackOffScreen = false;
    public float m_edgeBuffer = 0.03f;
    private float heightOffset = 3f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!target)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (m_trackOffScreen)
        {
            Vector2 pos = transform.position;
            pos = Camera.main.WorldToViewportPoint(pos);

            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
            {
                Pointer.GetComponent<Image>().enabled = true;
                RectTransform CanvasRect = GameObject.FindGameObjectWithTag("HUD").GetComponent<RectTransform>();
                Vector2 direction = (rb.position - (Vector2)target.position).normalized;
                Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target.position + new Vector3(0, heightOffset, 0));
                Vector2 PointerPosition = new Vector2(
                    (ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f),
                    (ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f));
                //Vector2 PointerPosition = new Vector2(0, m_PointerYOffset);
                while (Mathf.Abs(PointerPosition.x) < 890 && Mathf.Abs(PointerPosition.y) < 470)
                {
                    PointerPosition += direction;
                }
                Pointer.GetComponent<RectTransform>().anchoredPosition = PointerPosition;
                Pointer.GetComponent<RectTransform>().rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
            }
            else
            {
                Pointer.GetComponent<Image>().enabled = false;
            }
        }
    }
}
