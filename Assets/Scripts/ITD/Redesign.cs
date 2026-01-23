/*
 * Script Name: Repair.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 23/01/2026
 * Description: Handles repairing an object by swapping broken and fixed models.
 */
using UnityEngine;

public class Repair : MonoBehaviour
{
    // References to the broken and fixed objects
    public GameObject brokenObject;
    public GameObject fixedObject;

    // Allow repeated repairs if needed
    public bool canBeRepeated = false;
    // Track repair state
    private bool isRepaired = false;

    void Start()
    {
        // Ensure the correct starting state
        if (brokenObject != null) brokenObject.SetActive(true);
        if (fixedObject != null) fixedObject.SetActive(false);
    }

    // Method to perform the repair action
    public void PerformRepair()
    {
        // Prevent repairing if it's already done
        if (isRepaired && !canBeRepeated) return;

        // Swap the objects
        if (brokenObject != null) brokenObject.SetActive(false);
        if (fixedObject != null) fixedObject.SetActive(true);

        isRepaired = true;

        Debug.Log("Repair Complete!");
    }
}