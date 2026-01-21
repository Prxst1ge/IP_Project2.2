using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WheelchairController : MonoBehaviour
{
    // References to the wheelchair components
    public XRGrabInteractable leftWheel;
    public XRGrabInteractable rightWheel;
    public Transform cameraTransform;

    //  Movement parameters

    public float moveSpeed = 5f;
    public float rotateSpeed = 100f;
    public float pushSensitivity = 2f;

    // 
    private Vector3 lastLeftHandPos;
    private Vector3 lastRightHandPos;

    void Update()
    {
        Vector3 movement = Vector3.zero;
        float rotation = 0f;

        //   Handle Left Wheel Interaction
        if (leftWheel.isSelected)
        {
            Vector3 currentpos = leftWheel.interactorsSelecting[0].transform.localPosition;
            if (leftWheel.isSelected && leftWheel.interactorsSelecting.Count > 0)
            {
                Vector3 currentPos = leftWheel.interactorsSelecting[0].transform.position;

                if (lastLeftHandPos != Vector3.zero)
                {
                    // Calculate movement in world space (forward push)
                    float forwardDelta = (currentPos - lastLeftHandPos).z;

                    // Apply forward movement if pushing in world forward direction
                    movement += Vector3.forward * forwardDelta * pushSensitivity;
                    rotation += forwardDelta * rotateSpeed;
                }
                lastLeftHandPos = currentPos;
            }
            lastLeftHandPos = currentpos;
        }
        else
        {
            lastLeftHandPos = Vector3.zero;
        }

        //   Handle Right Wheel Interaction
        if (rightWheel.isSelected && rightWheel.interactorsSelecting.Count > 0)
        {
            Vector3 currentPos = rightWheel.interactorsSelecting[0].transform.position;

            if (lastRightHandPos != Vector3.zero)
            {
                // Calculate movement in world space (forward push)
                float forwardDelta = (currentPos - lastRightHandPos).z;

                // Apply forward movement if pushing in world forward direction
                movement += Vector3.forward * forwardDelta * pushSensitivity;
                rotation -= forwardDelta * rotateSpeed;
            }
            lastRightHandPos = currentPos;
        }
        else
        {
            lastRightHandPos = Vector3.zero;
        }
        // Apply Movement
        transform.position += movement * moveSpeed * Time.deltaTime;
        transform.Rotate(0, rotation * Time.deltaTime, 0);

    }

}
