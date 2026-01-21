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

}
