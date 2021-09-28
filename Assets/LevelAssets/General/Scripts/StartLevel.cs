using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{
    private string upgrade;
    public bool cleared = false;
    public bool compSci = false;
    public bool MC = false;

    private GameObject player;

    private Scene currentScene;
    private AsyncOperation nextScene;
    // Start is called before the first frame update
    private void Start()
    {
        //Time.timeScale = 0;
        upgrade = GameObject.FindGameObjectWithTag("Upgrade").GetComponent<Upgrade>().upgradeType;
    }

    private void FixedUpdate()
    {
        if (!player)
        {
            try
            {
                player = GameObject.FindGameObjectWithTag("Player");
                DontDestroyOnLoad(player);
                player.transform.position = transform.position;
                Invoke("Ready", 3f);
                Time.timeScale = 1;
                if (compSci)
                {
                    player.GetComponent<CharacterController2D>().topDown = true;
                }
                else
                {
                    player.GetComponent<CharacterController2D>().topDown = false;
                }
                player.GetComponent<CharacterController2D>().enableTime = true;
            }
            catch { }
        }
        if (cleared)
        {
            StartCoroutine(FinishLevel(3));
            cleared = false;
        }
    }

    private void Ready()
    {
        Time.timeScale = 1;
    }

    public void ExitLevel()
    {
        player.GetComponent<CharacterController2D>().Lock(upgrade);
        if (MC)
        {
            StartCoroutine(FinishLevel(0));
        }
        else
        {
            StartCoroutine(FinishLevel(3));
        }
    }

    IEnumerator FinishLevel(int index)
    {
        player.GetComponent<CharacterController2D>().enableTime = false;
        currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.buildIndex);
        nextScene = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
         // should be level navigation menu
        nextScene.allowSceneActivation = false;

        while (nextScene.progress < 0.9f)
        {
            Debug.Log(nextScene.progress);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        nextScene.allowSceneActivation = true;

        while (!nextScene.isDone)
        {
            yield return null;
        }

        Scene sceneToLoad = SceneManager.GetSceneByBuildIndex(index);
        SceneManager.MoveGameObjectToScene(player, sceneToLoad);
        SceneManager.SetActiveScene(sceneToLoad);
        SceneManager.UnloadSceneAsync(currentScene);
    }

    public void BossBeaten()
    {
        player.GetComponent<CharacterController2D>().finishedLevels.Add(player.GetComponent<CharacterController2D>().currentLevel);
        Invoke("Cleared", 2);
    }

    void Cleared()
    {
        cleared = true;
    }
}
