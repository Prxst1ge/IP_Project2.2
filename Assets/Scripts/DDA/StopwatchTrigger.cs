using UnityEngine;

public class StopwatchTrigger : MonoBehaviour
{
    private bool hasCompleted = false;

    void Start()
    {
        Debug.Log("StopwatchTrigger started. Waiting for player...");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"*** TRIGGER HIT *** Object: {other.gameObject.name}, Tag: {other.tag}");
        AttemptTrigger(other);
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log($"*** TRIGGER STAY *** Object: {other.gameObject.name}");
        AttemptTrigger(other);
    }

    private void AttemptTrigger(Collider other)
    {
        if (hasCompleted)
        {
            Debug.Log("Already completed, ignoring.");
            return;
        }

        // Check Player tag
        if (other.CompareTag("Player"))
        {
            Debug.Log("✓ Player tag detected!");
            CompleteScene();
            return;
        }

        // Check if any parent contains "XR"
        Transform current = other.transform;
        for (int i = 0; i < 10; i++) // Check up to 10 levels
        {
            if (current == null) break;
            Debug.Log($"  └─ Parent {i}: {current.name}");
            if (current.name.Contains("XR"))
            {
                Debug.Log($"✓ Found XR parent: {current.name}");
                CompleteScene();
                return;
            }
            current = current.parent;
        }
    }

    private void CompleteScene()
    {
        hasCompleted = true;
        Debug.Log("🎯 TRIGGER ACTIVATED - Completing scene!");
        StopWatch.Instance.CompleteScene();
    }
}
