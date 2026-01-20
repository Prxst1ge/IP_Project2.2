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
        FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("Game")
            .Child("Players")
            .Child(CurrentUserId())
            .Child("Stats")
            .SetValueAsync(displayName)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null) Debug.Log(task.Exception);
                    ShowError("Error signing up");
                    return;
                }
                Debug.Log("Signup completed and saved stats");
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
