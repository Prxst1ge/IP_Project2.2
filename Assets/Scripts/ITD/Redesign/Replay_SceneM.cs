/*
 * Script Name: Replay_SceneM.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 1/02/2026
 * Description: Manages the transit to replay scene after player has completed all redesign.
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class Replay_SceneM : MonoBehaviour
{
    public GameObject exitDoorObject; // The door to exit the scene
    public Redesign[] itemsToFix; // Array of redesign items to check
    public AudioSource successAudio; // Audio source for success sound

    private bool allTasksComplete = false;
    private bool levelIsLoading = false; // Safety switch to prevent crashing

    /// <summary>
    /// Initial setup
    /// </summary>
    void Start()
    {
        // Ensure the exit door is hidden at the start
        if (exitDoorObject != null) exitDoorObject.SetActive(false);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // If tasks are already complete, no need to check again
        if (allTasksComplete) return;
        // Safety check to prevent multiple loads
        if (levelIsLoading) return;

        if (CheckIfAllDone())
        {
            Debug.Log("All items fixed and explained! Spawning Exit Door.");
            allTasksComplete = true; // Mark as done so this block only runs once

            // Showing the door to go to the next scene
            if (exitDoorObject != null)
            {
                exitDoorObject.SetActive(true);
            }
            if (successAudio != null)
            {
                successAudio.Play();
            }
        }
    }

    /// <summary>
    /// Checks if all redesign items are repaired and explained
    /// </summary>
    private bool CheckIfAllDone()
    {
        foreach (Redesign item in itemsToFix)
        {
            if (!item.isRepaired) return false;
        }

        // If all zones are done and explanations closed
        return true;
    }
}