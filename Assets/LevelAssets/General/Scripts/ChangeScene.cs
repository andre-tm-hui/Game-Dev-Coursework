using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public int sceneIndex;
    private GameObject player;
    private Scene currentScene;
    private AsyncOperation nextScene;

    private void Start()
    {
        
    }
    private void FixedUpdate()
    {
        try
        {
            player = GameObject.FindGameObjectWithTag("Player");
            DontDestroyOnLoad(player);
        }
        catch { }
    }

    public void changeScene()
    {
        if (nextScene == null)
        {
            StartCoroutine(next());
        }
        
    }

    IEnumerator next()
    {
        currentScene = SceneManager.GetActiveScene();
        nextScene = SceneManager.LoadSceneAsync(sceneIndex);
        nextScene.allowSceneActivation = false;

        while (nextScene.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        nextScene.allowSceneActivation = true;

        while (!nextScene.isDone)
        {
            yield return null;
        }

        Scene sceneToLoad = SceneManager.GetSceneByBuildIndex(sceneIndex); // should be selection screen
        SceneManager.MoveGameObjectToScene(player, sceneToLoad);
        SceneManager.SetActiveScene(sceneToLoad);
        SceneManager.UnloadSceneAsync(currentScene);
    }
}
