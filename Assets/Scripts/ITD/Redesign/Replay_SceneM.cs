/*
 * Script Name: Replay_SceneM.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 1/02/2026
 * Description: Manages the replay scene after player has completed all redesign.
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class Replay_SceneM : MonoBehaviour
{

    public string sceneToLoad = "Level2"; // Name of the next scene to load
    public GameObject confirmationCanvas; // Canvas to confirm replay


    public Redesign[] itemsToFix; // An array (list) of redesign scripts
    public RedesignTrigger[] redesignZones;

    private bool allTasksComplete = false;
    private bool levelIsLoading = false; // Safety switch to prevent crashing

    void Start()
    {
        // Ensure the popup is hidden at the start
        if (confirmationCanvas != null) confirmationCanvas.SetActive(false);
    }

    void Update()
    {
        // If tasks are already complete, no need to check again
        if (allTasksComplete) return;
        // Safety check to prevent multiple loads
        if (levelIsLoading) return;

        if (CheckIfAllDone())
        {
            Debug.Log("All items fixed and read! Showing Next Level UI.");
            allTasksComplete = true; // Mark as done so this block only runs once

            // Showing the confirmation canvas
            if (confirmationCanvas != null) confirmationCanvas.SetActive(true);
        }
    }

    private bool CheckIfAllDone()
    {
        foreach (RedesignTrigger zone in redesignZones)
        {
            // Whether the redesign in this zone is done
            if (!zone.redesignScript.isRepaired) return false;

            // Whether the explanation UI has been closed
            if (!zone.hasClosedExplanation) return false;
        }

        // If all zones are done and explanations closed
        return true;
    }

    // Called by the UI Button to load the next level
    public void LoadNextLevel()
    {
        if (!levelIsLoading)
        {
            levelIsLoading = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }

}