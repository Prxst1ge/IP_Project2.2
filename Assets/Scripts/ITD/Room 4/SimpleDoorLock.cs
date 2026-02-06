/*
 * Script Name: SimpleDoorLock.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Locks a Rigidbody door by freezing its rotation/position until unlocked.
 */
using UnityEngine;

public class SimpleDoorLock : MonoBehaviour
{
    private Rigidbody doorRb;

    void Start()
    {
        doorRb = GetComponent<Rigidbody>();

        // Lock the door immediately on start
        LockDoor();
    }

    public void LockDoor()
    {
        // Making it kinematic is the most stable way to "Lock" it.
        // It becomes an immovable object that ignores grabs.
        if (doorRb) doorRb.isKinematic = true;
    }

    // Connect this to your Scanner's "OnCardEnter" event
    public void UnlockDoor()
    {
        Debug.Log("Door Unlocked! Physics enabled.");

        // Enable physics so the player can push/pull the door
        if (doorRb)
        {
            doorRb.isKinematic = false;

            // Optional: Give it a tiny nudging force to wake up the physics engine
            // This prevents the door from feeling "stuck" for the first millisecond
            doorRb.AddForce(transform.forward * 0.1f, ForceMode.Impulse);
        }
    }
}
