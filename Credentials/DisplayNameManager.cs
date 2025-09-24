// using System; // Add this for DateTime
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using Firebase.Auth;
// using Firebase.Firestore;
// using Firebase.Extensions;

// public class DisplayNameManager : MonoBehaviour
// {
//     [Header("UI")]
//     public InputField displayNameInput;
//     public GameObject displayNamePanel;
//     public GameObject mainMenuPanel;

//     public void SubmitDisplayName()
//     {
//         string displayName = displayNameInput.text;
//         if (string.IsNullOrEmpty(displayName))
//         {
//             Debug.LogError("Username cannot be empty.");
//             return; // Optionally show UI error message
//         }

//         Debug.Log($"Submitting Username: {displayName}");

//         FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
//         if (user == null)
//         {
//             Debug.LogError("No authenticated user found.");
//             return;
//         }

//         StartCoroutine(UpdateFirestoreWithDisplayName(user, displayName));
//     }

//     private IEnumerator UpdateFirestoreWithDisplayName(FirebaseUser user, string displayName)
//     {
//         FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
//         DocumentReference docRef = db.Collection("users").Document(user.Email);

//         Dictionary<string, object> userData = new Dictionary<string, object>
//         {
//             { "name", user.DisplayName ?? "" }, // Google's Username (handle null)
//             { "email", user.Email },
//             { "uid", user.UserId },
//             { "displayName", displayName },
//             { "loginTime", FieldValue.ServerTimestamp },
//             { "lastActive", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
//         };

//         var writeTask = docRef.SetAsync(userData);
//         yield return new WaitUntil(() => writeTask.IsCompleted);

//         if (writeTask.IsFaulted)
//         {
//             Debug.LogError($"Firestore write failed: {writeTask.Exception}");
//         }
//         else
//         {
//             Debug.Log("User data (including Username) stored in Firestore successfully!");
//             if (displayNamePanel != null)
//             {
//                 displayNamePanel.SetActive(false);
//             }
//             if (mainMenuPanel != null)
//             {
//                 mainMenuPanel.SetActive(true);
//             }
//         }
//     }
// }







using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for InputField and Text
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro; // Use TextMeshPro for better UI text

public class DisplayNameManager : MonoBehaviour
{
    [Header("UI")]
    // public TMP_InputField displayNameInput;
    public InputField displayNameInput;
    public Text feedbackText; // Add a Text element to show feedback
    public Button submitButton; // Reference to the submit button
    public GameObject displayNamePanel;
    public GameObject mainMenuPanel;

    private void Start()
    {
        // Clear feedback text on start and ensure button is disabled if input is empty
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
        if (displayNameInput != null && submitButton != null)
        {
            submitButton.interactable = !string.IsNullOrEmpty(displayNameInput.text);
            // Optional: Add a listener to enable/disable button as user types
            displayNameInput.onValueChanged.AddListener(OnInputChanged);
        }
    }

    private void OnInputChanged(string input)
    {
        // When user types, clear old feedback and enable the submit button
        if (feedbackText != null) feedbackText.text = "";
        if (submitButton != null) submitButton.interactable = !string.IsNullOrEmpty(input);
    }


    public void SubmitDisplayName()
    {
        string displayName = displayNameInput.text.Trim(); // Trim whitespace
        if (string.IsNullOrEmpty(displayName))
        {
            if (feedbackText != null) feedbackText.text = "Username cannot be empty.";
            Debug.LogError("Username cannot be empty.");
            return;
        }

        // Basic validation for name length or characters (optional)
        if (displayName.Length < 3)
        {
             if (feedbackText != null) feedbackText.text = "Name must be at least 3 characters.";
             return;
        }


        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("No authenticated user found.");
            if (feedbackText != null) feedbackText.text = "Error: Not signed in.";
            return;
        }

        // Start the process: Check for uniqueness, then submit if unique.
        StartCoroutine(CheckAndSubmitDisplayName(user, displayName));
    }

    private IEnumerator CheckAndSubmitDisplayName(FirebaseUser user, string displayName)
    {
        // --- 1. UI Feedback: Disable button and show "Checking..." ---
        if (submitButton != null) submitButton.interactable = false;
        if (feedbackText != null)
        {
            feedbackText.text = "Checking availability...";
            feedbackText.color = Color.white;
        }


        // --- 2. Check for Uniqueness ---
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        CollectionReference usersRef = db.Collection("users");
        // Create a query to find documents where 'displayName' matches the input
        Query query = usersRef.WhereEqualTo("displayName", displayName);

        var queryTask = query.GetSnapshotAsync();
        yield return new WaitUntil(() => queryTask.IsCompleted);

        if (queryTask.IsFaulted)
        {
            Debug.LogError($"Error checking username: {queryTask.Exception}");
            if (feedbackText != null) feedbackText.text = "Error checking name.";
            if (submitButton != null) submitButton.interactable = true; // Re-enable button on error
            yield break; // Stop the coroutine
        }

        QuerySnapshot snapshot = queryTask.Result;
        if (snapshot.Count > 0)
        {
            // Username is TAKEN
            Debug.LogWarning("Username already exists.");
            if (feedbackText != null)
            {
                feedbackText.text = "This name is already taken.";
                feedbackText.color = Color.red;
            }
            if (submitButton != null) submitButton.interactable = true; // Re-enable button
            yield break; // Stop the coroutine
        }

        // --- 3. If Unique, Proceed to Write to Firestore ---
        if (feedbackText != null) feedbackText.text = "Name is available! Saving...";

        // DocumentReference docRef = db.Collection("users").Document(user.Email);
        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "name", user.DisplayName ?? "" },
            { "email", user.Email },
            { "uid", user.UserId },
            { "displayName", displayName }, // The unique name the user chose
            { "loginTime", FieldValue.ServerTimestamp },
            { "lastActive", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        var writeTask = docRef.SetAsync(userData, SetOptions.MergeAll); // Use MergeAll to avoid overwriting other fields
        yield return new WaitUntil(() => writeTask.IsCompleted);

        if (writeTask.IsFaulted)
        {
            Debug.LogError($"Firestore write failed: {writeTask.Exception}");
            if (feedbackText != null) feedbackText.text = "Could not save profile.";
            if (submitButton != null) submitButton.interactable = true; // Re-enable on error
        }
        else
        {
            Debug.Log("User data (including Username) stored in Firestore successfully!");
            // Switch panels on success
            if (displayNamePanel != null)
            {
                displayNamePanel.SetActive(false);
            }
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
            }
        }
    }
}
