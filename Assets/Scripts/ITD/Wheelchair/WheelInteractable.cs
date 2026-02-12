/*
 * Script Name: WheelInteractable.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: this script manages the interactable behaviour of wheelchair wheels.
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;



public class WheelInteractable : XRBaseInteractable
{
    Rigidbody m_Rigidbody;

    float wheelRadius;

    bool onSlope = false;
    [SerializeField] bool hapticsEnabled = true;

    [Range(0, 0.5f), Tooltip("Distance from wheel collider at which the interaction manager will cancel selection.")]
    [SerializeField] float deselectionThreshold = 0.25f;
    SphereCollider m_SphereCollider; // Sphere collider of the wheel

    GameObject grabPoint;

    public Text label1;
    public Text label2;

    // Initializes references
    private void Start()
    {
        // Get references
        m_Rigidbody = GetComponent<Rigidbody>(); // Rigidbody of the wheel
        m_SphereCollider = GetComponent<SphereCollider>(); // Sphere collider of the wheel
        wheelRadius = m_SphereCollider.radius; // Radius of the wheel collider

        // Slope check is run in coroutine at optimized intervals.
        StartCoroutine(CheckForSlope());
    }

    // When selection is made on this wheel object.
    protected override void OnSelectEntered(SelectEnterEventArgs eventArgs)
    {
        Debug.Log("Wheel selected");
        base.OnSelectEntered(eventArgs);

        XRBaseInteractor interactor = eventArgs.interactorObject as XRBaseInteractor; // Get interactor from event args

        // Forcibly cancel selection with this wheel object.
        interactionManager.CancelInteractableSelection((IXRSelectInteractable)this);

        SpawnGrabPoint(interactor);

        StartCoroutine(BrakeAssist(interactor));
        StartCoroutine(MonitorDetachDistance(interactor));

        if (hapticsEnabled)
        {
            StartCoroutine(SendHapticFeedback(interactor));
        }
    }


    // Spawns a grab point at the interactor's position and attaches it to this wheel.
    void SpawnGrabPoint(XRBaseInteractor interactor)
    {
        // If there is an active grab point, cancel selection.
        if (grabPoint)
        {
            interactionManager.CancelInteractableSelection((IXRSelectInteractable)grabPoint.GetComponent<XRGrabInteractable>());
        }

        // Instantiate new grab point at interactor's position.
        grabPoint = new GameObject($"{transform.name}'s grabPoint", typeof(GrabPoints), typeof(FixedJoint));


        // Layer the grab point to the same layer as this wheel.
        grabPoint.layer = gameObject.layer;


        // Configure grab point's XRGrabInteractable component.
        grabPoint.GetComponent<XRGrabInteractable>().interactionLayers = interactionLayers;

        grabPoint.transform.position = interactor.transform.position; // Position grab point at interactor's position.

        // Attach grab point to this wheel using fixed joint.
        grabPoint.GetComponent<FixedJoint>().connectedBody = GetComponent<Rigidbody>();

        // Force selection between current interactor and new grab point.
        interactionManager.SelectEnter((IXRSelectInteractor)interactor, (IXRSelectInteractable)grabPoint.GetComponent<XRGrabInteractable>());
    }

    // Assists braking by applying counter-torque when the interactor's velocity is near zero.
    IEnumerator BrakeAssist(XRBaseInteractor interactor)
    {
        VelocitySupplier interactorVelocity = interactor.GetComponent<VelocitySupplier>();

        while (grabPoint)
        {
            // If the interactor's forward/backward movement approximates zero, it is considered to be braking.
            if (interactorVelocity.velocity.z < 0.05f && interactorVelocity.velocity.z > -0.05f)
            {
                m_Rigidbody.AddTorque(-m_Rigidbody.angularVelocity.normalized * 25f);

                SpawnGrabPoint(interactor);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    // Monitors the distance between the interactor and the wheel's collider to auto-deselect if too far.
    IEnumerator MonitorDetachDistance(XRBaseInteractor interactor)
    {
        while (grabPoint)
        {
            // Calculate the ACTUAL center of the collider in world space
            Vector3 colliderWorldCenter = transform.TransformPoint(m_SphereCollider.center);

            // Measure distance from the Green Sphere Center, not the Pivot
            float distance = Vector3.Distance(colliderWorldCenter, interactor.transform.position);

            // Check if hand is too far from the collider surface
            if (distance >= wheelRadius + deselectionThreshold)
            {
                interactionManager.CancelInteractorSelection((IXRSelectInteractor)interactor);
            }

            yield return null;
        }
    }

    IEnumerator SendHapticFeedback(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor)
    {
        // Interval between iterations of coroutine, in seconds.
        float runInterval = 0.1f;

        // Cast interactor to input interactor
        var inputInteractor = interactor as XRBaseInputInteractor;

        // If this interactor doesn't support inputs (e.g. it's a Gaze Interactor), stop here.
        if (inputInteractor == null) yield break;

        Vector3 lastAngularVelocity = new Vector3(transform.InverseTransformDirection(m_Rigidbody.angularVelocity).x, 0f, 0f);

        while (grabPoint)
        {
            Vector3 currentAngularVelocity = new Vector3(transform.InverseTransformDirection(m_Rigidbody.angularVelocity).x, 0f, 0f);
            Vector3 angularAcceleration = (currentAngularVelocity - lastAngularVelocity) / runInterval;

            // If current velocity and acceleration have perpendicular or opposite directions, the wheel is decelerating.
            if (Vector3.Dot(currentAngularVelocity.normalized, angularAcceleration.normalized) < 0f)
            {
                float impulseAmplitude = Mathf.Abs(angularAcceleration.x);

                if (impulseAmplitude > 1.5f)
                {
                    float remappedImpulseAmplitude = Remap(impulseAmplitude, 1.5f, 40f, 0f, 1f);

                    // COMMAND HAPTICS DIRECTLY ON THE INTERACTOR
                    inputInteractor.SendHapticImpulse(remappedImpulseAmplitude, runInterval * 2f);
                }
            }

            lastAngularVelocity = currentAngularVelocity;
            yield return new WaitForSeconds(runInterval);
        }
    }


    /// This is a utility method which remaps a float value from one range to another.

    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;

        //float normal = Mathf.InverseLerp(aLow, aHigh, value);
        //float bValue = Mathf.Lerp(bLow, bHigh, normal);
    }

    IEnumerator CheckForSlope()
    {
        while (true)
        {
            if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit))
            {
                onSlope = hit.normal != Vector3.up;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    // Check whether the player hand is touching the wheel
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<XRDirectInteractor>() != null)
        {
            Debug.Log("Player Hand touched the wheel!");

        }
    }

    // Called automatically when the collider leaves
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<XRDirectInteractor>() != null)
        {
            Debug.Log("Player Hand left the wheel.");

        }
    }
}

