using UnityEngine;
using UnityEngine.XR;

public class VelocitySupplier : MonoBehaviour
{
    [SerializeField]
    XRNode trackedNode;

    Vector3 _velocity = Vector3.zero;

    /// <summary>
    /// Most recently tracked velocity of attached transform. Read only.;
    /// </summary>
    public Vector3 velocity { get => _velocity; }

    private void Start()
    {
        InputDevices.GetDeviceAtXRNode(trackedNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out _velocity);
    }

    void Update()
    {
        InputDevices.GetDeviceAtXRNode(trackedNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out _velocity);
    }
}