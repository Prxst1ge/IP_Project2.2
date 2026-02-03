using UnityEngine;
using TMPro;

public class StopWatchUIConnector : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    /// <summary>
    /// On start, link the UI text to the StopWatch singleton and update the 
    /// reference UI element
    void Start()
    {
        if (StopWatch.Instance != null)
        {
            StopWatch.Instance.UpdateTimerTextReference(timerText);
        }
    }
}
