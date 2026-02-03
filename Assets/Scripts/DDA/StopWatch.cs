using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Threading.Tasks;

public class StopWatch : MonoBehaviour
{
    public static StopWatch Instance; // Singleton to persist across scenes
    
    public TextMeshProUGUI timerText; // Drag your UI text here
    private float elapsedTime;
    private bool isRunning = false;
    public string currentStageName; // Track which stage this timer is for
    
    [Header("Scene Settings")]
    [Tooltip("Is this the final scene of the stage? Timer will save when this scene completes.")]
    public bool isFinalScene = false;

    void Awake()
    {
        // Singleton pattern to persist timer across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // If there's already an instance and this is not it, destroy this
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Only start the timer if this is a new instance
        if (Instance == this && !isRunning)
        {
            StartTimer();
        }
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime; // Track time every frame
            if (timerText != null)
            {
                UpdateTimerDisplay();
            }
        }
    }
    
    /// <summary>
    /// Call this when the player reaches the end of a scene
    /// If it's the final scene, it will save the time to Firebase
    /// </summary>
    public async void CompleteScene()
    {
        if (isFinalScene)
        {
            await StopTimerAndSave();
        }
        // Otherwise, timer keeps running into the next scene
    }

    /// <summary>
    /// Initialize the timer for a specific stage
    /// </summary>
    public void InitializeForStage(string stageName)
    {
        currentStageName = stageName;
        elapsedTime = 0f;
    }

    public void StartTimer() => isRunning = true;

    public void StopTimer() => isRunning = false;


    public string GetCurrentStage() => currentStageName;
    
    /// <summary>
    /// Stop the timer and save the completion time to Firebase
    /// </summary>
    public async Task StopTimerAndSave()
    {
        StopTimer();
        if (!string.IsNullOrEmpty(currentStageName))
        {
            int completionTime = Mathf.FloorToInt(elapsedTime);
            await Database.SaveStageCompletionTime(currentStageName, completionTime);
        }
        else
        {
            Debug.LogWarning("Stage name not set. Call InitializeForStage() first.");
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        // Format as 00:00
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    /// <summary>
    /// Reset the timer for a new stage run
    /// </summary>
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
    }
    
    /// <summary>
    /// Update the timer text reference when changing scenes
    /// Call this in the new scene to reconnect the UI element
    /// </summary>
    public void UpdateTimerTextReference(TextMeshProUGUI newTimerText)
    {
        timerText = newTimerText;
    }

    public float GetFinalTime() => elapsedTime;
}