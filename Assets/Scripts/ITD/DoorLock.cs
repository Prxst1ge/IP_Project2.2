/*
 * Script Name: PhysicsDoorLock.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Locks a Rigidbody door by freezing its rotation/position until unlocked.
 */
using UnityEngine;


public class DoorLock : MonoBehaviour
{
    private Rigidbody doorRb;

    // Store the original constraints to restore them later (if needed)
    private RigidbodyConstraints unlockedConstraints;

    void Start()
    {
        doorRb = GetComponent<Rigidbody>();

        // Save how the door moves normally (e.g., usually Freeze Rotation X and Z are set)
        unlockedConstraints = doorRb.constraints;

        // Lock the door immediately on Start
        LockDoor();
    }


    // Freezes the door physics so it cannot be pushed or pulled.
    public void LockDoor()
    {
        // FreezeAll stops the door from moving or rotating entirely.
        doorRb.constraints = RigidbodyConstraints.FreezeAll;
    }


    // Restores physics so the door can swing.

    public void UnlockDoor()
    {
        // Remove all constraints from previous state.
        doorRb.constraints = RigidbodyConstraints.None;

    }
}
