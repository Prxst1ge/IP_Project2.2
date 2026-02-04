/*
 * Script Name: RepairButton.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 04/02/2026
 * Description: Controls the repair button for the redesign sequence.
 */
using UnityEngine;

public class RepairButton : MonoBehaviour
{
    public Redesign redesignScript; // Main Redesign object to trigger repair on

    public string targetTag = "Player"; // Defaults to Player

    // Cooldown to prevent accidental double-touches
    private float cooldown = 1.0f;
    private float lastTouchTime;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object touching the box is the Player
        if (other.CompareTag(targetTag))
        {
            // Check cooldown timer
            if (Time.time - lastTouchTime > cooldown)
            {
                lastTouchTime = Time.time; // Reset timer

                if (redesignScript != null)
                {
                    redesignScript.PerformRepair();
                    Debug.Log("Repair triggered by: " + gameObject.name);
                }
            }
        }
    }
}