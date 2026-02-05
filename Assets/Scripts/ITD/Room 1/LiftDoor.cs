/*
 * Script Name: LiftDoor.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Controls the movement of a VR elevator doors.
 */
using UnityEngine;
using System.Collections;

public class LiftDoor : MonoBehaviour
{
    // References to the door transforms
    public Transform leftDoor;
    public Transform rightDoor;

    // Anchors for open and closed positions
    public Transform leftOpenAnchor;
    public Transform leftClosedAnchor;
    public Transform rightOpenAnchor;
    public Transform rightClosedAnchor;

    // Door movement parameters
    public float speed = 2.0f;
    public float openDuration = 3.0f;

    // Door state
    private bool isOpen = false;


    void Update()
    {
        // Target the position of our Empty GameObjects
        Vector3 targetL = isOpen ? leftOpenAnchor.position : leftClosedAnchor.position;
        Vector3 targetR = isOpen ? rightOpenAnchor.position : rightClosedAnchor.position;

        // Move toward those world positions
        leftDoor.position = Vector3.MoveTowards(leftDoor.position, targetL, speed * Time.deltaTime);
        rightDoor.position = Vector3.MoveTowards(rightDoor.position, targetR, speed * Time.deltaTime);
    }

    // Function to check if the door is fully closed
    public bool IsFullyClosed()
    {
        // Returns true only if the door is NOT set to open AND both doors are at their closed positions
        float distL = Vector3.Distance(leftDoor.localPosition, leftClosedAnchor.localPosition);
        float distR = Vector3.Distance(rightDoor.localPosition, rightClosedAnchor.localPosition);

        return !isOpen && distL < 0.01f && distR < 0.01f;
    }

    // Function to open the door
    public void OpenDoor()
    {
        if (!isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(DoorTimer());
        }
    }

    // Coroutine to handle door open duration
    IEnumerator DoorTimer()
    {
        isOpen = true;
        yield return new WaitForSeconds(openDuration);
        isOpen = false;
    }

    void OnMouseDown() => OpenDoor();
}