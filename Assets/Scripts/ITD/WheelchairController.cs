using UnityEngine;

public class WheelchairController : MonoBehaviour
{
    public Rigidbody mainBody;
    public Rigidbody leftWheel;
    public Rigidbody rightWheel;
    public float sensitivity = 15f;

    void FixedUpdate()
    {
        // 1. Get the local spin speed of each wheel
        // (We check the local Angular Velocity X because the joint rotates on X)
        float speedL = leftWheel.transform.InverseTransformDirection(leftWheel.angularVelocity).x;
        float speedR = rightWheel.transform.InverseTransformDirection(rightWheel.angularVelocity).x;

        // 2. Average them for forward speed
        float forwardSpeed = (speedL + speedR) / 2f;

        // 3. Subtract them for turning speed
        float turnSpeed = (speedL - speedR);

        // 4. Apply forces to the Main Body
        // We use transform.forward to move in the direction the chair is facing
        mainBody.AddForce(mainBody.transform.forward * forwardSpeed * sensitivity);
        mainBody.AddTorque(mainBody.transform.up * turnSpeed * sensitivity);
    }
}
