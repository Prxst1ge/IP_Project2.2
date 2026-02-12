/*
 * Script Name: SimpleDoorLock.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 02/02/2026
 * Description: Locks the door by making its Rigidbody kinematic until unlocked.
 */
using UnityEngine;

public class SimpleDoorLock : MonoBehaviour
{
    private Rigidbody doorRb;// Reference to the door's Rigidbody

    /// <summary>
    /// Called when the scene starts to initialize the door lock.
    /// </summary>
    void Start()
    {
        doorRb = GetComponent<Rigidbody>();

        // Lock the door immediately when the game starts
        LockDoor();
    }

    /// <summary>
    /// Locks the door by setting its Rigidbody to kinematic.
    /// </summary>
    public void LockDoor()
    {
        // isKinematic = true, freezes the door so it can't be pushed
        if (doorRb != null)
        {
            doorRb.isKinematic = true;
        }
    }

    /// <summary>
    /// Unlocks the door by disabling its kinematic state, allowing physics interactions.
    /// </summary>
    public void UnlockDoor()
    {
        Debug.Log("Door Unlocked: Physics enabled.");

        // Enabling physics lets the player push/pull the door manually
        if (doorRb != null)
        {
            doorRb.isKinematic = false;

            // Wake up the Rigidbody to ensure it responds immediately
            doorRb.WakeUp();
        }
    }
}