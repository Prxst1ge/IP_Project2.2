/*
 * Script Name: RedesignTrigger.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 31/01/2026
 * Description: Controls the clue for the redesign sequence.
 */
using UnityEngine;
using System.Collections;

public class RedesignTrigger : MonoBehaviour
{
    public GameObject uiClue; // World Space UI Canvas here
    public GameObject explanationUI; // The "Why this design is better" info

    public Redesign redesignScript; // For the object with the redesign script

    public bool hasClosedExplanation = false; // To track if explanation was closed

    private bool playerIsInside = false; // To track if player is in trigger

    private void Start()
    {
        // Ensure UI starts hidden
        if (uiClue != null) uiClue.SetActive(false);
        if (explanationUI != null) explanationUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the 'other' object is the VR Player (usually by Tag)
        if (other.CompareTag("Player"))
        {
            if (uiClue != null) uiClue.SetActive(true);
        }
        if (other.CompareTag("Player") && !redesignScript.isRepaired)
        {
            uiClue.SetActive(true); // Only show UI if the work is NOT done
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // When player leaves trigger
        {
            if (uiClue != null) uiClue.SetActive(false);
        }
    }

    // Called by the UI Button to close the explanation
    public void CloseExplanation()
    {
        // 1. Remember that we closed it
        hasClosedExplanation = true;

        // 2. Hide the UI immediately
        if (explanationUI != null) explanationUI.SetActive(false);
    }

    private void Update()
    {
        // Check the redesign state each frame
        if (redesignScript.isRepaired && !hasClosedExplanation)
        {
            if (explanationUI != null) explanationUI.SetActive(true);
            if (uiClue != null) uiClue.SetActive(false); // Hide clue as it's no longer needed
        }
        // If not repaired
        else if (!redesignScript.isRepaired)
        {
            // Ensure Explanation is hidden if not repaired
            if (explanationUI != null) explanationUI.SetActive(false);
        }
    }

}
