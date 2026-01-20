using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Linq;
using System;

public class SignupForm : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField displayNameField;
    [SerializeField] private TextMeshProUGUI errorText;

    public void Signup()
    {
        Debug.Log("Signup Started");
        // Obtain text from input fields
        var email = emailField.text;
        var password = passwordField.text;
        var displayName = displayNameField.text;

        // Input validation
        if (!email.Contains("@") || !email.Contains("."))
        {
            ShowError("Empty or invalid e-mail address");
            return;
        }
        // TODO: More validations
        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters long");
            return;
        }
        // If there are no integers in the password, show error
        else if (!password.Any(char.IsDigit))
        {
            ShowError("Password must contain at least one number");
            return;
        }
        if (displayName.Length < 3)
        {
            ShowError("Display name must be at least 3 characters long");
            return;
        }
        else
        {
            ShowError(""); // Clear error
        }
        // Create the Firebase Auth user first, then save profile in the DB
        ShowError("Signing up...");
        FirebaseAuth.DefaultInstance
            .CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(authTask =>
            {
                if (authTask.IsCanceled || authTask.IsFaulted)
                {
                    if (authTask.Exception != null) Debug.Log(authTask.Exception);
                    ShowError("Error creating account: " + authTask.Exception?.GetBaseException().Message);
                    return;
                }

                // Obtain the created user from the FirebaseAuth current user (authTask may be non-generic)
                var firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
                if (firebaseUser == null)
                {
                    Debug.LogError("Could not obtain FirebaseUser from CurrentUser after signup.");
                    ShowError("Signup failed: no authenticated user");
                    return;
                }

                Debug.Log("User created: " + firebaseUser.UserId);

                // Optionally update the Firebase Auth profile display name
                var profile = new Firebase.Auth.UserProfile { DisplayName = displayName };
                firebaseUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(updateProfileTask =>
                {
                    if (updateProfileTask.IsCanceled || updateProfileTask.IsFaulted)
                    {
                        if (updateProfileTask.Exception != null) Debug.Log(updateProfileTask.Exception);
                    }

                    // Now write the display name to Realtime Database under the authenticated user's id
                    FirebaseDatabase.DefaultInstance
                        .RootReference
                        .Child("Game")
                        .Child("Players")
                        .Child(firebaseUser.UserId)
                        .Child("Stats")
                        .Child("DisplayName")
                        .SetValueAsync(displayName)
                        .ContinueWithOnMainThread(dbTask =>
                        {
                            if (dbTask.IsCanceled || dbTask.IsFaulted)
                            {
                                if (dbTask.Exception != null) Debug.Log(dbTask.Exception);
                                ShowError("Error saving profile: " + dbTask.Exception?.GetBaseException().Message);
                                return;
                            }

                            ShowError(""); // clear error
                            Debug.Log("Signup completed and saved profile");
                        });
                });
            });
    }
    void ShowError(string error)
    {
        errorText.text = error;
    }

    // Add this helper so CurrentUserId() exists for the DB write
    private string CurrentUserId()
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null) return user.UserId;
        // Fallback id when no authenticated user exists (keeps DB key safe)
        return "anon_" + Guid.NewGuid().ToString("N");
    }
}
