/*
 * Script Name: SimpleDoorLock.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Locks the door by making its Rigidbody kinematic until unlocked.
 */
using UnityEngine;

public class SimpleDoorLock : MonoBehaviour
{
    private Rigidbody doorRb;

    void Start()
    {
        doorRb = GetComponent<Rigidbody>();

        // Lock the door immediately when the game starts
        LockDoor();
    }

    public void LockDoor()
    {
        // "isKinematic = true" freezes the door so it can't be pushed
        if (doorRb != null)
        {
            doorRb.isKinematic = true;
        }
    }

    // Connect this to your Scanner's OnCardEnter event
    public void UnlockDoor()
    {
        Debug.Log("Door Unlocked: Physics enabled.");

        // Enabling physics lets the player push/pull the door manually
        if (doorRb != null)
        {
            doorRb.isKinematic = false;

            // Optional: A tiny "wake up" nudge ensures the physics engine sees the change immediately
            doorRb.WakeUp();
        }
    }
}