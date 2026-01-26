using UnityEngine;
using System.Collections;

public class VRElevator : MonoBehaviour
{
    public Transform topStop;
    public Transform bottomStop;
    public float speed = 2.0f;
    public float startDelay = 0.5f;

    // Internal state
    private bool isMoving = false;
    private Transform currentTarget;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    // Update is called once per frame
    void FixedUpdate() // Using FixedUpdate for consistent movement
    {
        if (isMoving && currentTarget != null)
        {
            // Move the platform smoothly towards the target
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

            // Check if we have arrived (stop moving if we are close enough)
            if (Vector3.Distance(transform.position, currentTarget.position) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    // BUTTON FUNCTIONS 

    // For going UP
    public void GoUp()
    {
        // Only move if we aren't already at the top
        if (Vector3.Distance(transform.position, topStop.position) > 0.1f)
        {
            StartCoroutine(StartMovingRoutine(topStop));
        }
    }

    // For going DOWN
    public void GoDown()
    {
        // Only move if we aren't already at the bottom
        if (Vector3.Distance(transform.position, bottomStop.position) > 0.1f)
        {
            StartCoroutine(StartMovingRoutine(bottomStop));
        }
    }
    // --- THE DELAY LOGIC ---
    IEnumerator StartMovingRoutine(Transform target)
    {
        // 1. Wait for the delay (allows physics to "settle" the player on the platform)
        yield return new WaitForSeconds(startDelay);

        // 2. NOW start moving
        currentTarget = target;
        isMoving = true;
    }

    // PLAYER STICKY LOGIC 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Rigidbody>() != null)
        {
            other.transform.SetParent(this.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Rigidbody>() != null)
        {
            other.transform.SetParent(null);
            if (other.CompareTag("Player")) DontDestroyOnLoad(other.gameObject);
        }
    }
}