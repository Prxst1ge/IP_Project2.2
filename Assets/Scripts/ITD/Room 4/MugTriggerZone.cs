/*
 * Script Name: MugTriggerZone.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 05/02/2026
 * Description: Triggers the next door when a coffee mug is delivered.
 */
using UnityEngine;

public class MugTriggerZone : MonoBehaviour
{

    public string mugTag = "CoffeeMug"; // Mug tag
    public string keycardTag = "Keycard"; // Keycard tag
    public GameObject nextDoorObject;        // The door we want to spawn

    private bool hasMug = false;// Mug delivery flag
    private bool hasKeycard = false;// 
    private bool doorOpened = false;// Door opened flag

    /// <summary>
    /// Called when the scene starts to initialize the trigger zone.
    /// </summary>
    void Start()
    {
        if (nextDoorObject != null)
        {
            nextDoorObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("MugTriggerZone: No 'Next Door Object' assigned!");
        }
    }

    /// <summary>
    /// Called when an object enters the trigger zone.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Prevent running this multiple times
        if (doorOpened) return;

        // Check for Mug
        if (other.CompareTag(mugTag))
        {
            if (!hasMug)
            {
                hasMug = true;
                Debug.Log("Item 1/2: Mug Delivered!");
            }
        }

        // Check for Keycard
        if (other.CompareTag(keycardTag))
        {
            if (!hasKeycard)
            {
                hasKeycard = true;
                Debug.Log("Item 2/2: Keycard Delivered!");
            }
        }
        CheckForCompletion();
    }

    /// <summary>
    /// Checks if both items have been delivered and opens the next door.
    /// </summary>
    private void CheckForCompletion()
    {
        // If both flags are true, open the door
        if (hasMug && hasKeycard)
        {
            doorOpened = true;
            Debug.Log("Mission Complete: All items delivered! Door Opening.");

            if (nextDoorObject != null)
            {
                nextDoorObject.SetActive(true);
            }
        }
    }
}
