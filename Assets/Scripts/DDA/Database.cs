using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using System.Threading.Tasks;

public class Database : MonoBehaviour
{
    public static Database Instance;

    // Added: track Firebase init state and error
    public static bool IsReady { get; private set; }
    public static string InitError { get; private set; }

    [Header("Firebase Storage Settings")]
    [Tooltip("Your Firebase Storage bucket URL (e.g., gs://your-project.appspot.com)")]
    public string storageBucketUrl = "gs://your-firebase-project.appspot.com";

    [Header("Achievement Badge Paths")]
    [Tooltip("Folder path in Firebase Storage where achievement badges are stored")]
    public string achievementBadgesFolder = "achievments";

     private async void Start()
    {
        Debug.Log("Initializing Firebase...");
        try
        {
            await Database.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogError("Firebase init failed: " + Database.InitError);
        }
    }

    void Awake()
    {
        Debug.Log("DatabaseManager Awake called");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static Task InitializeAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                try
                {
                    // Accessing DefaultInstance will create the app if needed
                    var app = Firebase.FirebaseApp.DefaultInstance;
                    IsReady = true;
                    InitError = null;
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    IsReady = false;
                    InitError = ex.Message;
                    tcs.SetException(ex);
                }
            }
            else
            {
                IsReady = false;
                InitError = $"Could not resolve Firebase dependencies: {status}";
                tcs.SetException(new Exception(InitError));
            }
        });
        return tcs.Task;
    }

    /// <summary>
    /// Save stage completion timing to Firebase Realtime Database
    /// Path: /Game/Players/{userId}/Stats/StageCompletionTimings/{stageName}
    /// </summary>
    public static async Task SaveStageCompletionTime(string stageName, int completionTimeInSeconds)
    {
        try
        {
            if (!IsReady)
            {
                Debug.LogError("Firebase is not initialized yet.");
                return;
            }

            // Get the current user
            var auth = FirebaseAuth.DefaultInstance;
            if (auth.CurrentUser == null)
            {
                Debug.LogError("No user is currently authenticated.");
                return;
            }

            string userId = auth.CurrentUser.UserId;

            // Get the database reference
            var dbRef = FirebaseDatabase.DefaultInstance.RootReference;

            // Build the path: /Game/Players/{userId}/Stats/StageCompletionTimings/{stageName}
            string path = $"Game/Players/{userId}/Stats/StageCompletionTimings/{stageName}";

            // Set the value
            await dbRef.Child(path).SetValueAsync(completionTimeInSeconds);

            Debug.Log($"Successfully saved {stageName} completion time: {completionTimeInSeconds}s");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save stage completion time: {ex.Message}");
        }
    }

    /// <summary>
    /// Get full storage URL for an image path
    /// </summary>
    public string GetFullStorageUrl(string imagePath)
    {
        return $"{storageBucketUrl}/{imagePath}";
    }

    /// <summary>
    /// Validate if storage bucket URL is configured
    /// </summary>
    public bool IsStorageConfigured()
    {
        return !string.IsNullOrEmpty(storageBucketUrl) &&
               !storageBucketUrl.Contains("your-firebase-project");
    }

}
