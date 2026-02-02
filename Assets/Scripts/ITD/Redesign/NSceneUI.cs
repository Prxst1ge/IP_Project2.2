/*
 * Script Name: NSceneUI.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 01/02/2026
 * Description: Controls the UI to transport to the new scene.
 */
using UnityEngine;

public class NSceneUI : MonoBehaviour
{
    public float distanceFromPlayer = 1.5f; // How far away the UI spawns (in meters)
    public bool lockYAxis = true; // Keeps the UI upright (so it doesn't tilt up/down)

    // This function runs automatically every time the object is turned ON
    void OnEnable()
    {
        PositionCanvas();
    }

    void PositionCanvas()
    {
        // Find the VR Headset (Camera)
        Camera playerCam = Camera.main;

        if (playerCam == null)
        {
            Debug.LogError("Could not find MainCamera! Tag your camera as 'MainCamera'.");
            return;
        }

        // 2. Calculate the position in front of the player
        Vector3 targetPosition = playerCam.transform.position + (playerCam.transform.forward * distanceFromPlayer);

        // 3. Optional: Adjust height so it's always at eye level, not looking at the floor
        if (lockYAxis)
        {
            // Keep the same height as the camera, or force a specific height
            // targetPosition.y = playerCam.transform.position.y; 
        }

        // 4. Apply Position
        transform.position = targetPosition;

        // 5. Rotate to face the player
        transform.LookAt(playerCam.transform);

        // 6. Flip it 180 degrees (UI LookAt usually makes it face backwards)
        transform.Rotate(0, 180, 0);

        // 7. Flatten the rotation if we want it straight up/down
        if (lockYAxis)
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, currentRotation.y, 0);
        }
    }
}

