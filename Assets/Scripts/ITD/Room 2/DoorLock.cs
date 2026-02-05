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

    void Start()
    {
        doorRb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        doorHinge = GetComponent<HingeJoint>(); // Get the HingeJoint component

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
        // If it's already opening, don't run this again
        if (isOpening) return;

        Debug.Log("Door Unlocked: Starting Auto-Open");

        // Remove all constraints from previous state.
        doorRb.constraints = RigidbodyConstraints.None;

        StartCoroutine(AnimateOpenAroundHinge());
    }

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
