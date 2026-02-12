/*
 * Script Name: GrabPoints.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Controls the grab points for a VR wheelchair.
 */
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabPoints : XRGrabInteractable
{
    /// <summary
    /// Initializes the grab point interactable.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // Configure interactable defaults
        movementType = MovementType.VelocityTracking;
        trackRotation = false;
        throwOnDetach = false;
    }

    /// <summary
    /// Called when the grab point is deselected by an interactor.
    /// </summary>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // Destroy grab point object on selection end
        Destroy(gameObject);
    }
}