using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject mainBg;
    public Transform[] backgrounds;
    private float[] parallaxScales;                 // proportion of camera movement
    public float smoothing = 1;

    private Transform cam;
    private Vector3 previousCamPos;

    private float mainBg_zIndex;

    private void Awake()
    {
        cam = Camera.main.transform;
        mainBg_zIndex = mainBg.transform.position.z;
    }
    // Start is called before the first frame update
    void Start()
    {
        previousCamPos = cam.position;

        parallaxScales = new float[backgrounds.Length];
        
        for (int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            Vector3 targetPos = new Vector3(backgrounds[i].position.x + parallax, backgrounds[i].position.y, backgrounds[i].position.z);
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, targetPos, smoothing * Time.deltaTime);
        }
        previousCamPos = cam.position;
        Vector3 mainBgTarget = new Vector3 (previousCamPos.x, previousCamPos.y, mainBg_zIndex);
        mainBg.transform.position = Vector3.Lerp(mainBg.transform.position, mainBgTarget, 1);
    }
}
