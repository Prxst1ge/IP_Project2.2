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
    public LayerMask obstacleLayer = 1; // Default layer is 1


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
        // For real-time tracking and updates
        if (redesignScript.isRepaired && !redesignScript.explanationSeen)
        {
            // Only run this IF the explanation is currently hidden
            if (explanationUI != null && !explanationUI.activeSelf)
            {
                // Position it ONCE right now
                PositionUIInFrontOfPlayer(explanationUI);

                // Then turn it on (it will stay there)
                explanationUI.SetActive(true);
            }

            // Ensure Clue is hidden
            if (uiClue != null) uiClue.SetActive(false);
        }

        // For clue tracking
        else if (uiClue != null && uiClue.activeSelf)
        {
            // Run this EVERY FRAME so it follows player's face
            PositionUIInFrontOfPlayer(uiClue);
        }

        else
        {
            // If explanation is seen, hide it
            if (explanationUI != null && redesignScript.explanationSeen)
            {
                explanationUI.SetActive(false);
            }
        }
    }

    void PositionUIInFrontOfPlayer(GameObject uiObject)
    {
        Camera playerCam = Camera.main;
        if (playerCam == null) return;

        Vector3 cameraPos = playerCam.transform.position;
        Vector3 forwardDir = playerCam.transform.forward;

        // Default target position (1.5m away)
        Vector3 finalPosition = cameraPos + (forwardDir * spawnDistance);

        // WALL CHECK (Raycast)
        RaycastHit hit;
        // Shoot a ray from eyes, forward, for the length of spawnDistance
        if (Physics.Raycast(cameraPos, forwardDir, out hit, spawnDistance, obstacleLayer))
        {
            // If a wall is hit, position the UI slightly in front of the wall
            finalPosition = hit.point - (forwardDir * 0.2f);
        }

        // Lock Height (Keep it at eye level)
        finalPosition.y = cameraPos.y;

        // Apply Position
        uiObject.transform.position = finalPosition;

        // Rotate to face the camera
        uiObject.transform.LookAt(playerCam.transform);
        uiObject.transform.Rotate(0, 180, 0);
    }


}
