using System; // Add this for DateTime
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class DisplayNameManager : MonoBehaviour
{
    [Header("UI")]
    public InputField displayNameInput;
    public GameObject displayNamePanel;
    public GameObject mainMenuPanel;

    public void SubmitDisplayName()
    {
        string displayName = displayNameInput.text;
        if (string.IsNullOrEmpty(displayName))
        {
            Debug.LogError("Display name cannot be empty.");
            return; // Optionally show UI error message
        }

        Debug.Log($"Submitting display name: {displayName}");

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("No authenticated user found.");
            return;
        }

        StartCoroutine(UpdateFirestoreWithDisplayName(user, displayName));
    }

    private IEnumerator UpdateFirestoreWithDisplayName(FirebaseUser user, string displayName)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("users").Document(user.Email);

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "name", user.DisplayName ?? "" }, // Google's display name (handle null)
            { "email", user.Email },
            { "uid", user.UserId },
            { "displayName", displayName },
            { "loginTime", FieldValue.ServerTimestamp },
            { "lastActive", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        var writeTask = docRef.SetAsync(userData);
        yield return new WaitUntil(() => writeTask.IsCompleted);

        if (writeTask.IsFaulted)
        {
            Debug.LogError($"Firestore write failed: {writeTask.Exception}");
        }
        else
        {
            Debug.Log("User data (including display name) stored in Firestore successfully!");
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