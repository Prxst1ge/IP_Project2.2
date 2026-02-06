/*
 * Script Name: MugTriggerZone.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Triggers the next door when a coffee mug is delivered.
 */
using UnityEngine;

public class MugTriggerZone : MonoBehaviour
{

    public string requiredTag = "CoffeeMug"; // Tag your Mug with this!
    public GameObject nextDoorObject;        // The door you want to spawn

    private bool hasTriggered = false;

    // automatically hide the door when the game begins
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

    private void OnTriggerEnter(Collider other)
    {
        // Prevent running this multiple times
        if (hasTriggered) return;

        if (other.CompareTag(requiredTag))
        {
            hasTriggered = true;
            Debug.Log("Mission Complete: Mug delivered!");

            // Spawn (enable) the next door
            if (nextDoorObject != null)
            {
                nextDoorObject.SetActive(true);
            }
        }
    }
}
