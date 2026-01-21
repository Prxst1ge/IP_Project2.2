using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SignupForm : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField displayNameField;
    [SerializeField] private TextMeshProUGUI errorText;

    // Simplified signup entry point using async/await
    public async void Signup()
    {
        if (!Database.IsReady)
        {
            ShowError("Firebase not initialized yet. Please wait.");
            return;
        }

        var email = emailField.text;
        var password = passwordField.text;
        var displayName = displayNameField.text;

        var validationError = ValidateInputs(email, password, displayName);
        if (!string.IsNullOrEmpty(validationError))
        {
            ShowError(validationError);
            return;
        }

        ShowError("Signing up...");
        try
        {
            // Create user
            await FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);

            var firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
            if (firebaseUser == null)
                throw new Exception("Could not obtain authenticated user after signup.");

            Debug.Log("User created: " + firebaseUser.UserId);

            // Try updating profile (non-fatal)
            try
            {
                var profile = new Firebase.Auth.UserProfile { DisplayName = displayName };
                await firebaseUser.UpdateUserProfileAsync(profile);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Profile update failed: " + ex.Message);
            }

            // Save initial stats
            var stats = BuildInitialStats(displayName);
            var dbRef = FirebaseDatabase.DefaultInstance.RootReference
                .Child("Game").Child("Players").Child(firebaseUser.UserId).Child("Stats");

            await dbRef.SetValueAsync(stats);

            ShowError("");
            Debug.Log("Signup completed and saved profile");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ShowError("Signup failed: " + e.Message);
        }
    }

    // Simple validator that returns an error message or null/empty when valid
    private string ValidateInputs(string email, string password, string displayName)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
            return "Empty or invalid e-mail address";

        if (string.IsNullOrEmpty(password) || password.Length < 6)
            return "Password must be at least 6 characters long";

        if (!password.Any(char.IsDigit))
            return "Password must contain at least one number";

        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length < 3)
            return "Display name must be at least 3 characters long";

        return null;
    }

    // Builds the initial stats payload for the DB
    private Dictionary<string, object> BuildInitialStats(string displayName)
    {
        return new Dictionary<string, object>
        {
            { "DisplayName", displayName },
            { "AchievementsCollected", new Dictionary<string, object> { { "Speedrunner", false } } },
            { "StageCompletionTimings", new Dictionary<string, object>
                {
                    { "Stage1", 0 },
                    { "Stage2", 0 },
                    { "Stage3", 0 },
                    { "Stage4", 0 }
                }
            },
            { "TotalTimePlayed", 0 }
        };
    }

    void ShowError(string error)
    {
        errorText.text = error;
    }

    // Add this helper so CurrentUserId() exists for the DB write
    private string CurrentUserId()
    {
        if (!Database.IsReady) return "anon_" + Guid.NewGuid().ToString("N");
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null) return user.UserId;
        // Fallback id when no authenticated user exists (keeps DB key safe)
        return "anon_" + Guid.NewGuid().ToString("N");
    }
}
