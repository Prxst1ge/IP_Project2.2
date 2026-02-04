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
    public float spawnDistance = 1.5f; // Distance from player to spawn UI


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
            if (!redesignScript.isRepaired)
            {
                // Only move and show if it's currently hidden
                if (uiClue != null && !uiClue.activeSelf)
                {
                    PositionUIInFrontOfPlayer(uiClue);
                    uiClue.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // When player leaves trigger
        {
            if (uiClue != null) uiClue.SetActive(false); // Hide the clue UI
        }
    }

    // Called by the UI Button to close the explanation
    public void CloseExplanation()
    {
        // Tell the redesign script that explanation has been seen
        redesignScript.explanationSeen = true;

        if (explanationUI != null) explanationUI.SetActive(false);
    }

    void Update()
    {
        // If repaired AND explanation NOT seen yet
        if (redesignScript.isRepaired && !redesignScript.explanationSeen)
        {
            // Show Explanation UI
            if (explanationUI != null && !explanationUI.activeSelf)
            {
                PositionUIInFrontOfPlayer(explanationUI);
                explanationUI.SetActive(true);
            }
            // Hide Clue UI
            if (uiClue != null) uiClue.SetActive(false);
        }
        else
        {
            // If explanation IS seen, ensure UI is hidden
            if (explanationUI != null) explanationUI.SetActive(false);
        }
    }

    void PositionUIInFrontOfPlayer(GameObject uiObject)
    {
        Camera playerCam = Camera.main; // Automatically finds camera tagged "MainCamera"

        if (playerCam == null)
        {
            Debug.LogError("Could not find MainCamera! Ensure your VR Headset Camera is tagged 'MainCamera'.");
            return;
        }

        // Calculate position: Start at camera, move forward X meters
        Vector3 targetPos = playerCam.transform.position + (playerCam.transform.forward * spawnDistance);

        // Lock Height: Keep it at eye level 
        targetPos.y = playerCam.transform.position.y;

        // Apply Position
        uiObject.transform.position = targetPos;

        // Rotate to face the camera
        uiObject.transform.LookAt(playerCam.transform);

        // Flip 180 degrees (because UI usually faces 'backwards' by default)
        uiObject.transform.Rotate(0, 180, 0);
    }

}
