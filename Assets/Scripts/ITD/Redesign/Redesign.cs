/*
 * Script Name: Redesign.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 23/01/2026
 * Description: Handles redesigning an object by swapping broken and fixed models.
 */
using UnityEngine;
using System.Collections;

public class Redesign : MonoBehaviour
{
    // References to the broken and fixed objects
    public GameObject brokenObject;
    public GameObject fixedObject;

    // Allow repeated repairs if needed
    public bool canBeRepeated = false;
    // Track repair state
    public bool isRepaired = false;

    public bool explanationSeen = false; // Whether the explanation UI has been seen

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

        Debug.Log("ReDesign Complete!");
    }
}