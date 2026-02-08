/*
 * Script Name: AutomaticDoor.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 02/02/2026
 * Description: this script manages the automatic opening and closing of doors.
 */
using UnityEngine;
using System.Collections;

public class AutomaticDoor : MonoBehaviour
{
    public Transform leftDoor; // The left door transform
    public Transform rightDoor; // The right door transform


    public Transform leftOpenAnchor; // The left door open position
    public Transform leftClosedAnchor; // The left door closed position
    public Transform rightOpenAnchor; // The right door open position
    public Transform rightClosedAnchor; // The right door closed position


    public float speed = 3.0f; // Speed of door movement
    public float closeDelay = 2.0f; // Time in seconds before closing
    public string detectionTag = "Player"; // Object tag that triggers the door

    // Internal state
    private bool isOpen = false;
    private Coroutine closeCoroutine;

    void Update()
    {
        // Determine where the doors should be right now
        Vector3 targetLeft = isOpen ? leftOpenAnchor.localPosition : leftClosedAnchor.localPosition;
        Vector3 targetRight = isOpen ? rightOpenAnchor.localPosition : rightClosedAnchor.localPosition;

        // Move the doors smoothly towards that target
        leftDoor.localPosition = Vector3.MoveTowards(leftDoor.localPosition, targetLeft, speed * Time.deltaTime);
        rightDoor.localPosition = Vector3.MoveTowards(rightDoor.localPosition, targetRight, speed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(detectionTag))
        {
            isOpen = true;
            // If a close timer is running, stop it
            if (closeCoroutine != null)
            {
                StopCoroutine(closeCoroutine);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(detectionTag))
        {
            // Start the timer to close the door
            closeCoroutine = StartCoroutine(AutoCloseTimer());
        }
    }

    // The timer logic
    IEnumerator AutoCloseTimer()
    {
        yield return new WaitForSeconds(closeDelay);
        isOpen = false;
    }
}