using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private bool isLoading = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSceneAsync(string sceneName, float delay = 0.0f)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, delay));
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);

        isLoading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        isLoading = false;
    }

    public void LoadAdditiveSceneAtPosition(string sceneName, Vector3 targetPosition, System.Action onLoaded = null)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadAdditiveSceneCoroutine(sceneName, targetPosition, onLoaded));
        }
    }

    private IEnumerator LoadAdditiveSceneCoroutine(string sceneName, Vector3 targetPosition, System.Action onLoaded)
    {
        isLoading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
            yield return null;

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        while (!loadedScene.isLoaded)
            yield return null;

        foreach (GameObject rootObj in loadedScene.GetRootGameObjects())
        {
            if (rootObj.name == "Root") // TODO: check if this main object detection logic is appropriate
            {
                rootObj.transform.position = targetPosition;
                break;
            }
        }

        onLoaded?.Invoke();
        isLoading = false;
    }

    public bool IsLoading()
    {
        return isLoading;
    }
}

// TODO: perform fade-out transition
