/*
 * Script Name: StageUnlockManager.cs
 * Date: 05/02/2026
 * Description: Checks Firebase for all stage completion timings and unlocks the next stage portals when timing > 0.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class StagePortalPair
{
    [FormerlySerializedAs("stageName")]
    [Tooltip("Stage key used in Firebase (e.g., 'Stage1', 'Stage2')")]
    public string stageKey;
    
    [FormerlySerializedAs("nextStagePortal")]
    [Tooltip("Portal GameObject that leads to the next stage")]
    public GameObject portalToNextStage;
    
    [HideInInspector]
    [FormerlySerializedAs("isUnlocked")]
    public bool unlocked = false;
}

public class StageUnlockManager : MonoBehaviour
{
    [Header("All Stage Portals")]
    [Tooltip("Configure all 4 stages and their corresponding portals")]
    [FormerlySerializedAs("stagePortals")]
    public List<StagePortalPair> stagePortalLinks = new List<StagePortalPair>()
    {
        new StagePortalPair { stageKey = "Stage1" },
        new StagePortalPair { stageKey = "Stage2" },
        new StagePortalPair { stageKey = "Stage3" },
        new StagePortalPair { stageKey = "Stage4" }
    };
    
    [Header("Settings")]
    [Tooltip("Check for completion on Start")]
    public bool checkOnStart = true;
    
    [Tooltip("Time in seconds between auto-checks (0 = no auto-check)")]
    public float autoCheckInterval = 0f;
    
    private float nextCheckTime = 0f;

    async void Start()
    {
        // Initially disable all portal prefabs
        foreach (var stagePortal in stagePortalLinks)
        {
            if (stagePortal.portalToNextStage != null)
            {
                stagePortal.portalToNextStage.SetActive(false);
            }
        }
        
        // Wait for Firebase to be ready
        await WaitForFirebase();
        
        // Check completion on start if enabled
        if (checkOnStart)
        {
            await CheckAllStagesAndUnlock();
        }
    }

    void Update()
    {
        // Auto-check if interval is set
        if (autoCheckInterval > 0 && Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + autoCheckInterval;
            _ = CheckAllStagesAndUnlock();
        }
    }

    /// <summary>
    /// Wait for Firebase to initialize before checking
    /// </summary>
    private async Task WaitForFirebase()
    {
        int maxWait = 100; // Maximum 10 seconds wait
        int waitCount = 0;
        
        while (!Database.IsReady && waitCount < maxWait)
        {
            await Task.Delay(100);
            waitCount++;
        }
        
        if (!Database.IsReady)
        {
            Debug.LogError("Firebase failed to initialize. Cannot check stage completion.");
        }
    }

    /// <summary>
    /// Check all 4 stages and unlock their respective portals if completed
    /// </summary>
    public async Task CheckAllStagesAndUnlock()
    {
        try
        {
            if (!Database.IsReady)
            {
                Debug.LogWarning("Firebase is not ready yet. Waiting...");
                await WaitForFirebase();
                
                if (!Database.IsReady)
                {
                    Debug.LogError("Firebase initialization failed. Cannot check stage completion.");
                    return;
                }
            }

            // Get the current user
            var auth = FirebaseAuth.DefaultInstance;
            if (auth.CurrentUser == null)
            {
                Debug.LogWarning("No user is currently authenticated. Portals remain locked.");
                return;
            }

            string userId = auth.CurrentUser.UserId;
            var dbRef = FirebaseDatabase.DefaultInstance.RootReference;

            Debug.Log("Checking completion status for all stages...");

            // Check each stage
            foreach (var stagePortal in stagePortalLinks)
            {
                if (string.IsNullOrEmpty(stagePortal.stageKey))
                    continue;

                // Build the path: /Game/Players/{userId}/Stats/StageCompletionTimings/{stageName}
                string path = $"Game/Players/{userId}/Stats/StageCompletionTimings/{stagePortal.stageKey}";

                // Get the completion time from Firebase
                var snapshot = await dbRef.Child(path).GetValueAsync();

                if (snapshot.Exists && snapshot.Value != null)
                {
                    // Try to parse the timing value
                    int completionTime = 0;
                    
                    if (int.TryParse(snapshot.Value.ToString(), out completionTime))
                    {
                        Debug.Log($"{stagePortal.stageKey} completion time: {completionTime} seconds");
                        
                        // Check if timing is more than 0 (stage completed)
                        if (completionTime > 0)
                        {
                            UnlockPortal(stagePortal);
                        }
                        else
                        {
                            Debug.Log($"{stagePortal.stageKey} not completed yet (timing = 0). Portal remains locked.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Could not parse completion time for {stagePortal.stageKey}. Value: {snapshot.Value}");
                    }
                }
                else
                {
                    Debug.Log($"{stagePortal.stageKey} has no completion data. Portal remains locked.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error checking stage completions: {ex.Message}");
        }
    }

    /// <summary>
    /// Main method to check stage completion timing from Firebase and unlock portal if timing > 0
    /// </summary>
    public async Task CheckStageCompletionAndUnlock()
    {
        // This method now calls CheckAllStagesAndUnlock for backwards compatibility
        await CheckAllStagesAndUnlock();
    }

    /// <summary>
    /// Unlocks a specific portal by enabling the portal prefab
    /// </summary>
    private void UnlockPortal(StagePortalPair stagePortal)
    {
        if (stagePortal.unlocked)
        {
            return;
        }

        stagePortal.unlocked = true;
        
        // Enable the portal prefab
        if (stagePortal.portalToNextStage != null)
        {
            stagePortal.portalToNextStage.SetActive(true);
            
            // Extract stage number for logging
            string stageNumber = stagePortal.stageKey.Replace("Stage", "");
            int nextStageNum = int.Parse(stageNumber) + 1;
            
            Debug.Log($"✓ {stagePortal.stageKey} completed! Portal to Stage {nextStageNum} enabled!");
        }
        else
        {
            Debug.LogWarning($"Next stage portal for {stagePortal.stageKey} is not assigned!");
        }
    }

    /// <summary>
    /// Sets the portal prefab active/inactive state
    /// </summary>
    private void SetPortalState(bool unlocked)
    {
        foreach (var stagePortal in stagePortalLinks)
        {
            if (stagePortal.portalToNextStage != null)
            {
                stagePortal.portalToNextStage.SetActive(unlocked);
            }
        }
    }

    /// <summary>
    /// Public method to manually trigger a completion check (can be called from other scripts)
    /// </summary>
    public void ManualCheckCompletion()
    {
        _ = CheckStageCompletionAndUnlock();
    }

    /// <summary>
    /// Check if the portal is currently unlocked
    /// </summary>
    public bool IsPortalUnlocked(string stageName)
    {
        var stagePortal = stagePortalLinks.Find(sp => sp.stageKey == stageName);
        return stagePortal != null && stagePortal.unlocked;
    }

    /// <summary>
    /// Get the unlock status of all stages
    /// </summary>
    public Dictionary<string, bool> GetAllUnlockStates()
    {
        var states = new Dictionary<string, bool>();
        foreach (var stagePortal in stagePortalLinks)
        {
            states[stagePortal.stageKey] = stagePortal.unlocked;
        }
        return states;
    }
}
