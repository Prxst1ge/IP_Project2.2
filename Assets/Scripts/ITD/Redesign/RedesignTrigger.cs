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
    public Transform vrHeadset; // Player's head/camera transform

    public GameObject uiClue; // World Space UI Canvas here
    public GameObject explanationUI; // The "Why this design is better" info

    public Redesign redesignScript; // For the object with the redesign script
    public float spawnDistance = 1.5f; // Distance from player to spawn UI
    public LayerMask obstacleLayer = 1; // Default layer is 1


    public float animationDuration = 2f; // How long the pop-up takes
    public float spinAmount = 180f; // How much it spins during pop-up

    private Vector3 clueTargetScale; // To store target scale for clue
    private Vector3 explainTargetScale; // To store target scale for explanation
    private bool isAnimating = false; // To prevent Update() conflicts  

    /// <summary>
    /// Initial setup
    /// </summary>
    private void Start()
    {
        // Auto-assign VR Headset if not set
        if (vrHeadset == null)
        {
            if (Camera.main != null) vrHeadset = Camera.main.transform;
            else Debug.LogError("CRITICAL ERROR: No Camera found! Assign 'vrHeadset' in Inspector.");
        }

        // Memorize the correct size from the Inspector before we hide them
        if (uiClue != null)
        {
            clueTargetScale = uiClue.transform.localScale;
            uiClue.SetActive(false);
        }

        if (explanationUI != null)
        {
            explainTargetScale = explanationUI.transform.localScale;
            explanationUI.SetActive(false);
        }
    }

    /// <summary>
    /// Handles player entering the trigger zone
    /// </summary>
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

                    StartCoroutine(AnimatePopUp(uiClue, clueTargetScale));
                }
            }
        }
    }

    /// <summary>
    /// Handles player exiting the trigger zone
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // When player leaves trigger
        {
            if (uiClue != null) uiClue.SetActive(false); // Hide the clue UI
        }
    }

    /// <summary>
    /// Called by UI Button to close the explanation
    /// </summary>
    public void CloseExplanation()
    {
        // Tell the redesign script that explanation has been seen
        redesignScript.explanationSeen = true;

        if (explanationUI != null) explanationUI.SetActive(false);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // If animating, let the Coroutine handle the position!
        if (isAnimating) return;

        if (redesignScript.isRepaired && !redesignScript.explanationSeen)
        {
            if (explanationUI != null && !explanationUI.activeSelf)
            {
                explanationUI.SetActive(true);
                StartCoroutine(AnimatePopUp(explanationUI, explainTargetScale));
            }
            if (uiClue != null) uiClue.SetActive(false);
        }
        else if (uiClue != null && uiClue.activeSelf)
        {
            // Only track face if we are NOT animating (and not already done)
            ForcePositionNow(uiClue);
        }
    }

    /// <summary>
    /// Resets RectTransform offsets to zero
    /// </summary>
    void ResetRectTransform(GameObject obj)
    {
        // If the custom UI has a RectTransform with weird positions, fix it
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect != null)
        {
            // Keep the pivot, but zero out the local position offset
            rect.anchoredPosition3D = Vector3.zero;
        }
    }

    /// <summary>
    /// Positions the given UI object in front of the player, avoiding walls.
    /// </summary>
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

    /// <summary>
    /// Animate a UI object's scale from zero to a target size.
    /// </summary>
    IEnumerator AnimatePopUp(GameObject uiObject, Vector3 finalSize)
    {
        isAnimating = true;
        float timer = 0;

        Vector3 startSize = Vector3.zero;
        uiObject.transform.localScale = startSize;

        while (timer < animationDuration)
        {
            float progress = timer / animationDuration;

            // animate scale
            float easeScale = 1 - Mathf.Pow(1 - progress, 3);
            uiObject.transform.localScale = Vector3.Lerp(startSize, finalSize, easeScale);

            // force position update
            ForcePositionNow(uiObject);

            // apply 
            float currentSpin = Mathf.Lerp(spinAmount, 0, easeScale);
            uiObject.transform.Rotate(0, currentSpin, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        // Finalize
        uiObject.transform.localScale = finalSize;
        ForcePositionNow(uiObject); // Snap to final position one last time

        isAnimating = false;
    }

    /// <summary>
    /// Immediately positions the UI object in front of the player
    /// </summary>
    void ForcePositionNow(GameObject uiObject)
    {
        // Fallback if forgot to assign camera
        if (vrHeadset == null)
        {
            if (Camera.main != null) vrHeadset = Camera.main.transform;
            else return;
        }

        Vector3 cameraPos = vrHeadset.position;
        Vector3 forwardDir = vrHeadset.forward;

        // Calculate Position relative to CURRENT headset position
        Vector3 finalPosition = cameraPos + (forwardDir * spawnDistance);

        // Apply Position
        uiObject.transform.position = finalPosition;

        // Face the headset
        uiObject.transform.LookAt(2 * uiObject.transform.position - vrHeadset.position);
    }

}
