using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabPoints : XRGrabInteractable
{
    protected override void Awake()
    {
        base.Awake();

        // Configure interactable defaults.
        movementType = MovementType.VelocityTracking;
        trackRotation = false;
        throwOnDetach = false;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // Destroy grab point object on selection end.
        Destroy(gameObject);
    }
}