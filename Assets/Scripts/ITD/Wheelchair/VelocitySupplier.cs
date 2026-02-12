/*
 * Script Name: VelocitySupplier.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Supplies velocity data for tracked XR devices.
 */
using UnityEngine;
using UnityEngine.XR;

public class VelocitySupplier : MonoBehaviour
{
    [SerializeField]
    XRNode trackedNode;

    Vector3 _velocity = Vector3.zero;

    /// <summary>
    /// Most recently tracked velocity of attached transform. 
    /// </summary>
    public Vector3 velocity { get => _velocity; }

    private void Start()
    {
        InputDevices.GetDeviceAtXRNode(trackedNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out _velocity); // Initialize velocity
    }

    /// <summary
    /// Updates the velocity of the tracked XR device each frame.
    /// </summary>
    void Update()
    {
        InputDevices.GetDeviceAtXRNode(trackedNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out _velocity); // Update velocity each frame
    }
}