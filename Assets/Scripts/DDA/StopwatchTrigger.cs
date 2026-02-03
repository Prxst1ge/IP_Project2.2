using UnityEngine;

public class StopwatchTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopWatch.Instance.CompleteScene();
        }
    }
}
