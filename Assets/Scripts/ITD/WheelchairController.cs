/*
 * Script Name: WheelchairController.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 02/02/2026
 * Description: this script manages the interactable behaviour of wheelchair wheels.
 */
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WheelchairController : MonoBehaviour
{

    public Rigidbody chairRigidbody; // The main Rigidbody of the wheelchair


    public float powerMultiplier = 15f; // Multiplier for the force applied based on wheel rotation speed

    public float maxSpeed = 5f;// Maximum speed of the wheelchair

    private XRGrabInteractable interactable; // XR Interaction component for grab detection
    private Transform currentInteractor; // The hand currently grabbing the wheel
    private bool isGrabbed = false; // Is the wheel currently grabbed
    private float previousHandAngle; // Previous angle of the hand around the wheel

    void Start()
    {
        interactable = GetComponent<XRGrabInteractable>();

        // Subscribe to grab events
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        currentInteractor = args.interactorObject.transform;

        // Calculate the starting angle of the hand relative to the wheel center
        previousHandAngle = CalculateHandAngle();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        currentInteractor = null;
    }

    void FixedUpdate()
    {
        if (isGrabbed && currentInteractor != null)
        {
            ApplyWheelForce();
        }
    }

    private void ApplyWheelForce()
    {
        //Calculate the new angle of the hand
        float currentHandAngle = CalculateHandAngle();

        // Calculate the difference (Delta)
        // Use DeltaAngle to handle wrap-around at 360 degrees
        float angleDifference = Mathf.DeltaAngle(previousHandAngle, currentHandAngle);

        // Filter small noise movements
        if (Mathf.Abs(angleDifference) > 0.5f)
        {
            // Calculate Force
            Vector3 forceDirection = transform.forward; // Or chairRigidbody.transform.forward

            // The magnitude of the force is proportional to the angle difference
            float forceMagnitude = -angleDifference * powerMultiplier;

            // Apply Force to the Main Chair (The Chassis)
            chairRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Force);

            // Rotate the visual wheel mesh to match hand
            transform.Rotate(Vector3.right, angleDifference);
        }

        // Update previous angle for the next frame
        previousHandAngle = currentHandAngle;
    }

    // Calculates the angle of the hand around the wheel's local X-axis
    private float CalculateHandAngle()
    {
        // Get hand position local to the wheel
        Vector3 localHandPos = transform.InverseTransformPoint(currentInteractor.position);

        // Calculate angle on the YZ plane (Standard side view for wheels rotated on X)
        // Atan2(y, z) gives us the angle in radians
        float angleRad = Mathf.Atan2(localHandPos.y, localHandPos.z);

        // Convert to degrees
        return angleRad * Mathf.Rad2Deg;
    }
}