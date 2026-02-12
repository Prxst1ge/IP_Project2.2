/*
 * Script Name: PhysicsDoorLock.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Locks a Rigidbody door by freezing its rotation/position until unlocked.
 */
using UnityEngine;
using System.Collections;


public class DoorLock : MonoBehaviour
{
    private Rigidbody doorRb; // The Rigidbody component of the door
    private HingeJoint doorHinge; // Reference to the Hinge
    private RigidbodyConstraints unlockedConstraints;// Store the original constraints to restore them later (if needed)

    public float openAngle = 90f;   // Target angle (e.g. 90 degrees)
    public float speed = 2.0f;      // How fast it opens
    private bool isOpening = false; // Is the door currently opening?

    /// <summary>
    /// When the game starts, get the Rigidbody and HingeJoint components, save the original constraints, and lock the door immediately.
    /// </summary>
    void Start()
    {
        doorRb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        doorHinge = GetComponent<HingeJoint>(); // Get the HingeJoint component

        // Save how the door moves normally 
        unlockedConstraints = doorRb.constraints;

        // Lock the door immediately on Start
        LockDoor();
    }


    /// <summary>
    /// Locks the door by freezing all movement and rotation, effectively making it immovable until unlocked.
    /// </summary>
    public void LockDoor()
    {
        // FreezeAll stops the door from moving or rotating entirely.
        doorRb.constraints = RigidbodyConstraints.FreezeAll;
    }


    /// <summary>
    /// Unlocks the door by removing constraints and starts the opening animation around the hinge. It ensures that the door only starts opening if it's not already in the process of opening.
    /// </summary>

    public void UnlockDoor()
    {
        // Don't unlock again if already opening
        if (isOpening) return;

        Debug.Log("Door Unlocked: Starting Auto-Open");

        // Remove all constraints from previous state.
        doorRb.constraints = RigidbodyConstraints.None;

        StartCoroutine(AnimateOpenAroundHinge());
    }

    /// <summary>
    /// Animates the door opening by rotating it around the hinge's pivot point. It calculates the pivot point based on the HingeJoint's anchor and rotates the door incrementally until it reaches the target open angle. The door's Rigidbody is set to kinematic during this animation to allow for precise control over its movement without physics interference.
    /// </summary>
    private IEnumerator AnimateOpenAroundHinge()
    {
        isOpening = true;
        doorRb.isKinematic = true; // Disable physics to control movement manually

        // Find the exact position of the Hinge in the world
        Vector3 pivotPoint = transform.position;
        Vector3 rotateAxis = Vector3.up;

        if (doorHinge != null)
        {
            // Convert local anchor point to world space
            pivotPoint = transform.TransformPoint(doorHinge.anchor);
            rotateAxis = doorHinge.axis;
        }

        float currentAngle = 0f;

        // Rotate incrementally until we reach the target angle
        while (currentAngle < Mathf.Abs(openAngle))
        {
            float step = speed * Time.deltaTime;

            // Ensure we don't over-rotate past the target
            if (currentAngle + step > Mathf.Abs(openAngle))
            {
                step = Mathf.Abs(openAngle) - currentAngle;
            }

            // Apply rotation around the specific Pivot Point (Hinge)

            float direction = Mathf.Sign(openAngle);
            transform.RotateAround(pivotPoint, rotateAxis, step * direction);

            currentAngle += step;
            yield return null;
        }
    }

}
