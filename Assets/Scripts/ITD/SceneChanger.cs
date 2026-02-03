using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string targetScene;

    /// <summary>
    /// Load the target scene set in the Inspector
    /// </summary>
    public void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("Target Scene is not set in the Inspector!");
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
