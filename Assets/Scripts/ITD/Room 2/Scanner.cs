/*
 * Script Name: ScannerTrigger.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Detects a 'Keycard' tag in a trigger zone to toggle door locks.
 */
using UnityEngine;
using UnityEngine.Events;

public class Scanner : MonoBehaviour
{
    // Tag that identifies valid keycards
    public string validTag = "Keycard";

    // Events to invoke on card detection
    public UnityEvent OnCardEnter;

    /// <summary>
    /// When another collider enters the trigger zone, check if it has the valid tag keycard. If it does, invoke the OnCardEnter event to trigger any actions (like unlocking a door) that are subscribed to this event. 
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Unlock when card enters
        if (other.CompareTag(validTag))
        {
            Debug.Log("Card Detected: Unlocking Door");
            OnCardEnter.Invoke();
        }
    }

}
