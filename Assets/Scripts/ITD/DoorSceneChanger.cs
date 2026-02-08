/*
 * Script Name: DoorSceneChanger.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 03/02/2026
 * Description: Controls the scene change when player enters a door trigger.
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneChanger : MonoBehaviour
{
    public string sceneToLoad; // Name of the scene to load when player enters

    public string debugMessage = "Player entered the door!"; // Debug message to show in console

    // This runs when the player walks into the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            LoadDestination();
        }
        if (other.CompareTag("Player"))
        {
            Debug.Log(debugMessage);
            LoadDestination();
        }
    }

    // Loads the specified scene
    private void LoadDestination()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError($"Door '{gameObject.name}' has no Scene Name assigned!");
        }
    }
}
