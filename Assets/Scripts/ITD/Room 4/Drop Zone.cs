/*
 * Script Name: DropZone.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 11/02/2026
 * Description: Triggers when the keycard or mug is dropped onto the ground.
 */
using UnityEngine;

public class DropZone : MonoBehaviour
{
    public Transform mugTarget;      // Where the mug will spawn back to
    public Transform keycardTarget;  // Where the keycard will spawn back to

    /// <summary>
    /// Called when the scene starts to initialize the drop zone.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Check if it is the Coffee Mug
        if (other.CompareTag("CoffeeMug"))
        {
            // Reset Physics (Stop it from flying)
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;

            // Teleport to the Empty's position
            other.transform.position = mugTarget.position;
            other.transform.rotation = mugTarget.rotation;
        }

        // Check if it is the Keycard
        else if (other.CompareTag("Keycard"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;

            other.transform.position = keycardTarget.position;
            other.transform.rotation = keycardTarget.rotation;
        }
    }
}
