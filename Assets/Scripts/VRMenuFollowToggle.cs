using UnityEngine;
using UnityEngine.XR;

public class VRMenuFollowToggle : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject menuRoot; // VRMenu (Canvas)
    [SerializeField] private Transform head;      // Main Camera
    [SerializeField] private float distance = 1.2f;
    [SerializeField] private float heightOffset = -0.15f; // slightly below eye level

    [Header("Toggle Button (Right Hand)")]
    [SerializeField] private bool useRightHand = true;

    [Header("Anti-spam")]
    [SerializeField] private float toggleCooldown = 0.35f;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private float debugLogInterval = 1.0f;

    private bool isOpen;
    private float lastToggleTime;
    private float lastDebugTime;

    void Start()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);
    }

    void Update()
    {
        // Cooldown so it doesn't toggle repeatedly while held
        if (Time.time - lastToggleTime < toggleCooldown) return;

        InputDevice device = InputDevices.GetDeviceAtXRNode(useRightHand ? XRNode.RightHand : XRNode.LeftHand);

        if (enableDebugLogs && Time.time - lastDebugTime >= debugLogInterval)
        {
            string handLabel = useRightHand ? "RightHand" : "LeftHand";
            bool debugPressed = false;
            bool hasSecondary = device.isValid &&
                                device.TryGetFeatureValue(CommonUsages.secondaryButton, out debugPressed);
            Debug.Log($"[VRMenuFollowToggle] {handLabel} deviceValid={device.isValid}, " +
                      $"hasSecondaryButton={hasSecondary}, secondaryPressed={debugPressed}");
            lastDebugTime = Time.time;
        }

        // Good "menu-like" button choice: secondaryButton (B / Y)
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool pressed) && pressed)
        {
            ToggleMenu();
            lastToggleTime = Time.time;
        }
    }

    void LateUpdate()
    {
        if (!isOpen || menuRoot == null || head == null) return;

        // Keep menu in front of player's view, but level on the Y axis (reduces nausea)
        Vector3 forward = head.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            forward = head.forward;

        forward.Normalize();

        Vector3 targetPos = head.position + forward * distance;
        targetPos += Vector3.up * heightOffset;

        menuRoot.transform.position = targetPos;
        menuRoot.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    public void ToggleMenu()
    {
        if (menuRoot == null) return;

        isOpen = !isOpen;
        menuRoot.SetActive(isOpen);

        Debug.Log(isOpen ? "VR Menu OPEN" : "VR Menu CLOSED");
    }
}
