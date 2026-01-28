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

    GameObject grabPoint;

    public Text label1;
    public Text label2;

    // Initializes references
    private void Start()
    {
        // Get references
        m_Rigidbody = GetComponent<Rigidbody>(); // Rigidbody of the wheel
        wheelRadius = GetComponent<SphereCollider>().radius; // Radius of the wheel collider

        // Slope check is run in coroutine at optimized intervals.
        StartCoroutine(CheckForSlope());
    }

    // When selection is made on this wheel object.
    protected override void OnSelectEntered(SelectEnterEventArgs eventArgs)
    {
        Debug.Log("Wheel selected");
        base.OnSelectEntered(eventArgs);

        XRBaseInteractor interactor = eventArgs.interactorObject as XRBaseInteractor;

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


    /// Generates a grab point to mediate physics interaction with the wheel's rigidbody. This "grab
    /// point" contains an XRGrabInteractable component, as well as a rigidbody. It's fused to the wheel using a Fixed Joint.
    /// <param name="interactor">Interactor which is making the selection.</param>
    void SpawnGrabPoint(XRBaseInteractor interactor)
    {
        // If there is an active grab point, cancel selection.
        if (grabPoint)
        {
            interactionManager.CancelInteractableSelection((IXRSelectInteractable)grabPoint.GetComponent<XRGrabInteractable>());
        }

        // Instantiate new grab point at interactor's position.
        grabPoint = new GameObject($"{transform.name}'s grabPoint", typeof(GrabPoints), typeof(FixedJoint));


        // Ensure the grab point is on the "Wheelchair" layer so it follows the Physics Matrix rules.
        grabPoint.layer = gameObject.layer;


        // Ensure the grab point uses the "Wheelchair" Interaction Layer.
        // Without this, your hand (which is looking for "Wheelchair") will reject the default grab point.
        grabPoint.GetComponent<XRGrabInteractable>().interactionLayers = interactionLayers;

        grabPoint.transform.position = interactor.transform.position;

        // Attach grab point to this wheel using fixed joint.
        grabPoint.GetComponent<FixedJoint>().connectedBody = GetComponent<Rigidbody>();

        // Force selection between current interactor and new grab point.
        interactionManager.SelectEnter((IXRSelectInteractor)interactor, (IXRSelectInteractable)grabPoint.GetComponent<XRGrabInteractable>());
    }

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

    IEnumerator MonitorDetachDistance(XRBaseInteractor interactor)
    {
        // While this wheel has an active grabPoint.
        while (grabPoint)
        {
            // If interactor drifts beyond the threshold distance from wheel, force deselection.
            if (Vector3.Distance(transform.position, interactor.transform.position) >= wheelRadius + deselectionThreshold)
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

        // NEW WAY (XRI 3.0+):
        // Instead of getting a "Controller" component, we check if this Interactor 
        // supports inputs (like haptics) by casting it to 'XRBaseInputInteractor'.
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
}