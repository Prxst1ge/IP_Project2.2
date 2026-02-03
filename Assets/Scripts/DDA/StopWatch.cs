using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Threading.Tasks;

public class StopWatch : MonoBehaviour
{
    public static StopWatch Instance; // Singleton to persist across scenes
    
    public TextMeshProUGUI timerText; // Drag your UI text here
    private float elapsedTime;
    private bool isRunning = false;
    private string currentStageName; // Track which stage this timer is for
    private bool isFinalScene = false; // Track if this is the final scene of the stage

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
            // Scene loader will initialize stage name and final scene state
            // Timer will start once initialized
            elapsedTime = 0f;
            isRunning = false; // Will be started by InitializeForStage
            if (timerText != null)
            {
                UpdateTimerDisplay();
            }
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
    /// Initialize the timer for a specific stage and scene
    /// Call this from the Scene Loader when entering a scene
    /// If it's a new stage, resets the timer. If continuing the same stage, keeps accumulating time.
    /// </summary>
    public void InitializeForStage(string stageName, bool isFinal)
    {
        // Only reset timer if this is a NEW stage
        if (currentStageName != stageName)
        {
            elapsedTime = 0f;
            Debug.Log($"New stage '{stageName}' detected. Timer reset.");
        }
        else
        {
            Debug.Log($"Continuing stage '{stageName}'. Time carries over.");
        }

        currentStageName = stageName;
        isFinalScene = isFinal;
        StartTimer();
        Debug.Log($"Timer initialized for stage: {currentStageName} (Final scene: {isFinal})");
    }

    /// <summary>
    /// Call this when the player reaches the end of a scene
    /// If it's the final scene, it will save the time to Firebase and destroy the singleton
    /// </summary>
    public async void CompleteScene()
    {
        if (isFinalScene)
        {
            await StopTimerAndSave();
            // Destroy the singleton so it doesn't persist to the main hub
            Destroy(gameObject);
        }
        // Otherwise, timer keeps running into the next scene
    }

    public void StartTimer() => isRunning = true;

    public void StopTimer() => isRunning = false;


    public string GetCurrentStage() => currentStageName;
    
    /// <summary>
    /// Stop the timer and save the completion time to Firebase
    /// </summary>
    private async Task StopTimerAndSave()
    {
        StopTimer();
        if (!string.IsNullOrEmpty(currentStageName))
        {
            int completionTime = Mathf.FloorToInt(elapsedTime);
            await Database.SaveStageCompletionTime(currentStageName, completionTime);
            Debug.Log($"Stage '{currentStageName}' completed in {completionTime} seconds and saved to Firebase.");
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