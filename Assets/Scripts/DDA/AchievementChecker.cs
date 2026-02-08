using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class AchievementChecker : MonoBehaviour
{
    public static AchievementChecker Instance;
    
    private DatabaseReference databaseReference;
    private FirebaseAuth firebaseAuth;

    // Dictionary to store all achievements
    private Dictionary<string, Achievement> achievements;

    void Awake()
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

    void Start()
    {
        // Initialize Firebase references
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        firebaseAuth = FirebaseAuth.DefaultInstance;
        
        // Register all achievements
        InitializeAchievements();
    }

    /// <summary>
    /// Initialize and register all achievements
    /// Add new achievements here
    /// </summary>
    private void InitializeAchievements()
    {
        achievements = new Dictionary<string, Achievement>();
        
        // SPEEDRUNNER: Complete all stages between 20 and 180 seconds each
        achievements.Add("Speedrunner", new Achievement
        {
            name = "Speedrunner",
            description = "Complete all 4 stages between 20 seconds to 3 minutes each",
            imagePath = "achievements/speedrunner.png",
            checkCondition = CheckSpeedrunnerCondition
        });
        
        // BEYOND COLOR: Complete Stage 1
        achievements.Add("BeyondColor", new Achievement
        {
            name = "BeyondColor",
            description = "Complete Stage 1",
            imagePath = "achievements/beyond_color.png",
            checkCondition = CheckBeyondColorCondition
        });
        
        // SHARP AT SHORT RANGE: Complete Stage 2
        achievements.Add("SharpAtShortRange", new Achievement
        {
            name = "SharpAtShortRange",
            description = "Complete Stage 2",
            imagePath = "achievements/sharp_at_short_range.png",
            checkCondition = CheckSharpAtShortRangeCondition
        });

        // SMOOTH OPERATOR: Complete Stage 3
        achievements.Add("SmoothOperator", new Achievement
        {
            name = "SmoothOperator",
            description = "Complete Stage 3",
            imagePath = "achievements/smooth_operator.png",
            checkCondition = CheckSmoothOperatorCondition
        });

        // ONE ARM WONDER: Complete Stage 4
        achievements.Add("OneArmWonder", new Achievement
        {
            name = "OneArmWonder",
            description = "Complete Stage 4",
            imagePath = "achievements/one_arm_wonder.png",
            checkCondition = CheckOneArmWonderCondition
        });

        // WALKED EVERY PATH: Complete all stages
        achievements.Add("WalkedEveryPath", new Achievement
        {
            name = "WalkedEveryPath",
            description = "Complete all stages",
            imagePath = "achievements/walked_every_path.png",
            checkCondition = CheckWalkedEveryPathCondition
        });
        
        // FIRST STEPS: Create a new account
        achievements.Add("FirstSteps", new Achievement
        {
            name = "FirstSteps",
            description = "Create a new account for the first time",
            imagePath = "achievements/first_steps.png",
            checkCondition = CheckFirstStepsCondition
        });
        
        // ADD MORE ACHIEVEMENTS HERE:
        // Example:
        // achievements.Add("PerfectScore", new Achievement
        // {
        //     name = "PerfectScore",
        //     description = "Complete all stages with 100% accuracy",
        //     checkCondition = CheckPerfectScoreCondition
        // });
    }

    /// <summary>
    /// Check a specific achievement by name
    /// </summary>
    public async void CheckAchievement(string achievementName)
    {
        if (!achievements.ContainsKey(achievementName))
        {
            Debug.LogWarning($"Achievement '{achievementName}' not found!");
            return;
        }
        
        string playerId = GetPlayerId();
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("Player ID not found!");
            return;
        }
        
        Achievement achievement = achievements[achievementName];
        Debug.Log($"Checking achievement: {achievement.name}");
        
        bool isUnlocked = await achievement.checkCondition(playerId);
        
        if (isUnlocked)
        {
            await UnlockAchievement(playerId, achievementName);
        }
        else
        {
            Debug.Log($"Achievement '{achievementName}' conditions not met");
        }
    }

    /// <summary>
    /// Check all registered achievements
    /// Skips FirstSteps since it's a one-time signup achievement
    /// </summary>
    public async void CheckAllAchievements()
    {
        string playerId = GetPlayerId();
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("Player ID not found!");
            return;
        }
        
        foreach (var achievement in achievements.Values)
        {
            // Skip FirstSteps - it's only checked on signup, not during normal gameplay
            if (achievement.name == "FirstSteps")
            {
                continue;
            }
            
            Debug.Log($"Checking achievement: {achievement.name}");
            bool isUnlocked = await achievement.checkCondition(playerId);
            
            if (isUnlocked)
            {
                await UnlockAchievement(playerId, achievement.name);
            }
        }
    }

    // ============= ACHIEVEMENT CONDITIONS =============
    // Add new achievement check methods here
    
    /// <summary>
    /// SPEEDRUNNER: Check if all stages completed between 20 and 180 seconds
    /// </summary>
    private async Task<bool> CheckSpeedrunnerCondition(string playerId)
    {
        const float SPEEDRUN_TIME_MIN = 20f;
        const float SPEEDRUN_TIME_MAX = 180f;
        
        try
        {
            for (int stage = 1; stage <= 4; stage++)
            {
                float stageTime = await GetStageCompletionTime(playerId, stage);
                Debug.Log($"Stage {stage} time: {stageTime}s (required: {SPEEDRUN_TIME_MIN}s - {SPEEDRUN_TIME_MAX}s)");
                
                if (stageTime <= SPEEDRUN_TIME_MIN || stageTime >= SPEEDRUN_TIME_MAX)
                {
                    Debug.Log($"Stage {stage} failed speedrun check: {stageTime}s not between {SPEEDRUN_TIME_MIN}s and {SPEEDRUN_TIME_MAX}s");
                    return false;
                }
            }
            
            Debug.Log("✓ All stages completed between 20 and 180 seconds!");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error checking Speedrunner: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// BEYOND COLOR: Check if Stage 1 is completed
    /// </summary>
    private async Task<bool> CheckBeyondColorCondition(string playerId)
    {
        try
        {
            float stage1Time = await GetStageCompletionTime(playerId, 1);
            Debug.Log($"Stage 1 completion time: {stage1Time}s");
            
            // Stage is completed if time is greater than 0
            if (stage1Time > 0 && stage1Time < float.MaxValue)
            {
                Debug.Log("✓ Stage 1 completed!");
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error checking Beyond Color: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// SHARP AT SHORT RANGE: Check if Stage 2 is completed
    /// </summary>
    private async Task<bool> CheckSharpAtShortRangeCondition(string playerId)
    {
        try
        {
            float stage2Time = await GetStageCompletionTime(playerId, 2);
            Debug.Log($"Stage 2 completion time: {stage2Time}s");
            
            // Stage is completed if time is greater than 0
            if (stage2Time > 0 && stage2Time < float.MaxValue)
            {
                Debug.Log("✓ Stage 2 completed!");
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error checking Sharp at Short Range: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// SMOOTH OPERATOR: Check if Stage 3 is completed
    /// </summary>
    private async Task<bool> CheckSmoothOperatorCondition(string playerId)
    {
        try
        {
            float stage3Time = await GetStageCompletionTime(playerId, 3);
            Debug.Log($"Stage 3 completion time: {stage3Time}s");

            // Stage is completed if time is greater than 0
            if (stage3Time > 0 && stage3Time < float.MaxValue)
            {
                Debug.Log("✓ Stage 3 completed!");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error checking Smooth Operator: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// ONE ARM WONDER: Check if Stage 4 is completed
    /// </summary>
    private async Task<bool> CheckOneArmWonderCondition(string playerId)
    {
        try
        {
            float stage4Time = await GetStageCompletionTime(playerId, 4);
            Debug.Log($"Stage 4 completion time: {stage4Time}s");

            // Stage is completed if time is greater than 0
            if (stage4Time > 0 && stage4Time < float.MaxValue)
            {
                Debug.Log("✓ Stage 4 completed!");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error checking One Arm Wonder: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// WALKED EVERY PATH: Check if all stages are completed
    /// </summary>
    private async Task<bool> CheckWalkedEveryPathCondition(string playerId)
    {
        try
        {
            for (int stage = 1; stage <= 4; stage++)
            {
                float stageTime = await GetStageCompletionTime(playerId, stage);
                Debug.Log($"Stage {stage} completion time: {stageTime}s");

                // Stage is completed if time is greater than 0
                if (stageTime <= 0 || stageTime >= float.MaxValue)
                {
                    return false;
                }
            }

            Debug.Log("✓ All stages completed!");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error checking Walked Every Path: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// FIRST STEPS: Unlocked when signup is successful
    /// This achievement is triggered directly from SignupForm after account creation
    /// Only unlocks once per player
    /// </summary>
    private async Task<bool> CheckFirstStepsCondition(string playerId)
    {
        // Check if FirstSteps has already been unlocked
        bool alreadyUnlocked = await IsAchievementUnlocked(playerId, "FirstSteps");
        
        if (alreadyUnlocked)
        {
            return false; // Already unlocked, don't unlock again
        }
        
        Debug.Log("✓ First Steps achievement unlocked - Welcome new player!");
        return true; // Not yet unlocked, so unlock it now
    }

    // ADD MORE ACHIEVEMENT CONDITIONS HERE:
    // Example:
    // private async Task<bool> CheckPerfectScoreCondition(string playerId)
    // {
    //     // Your logic here
    //     return true;
    // }

    // ============= HELPER METHODS =============
    
    /// <summary>
    /// Get the Firebase path to player's stats
    /// Path structure: Game/Players/{playerId}/Stats
    /// </summary>
    private DatabaseReference GetPlayerStatsReference(string playerId)
    {
        return databaseReference
            .Child("Game")
            .Child("Players")
            .Child(playerId)
            .Child("Stats");
    }
    
    /// <summary>
    /// Fetch stage completion time from Firebase
    /// </summary>
    private async Task<float> GetStageCompletionTime(string playerId, int stageNumber)
    {
        try
        {
            DataSnapshot snapshot = await GetPlayerStatsReference(playerId)
                .Child("StageCompletionTimings")
                .Child($"Stage{stageNumber}")
                .GetValueAsync();

            if (snapshot.Exists)
            {
                return float.Parse(snapshot.Value.ToString());
            }
            
            return float.MaxValue;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching Stage {stageNumber} time: {ex.Message}");
            return float.MaxValue;
        }
    }

    /// <summary>
    /// Unlock an achievement in Firebase
    /// Only shows UI notification and plays sound if this is a NEW unlock
    /// </summary>
    private async Task UnlockAchievement(string playerId, string achievementName)
    {
        try
        {
            // Check if already unlocked - sound effect only plays for new unlocks
            bool alreadyUnlocked = await IsAchievementUnlocked(playerId, achievementName);
            if (alreadyUnlocked)
            {
                Debug.Log($"Achievement '{achievementName}' already unlocked - skipping notification and sound");
                return;
            }
            
            Debug.Log($"🏆 Achievement UNLOCKED: {achievementName}");
            
            // Update Firebase: Game/Players/{playerId}/Stats/AchievementsCollected/{achievementName}
            await GetPlayerStatsReference(playerId)
                .Child("AchievementsCollected")
                .Child(achievementName)
                .SetValueAsync(true);
            
            Debug.Log($"Achievement '{achievementName}' saved to Firebase");
            
            // Show UI notification and play sound (only for NEW unlocks, not already-unlocked)
            if (achievements.ContainsKey(achievementName))
            {
                Achievement achievement = achievements[achievementName];
                AchievementNotificationUI.Instance?.ShowAchievement(achievement.name, achievement.description, achievement.imagePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error unlocking achievement: {ex.Message}");
        }
    }

    /// <summary>
    /// Check if achievement is already unlocked
    /// </summary>
    private async Task<bool> IsAchievementUnlocked(string playerId, string achievementName)
    {
        try
        {
            DataSnapshot snapshot = await GetPlayerStatsReference(playerId)
                .Child("AchievementsCollected")
                .Child(achievementName)
                .GetValueAsync();

            if (snapshot.Exists)
            {
                return bool.Parse(snapshot.Value.ToString());
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get current player ID
    /// </summary>
    private string GetPlayerId()
    {
        if (firebaseAuth != null && firebaseAuth.CurrentUser != null)
        {
            return firebaseAuth.CurrentUser.UserId;
        }
        
        Debug.LogWarning("No Firebase Auth user found");
        return null;
    }
}

/// <summary>
/// Achievement data structure
/// </summary>
public class Achievement
{
    public string name;
    public string description;
    public string imagePath; // Path/URL to achievement badge image in Firebase Storage
    public Func<string, Task<bool>> checkCondition; // Function that checks if achievement is unlocked
}

