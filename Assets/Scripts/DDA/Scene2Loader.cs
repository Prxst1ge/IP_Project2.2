using UnityEngine;
using TMPro;

public class Scene2Loader : MonoBehaviour
{
    // Set these in the Inspector for each scene
    [SerializeField] private string stageName = "Stage2";
    [SerializeField] private bool isFinalScene = true;
    [SerializeField] private TextMeshProUGUI timerText; // Drag your timer UI text here

    void Start()
    {
        // Tell the StopWatch which stage this is and whether it's the final scene
        if (StopWatch.Instance != null)
        {
            StopWatch.Instance.InitializeForStage(stageName, isFinalScene);
            
            // Update the timer text reference for this scene
            if (timerText != null)
            {
                StopWatch.Instance.UpdateTimerTextReference(timerText);
                Debug.Log($"Timer text updated for scene: {gameObject.scene.name}");
            }
            else
            {
                Debug.LogWarning("Timer text field not assigned in Scene2Loader!");
            }
            
            Debug.Log($"StopWatch initialized for {stageName}, Final Scene: {isFinalScene}");
        }
        else
        {
            Debug.LogError("StopWatch singleton not found!");
        }
    }
}
