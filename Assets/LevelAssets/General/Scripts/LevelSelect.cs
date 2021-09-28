using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public List<Transform> levels;
    public List<int> sceneIndices;
    private GameObject player;
    private int currentLevel = 0;
    private List<int> finishedLevels;

    private bool lerping = false;
    private float lerpTime = 0.5f;
    private float timer = 1f;
    private float freezeTime = 0;
    private bool loading = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (!player)
        {
            try
            {
                player = GameObject.FindGameObjectWithTag("Player");
                if (player.GetComponent<CharacterController2D>().currentLevel >= 0 && player.GetComponent<CharacterController2D>().currentLevel < levels.Count)
                {
                    currentLevel = player.GetComponent<CharacterController2D>().currentLevel;
                }
                player.transform.position = levels[currentLevel].position;
                player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                player.GetComponent<CharacterController2D>().Heal();
                finishedLevels = player.GetComponent<CharacterController2D>().finishedLevels;
                foreach (int i in finishedLevels)
                {
                    Debug.Log(i);
                }
            } 
            catch { }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (freezeTime > 1 && !loading)
        {
            if (Input.GetButtonDown("Horizontal") && player && timer > lerpTime)
            {
                if (Input.GetAxisRaw("Horizontal") > 0 && currentLevel < levels.Count - 1)
                {
                    currentLevel++;
                }
                else if (Input.GetAxisRaw("Horizontal") < 0 && currentLevel > 0)
                {
                    currentLevel--;
                }
                timer = 0;
            }

            if (Input.GetButtonDown("Submit") && player && timer > lerpTime && !finishedLevels.Contains(currentLevel))
            {
                Debug.Log("submit");
                GetComponent<ChangeScene>().sceneIndex = sceneIndices[currentLevel];
                player.GetComponent<CharacterController2D>().currentLevel = currentLevel;
                player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                loading = true;
                GetComponent<ChangeScene>().changeScene();
            }

            if (timer < lerpTime && player)
            {
                timer += Time.deltaTime;
                player.transform.position = Vector2.Lerp(player.transform.position, levels[currentLevel].position, timer / lerpTime);
            }
        } else
        {
            freezeTime += Time.deltaTime;
        }
        
    }
}
