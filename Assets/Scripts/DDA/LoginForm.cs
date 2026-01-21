using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LoginForm : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TextMeshProUGUI errorText;

    // Replaces Signup(): performs email/password sign-in
    public async void Login()
    {
        if (!Database.IsReady)
        {
            ShowError("Firebase not initialized yet. Please wait.");
            return;
        }

        var email = emailField.text;
        var password = passwordField.text;

        var validationError = ValidateInputs(email, password);
        if (!string.IsNullOrEmpty(validationError))
        {
            ShowError(validationError);
            return;
        }

        ShowError("Signing in...");
        try
        {
            // Sign in existing user
            await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);

            var firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
            if (firebaseUser == null)
                throw new Exception("Could not obtain authenticated user after sign-in.");

            Debug.Log("User signed in: " + firebaseUser.UserId);

            // Optionally read player stats after login (non-blocking)
            _ = LoadPlayerStatsAsync(firebaseUser.UserId);

            ShowError("");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ShowError("Login failed: " + e.Message);
        }
    }

    // Validate email/password
    private string ValidateInputs(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
            return "Empty or invalid e-mail address";

        if (string.IsNullOrEmpty(password) || password.Length < 6)
            return "Password must be at least 6 characters long";

        return null;
    }

    // Loads player stats from DB (non-blocking caller)
    private async Task LoadPlayerStatsAsync(string userId)
    {
        try
        {
            var dbRef = FirebaseDatabase.DefaultInstance.RootReference
                .Child("Game").Child("Players").Child(userId).Child("Stats");

            var snapshot = await dbRef.GetValueAsync();
            if (snapshot.Exists)
            {
                Debug.Log("Player stats loaded for " + userId);
                // ...existing code... // parse snapshot as needed by the game
            }
            else
            {
                Debug.Log("No stats found for user: " + userId);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Failed loading stats: " + ex.Message);
        }
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
