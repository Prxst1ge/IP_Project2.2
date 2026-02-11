/*
 * Script Name: TotalTimePlayedTracker.cs
 * Date: 05/02/2026
 * Description: Tracks total playtime during the session and saves it to Firebase on application quit.
 */

using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class TotalTimePlayedTracker : MonoBehaviour
{
    public static TotalTimePlayedTracker Instance;

    [Header("Settings")]
    [Tooltip("Track time using unscaled delta time (ignores timeScale)")]
    public bool useUnscaledTime = true;

    [Tooltip("Max wait time (seconds) for Firebase initialization before giving up")]
    public float firebaseInitTimeoutSeconds = 10f;

    private double sessionSeconds = 0;
    private double initialTotalSeconds = 0;
    private bool hasLoadedInitialTotal = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private async void Start()
    {
        await WaitForFirebaseReady();
        await LoadInitialTotalTime();
    }

    private void Update()
    {
        float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        sessionSeconds += delta;
    }

    private async void OnApplicationQuit()
    {
        await SaveTotalTimePlayed();
    }

    private async Task WaitForFirebaseReady()
    {
        float elapsed = 0f;
        while (!Database.IsReady && elapsed < firebaseInitTimeoutSeconds)
        {
            await Task.Delay(100);
            elapsed += 0.1f;
        }

        if (!Database.IsReady)
        {
            Debug.LogWarning("Firebase is not ready. TotalTimePlayed may not be saved.");
        }
    }

    private async Task LoadInitialTotalTime()
    {
        try
        {
            if (!Database.IsReady)
            {
                return;
            }

            var auth = FirebaseAuth.DefaultInstance;
            if (auth.CurrentUser == null)
            {
                Debug.LogWarning("No authenticated user. TotalTimePlayed will not be tracked.");
                return;
            }

            string userId = auth.CurrentUser.UserId;
            var dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            string path = $"Game/Players/{userId}/Stats/TotalTimePlayed";

            var snapshot = await dbRef.Child(path).GetValueAsync();
            if (snapshot.Exists && snapshot.Value != null)
            {
                if (double.TryParse(snapshot.Value.ToString(), out var total))
                {
                    initialTotalSeconds = total;
                }
            }

            hasLoadedInitialTotal = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load TotalTimePlayed: {ex.Message}");
        }
    }

    private async Task SaveTotalTimePlayed()
    {
        try
        {
            if (!Database.IsReady)
            {
                return;
            }

            var auth = FirebaseAuth.DefaultInstance;
            if (auth.CurrentUser == null)
            {
                return;
            }

            if (!hasLoadedInitialTotal)
            {
                await LoadInitialTotalTime();
            }

            string userId = auth.CurrentUser.UserId;
            var dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            string path = $"Game/Players/{userId}/Stats/TotalTimePlayed";

            double totalSeconds = initialTotalSeconds + sessionSeconds;
            int totalSecondsRounded = Mathf.RoundToInt((float)totalSeconds);

            await dbRef.Child(path).SetValueAsync(totalSecondsRounded);
            Debug.Log($"TotalTimePlayed saved: {totalSecondsRounded}s");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save TotalTimePlayed: {ex.Message}");
        }
    }
}
