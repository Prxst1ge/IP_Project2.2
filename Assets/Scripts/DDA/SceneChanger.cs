using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private float loadDelay = 2f;

    /// <summary>
    /// Load the target scene set in the Inspector with a delay
    /// </summary>
    public void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            StartCoroutine(LoadSceneWithDelay(targetScene));
        }
        else
        {
            Debug.LogError("Target Scene is not set in the Inspector!");
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithDelay(sceneName));
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneWithDelayIndex(sceneIndex));
    }

    public void ReloadScene()
    {
        StartCoroutine(LoadSceneWithDelay(SceneManager.GetActiveScene().name));
    }

    /// <summary>
    /// Load scene after a timed delay
    /// </summary>
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(loadDelay);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneWithDelayIndex(int sceneIndex)
    {
        yield return new WaitForSeconds(loadDelay);
        SceneManager.LoadScene(sceneIndex);
    }

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
