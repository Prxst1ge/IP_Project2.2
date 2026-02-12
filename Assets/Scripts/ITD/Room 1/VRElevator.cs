/*
 * Script Name: VRElevator.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Controls the movement of a VR elevator platform.
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class VRElevator : MonoBehaviour
{
    public Transform topStop; // Lift top position
    public Transform bottomStop;// Lift bottom position
    public float speed = 2.0f; // Speed of the elevator
    public float startDelay = 0.5f; // Delay before starting movement
    public LiftDoor liftDoor; // Reference to the LiftDoor script

    public UnityEvent onReachedBottom;// Event invoked when the elevator reaches the bottom

    // Internal state
    private bool isMoving = false; // Flag to indicate if the elevator is currently moving
    private Transform currentTarget; // The current target position (top or bottom)
    private Rigidbody rb; // Rigidbody reference for physics interactions

    /// <summary>
    /// Called when the scene starts to initialize the elevator.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    /// <summary>
    /// Checks if the doors are fully closed.
    /// </summary>
    void FixedUpdate() // Using FixedUpdate for physics consistency
    {
        if (isMoving && currentTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

            // Check if we have arrived
            if (Vector3.Distance(transform.position, currentTarget.position) < 0.01f)
            {
                isMoving = false;

                // Automatically open the door upon arrival
                if (liftDoor != null)
                {
                    liftDoor.OpenDoor();
                }
                // Invoke event if reached bottom
                if (currentTarget == bottomStop)
                {
                    onReachedBottom?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Go up to the top stop, but only if the doors are fully closed and we aren't already at the top.
    /// </summary>
    public void GoUp()
    {
        // Check if door is closed first
        if (liftDoor != null && !liftDoor.IsFullyClosed())
        {
            Debug.Log("Waiting for doors to close...");
            return;
        }
        // Only move if we aren't already at the top
        if (Vector3.Distance(transform.position, topStop.position) > 0.1f)
        {
            StartCoroutine(StartMovingRoutine(topStop));
        }
    }

    /// <summary>
    /// Go down to the bottom stop, but only if the doors are fully closed and we aren't already at the bottom.
    /// </summary>
    public void GoDown()
    {
        // Check if door is closed first
        if (liftDoor != null && !liftDoor.IsFullyClosed())
        {
            Debug.Log("Waiting for doors to close...");
            return;
        }
        // Only move if we aren't already at the bottom
        if (Vector3.Distance(transform.position, bottomStop.position) > 0.1f)
        {
            StartCoroutine(StartMovingRoutine(bottomStop));
        }
    }

    /// <summary>
    /// Starts the movement routine with a delay.
    /// </summary>
    IEnumerator StartMovingRoutine(Transform target)
    {
        // Wait for the delay allows physics to "settle" the player on the platform
        yield return new WaitForSeconds(startDelay);

        // NOW start moving
        currentTarget = target;
        isMoving = true;
    }

    /// <summary>
    /// When the player enters the elevator, we parent them to the elevator so they move together. When they exit, we unparent them.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Rigidbody>() != null)
        {
            other.transform.SetParent(this.transform);
        }
    }

    /// <summary>
    /// When the player enters the elevator, we parent them to the elevator so they move together. When they exit, we unparent them.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Rigidbody>() != null)
        {
            other.transform.SetParent(null);
        }
    }
}