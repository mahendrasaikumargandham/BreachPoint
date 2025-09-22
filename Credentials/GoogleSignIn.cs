// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Net;
// using System.Text;
// using System.Linq;
// using System.Threading;
// using UnityEngine;
// using UnityEngine.Networking;

// [System.Serializable]
// public class TokenResponse
// {
//     public string access_token;
//     public int expires_in;
//     public string token_type;
//     public string scope;
//     public string refresh_token;
// }

// public class GoogleSignIn : MonoBehaviour
// {
//     [Header("Google OAuth Settings")]
//     public string clientId = "473018706625-3e24bn66jk9id5n0ck06qjggj9dl0mhd.apps.googleusercontent.com"; // Replace with your Client ID
//     public string clientSecret = "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"; // Replace with your Client Secret (use PKCE for better security in production)
//     public string redirectUri = "http://localhost:8080"; // Must match your Google Cloud Console redirect URI

//     [Header("UI")]
//     public GameObject gameScreen; // The GameObject to enable after successful sign-in

//     private string authCode;
//     private bool callbackReceived = false;
//     private readonly object lockObject = new object();

//     public void OnSignInButtonClick()
//     {
//         StartCoroutine(SignInCoroutine());
//     }

//     private IEnumerator SignInCoroutine()
//     {
//         string authUrl = BuildAuthUrl();
//         Application.OpenURL(authUrl);

//         StartThreadForListener();

//         // Wait for callback with timeout
//         float timeout = 120f; // 2 minutes
//         while (!callbackReceived && timeout > 0f)
//         {
//             timeout -= Time.deltaTime;
//             yield return null;
//         }

//         if (callbackReceived && !string.IsNullOrEmpty(authCode))
//         {
//             yield return StartCoroutine(ExchangeCodeForToken(authCode));
//             if (gameScreen != null)
//             {
//                 gameScreen.SetActive(true);
//             }
//             Debug.Log("Sign-in successful! Game screen enabled.");
//         }
//         else
//         {
//             Debug.LogError("Sign-in failed or timed out.");
//         }
//     }

//     private string BuildAuthUrl()
//     {
//         string baseAuthUri = "https://accounts.google.com/o/oauth2/v2/auth";
//         var authUriQueryParams = new Dictionary<string, string>
//         {
//             ["client_id"] = clientId,
//             ["redirect_uri"] = redirectUri,
//             ["response_type"] = "code",
//             ["scope"] = "openid email profile", // Adjust scopes as needed for your multiplayer game (e.g., add more for specific APIs)
//             ["access_type"] = "offline", // Requests a refresh token
//             ["prompt"] = "select_account" // Forces account selection to show active logged-in accounts
//         };

//         var queryString = string.Join("&", authUriQueryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
//         return $"{baseAuthUri}?{queryString}";
//     }

//     private void StartThreadForListener()
//     {
//         authCode = null;
//         callbackReceived = false;
//         var thread = new Thread(ListenForCallback);
//         thread.IsBackground = true;
//         thread.Start();
//     }

//     private void ListenForCallback()
//     {
//         using (var listener = new HttpListener())
//         {
//             string prefix = redirectUri.EndsWith("/") ? redirectUri : redirectUri + "/";
//             listener.Prefixes.Add(prefix);
//             listener.Start();

//             try
//             {
//                 var context = listener.GetContext();
//                 string code = context.Request.QueryString["code"];

//                 lock (lockObject)
//                 {
//                     authCode = code;
//                     callbackReceived = true;
//                 }

//                 // Send success response to browser
//                 string responseString = @"
//                     <html>
//                         <body>
//                             <h1>Login Successful!</h1>
//                             <p>You can close this tab and return to the game.</p>
//                             <script>
//                                 window.close();
//                             </script>
//                         </body>
//                     </html>";
//                 byte[] buffer = Encoding.UTF8.GetBytes(responseString);
//                 context.Response.ContentLength64 = buffer.Length;
//                 context.Response.ContentType = "text/html";
//                 context.Response.OutputStream.Write(buffer, 0, buffer.Length);
//                 context.Response.OutputStream.Close();
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError($"Listener error: {e.Message}");
//                 lock (lockObject)
//                 {
//                     callbackReceived = true; // To unblock the wait
//                 }
//             }
//             finally
//             {
//                 listener.Stop();
//             }
//         }
//     }

//     private IEnumerator ExchangeCodeForToken(string code)
//     {
//         string tokenUrl = "https://oauth2.googleapis.com/token";

//         WWWForm form = new WWWForm();
//         form.AddField("code", code);
//         form.AddField("client_id", clientId);
//         form.AddField("client_secret", clientSecret);
//         form.AddField("redirect_uri", redirectUri);
//         form.AddField("grant_type", "authorization_code");

//         using (UnityWebRequest www = UnityWebRequest.Post(tokenUrl, form))
//         {
//             yield return www.SendWebRequest();

//             if (www.result == UnityWebRequest.Result.Success)
//             {
//                 string jsonResponse = www.downloadHandler.text;
//                 TokenResponse token = JsonUtility.FromJson<TokenResponse>(jsonResponse);

//                 if (token != null && !string.IsNullOrEmpty(token.access_token))
//                 {
//                     // Store the token securely (e.g., in PlayerPrefs for demo, use SecurePlayerPrefs or backend in production)
//                     PlayerPrefs.SetString("GoogleAccessToken", token.access_token);
//                     if (!string.IsNullOrEmpty(token.refresh_token))
//                     {
//                         PlayerPrefs.SetString("GoogleRefreshToken", token.refresh_token);
//                     }
//                     PlayerPrefs.Save();

//                     Debug.Log("Token acquired successfully.");
//                 }
//                 else
//                 {
//                     Debug.LogError("Failed to parse token response.");
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Token exchange failed: {www.error} - {www.downloadHandler.text}");
//             }
//         }
//     }
// }









// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Net;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks; // Add for async
// using System.Linq;
// using UnityEngine;
// using UnityEngine.Networking;
// using Firebase; // Add Firebase namespaces
// using Firebase.Auth;
// using Firebase.Extensions;
// using Firebase.Firestore;

// [System.Serializable]
// public class TokenResponse
// {
//     public string access_token;
//     public string id_token; // Add this for Firebase Auth
//     public int expires_in;
//     public string token_type;
//     public string scope;
//     public string refresh_token;
// }

// public class GoogleSignIn : MonoBehaviour
// {
//     [Header("Google OAuth Settings")]
//     public string clientId = "YOUR_CLIENT_ID.apps.googleusercontent.com";
//     public string clientSecret = "YOUR_CLIENT_SECRET";
//     public string redirectUri = "http://localhost:8080";

//     [Header("UI")]
//     public GameObject gameScreen;

//     [Header("Firebase")] // New header
//     public FirebaseAuth auth; // Assign in Inspector or init in code

//     private string authCode;
//     private bool callbackReceived = false;
//     private readonly object lockObject = new object();

//     void Start()
//     {
//         // Initialize Firebase Auth
//         auth = FirebaseAuth.DefaultInstance;
//     }

//     public void OnSignInButtonClick()
//     {
//         StartCoroutine(SignInCoroutine());
//     }

//     private IEnumerator SignInCoroutine()
//     {
//         string authUrl = BuildAuthUrl();
//         Application.OpenURL(authUrl);

//         StartThreadForListener();

//         float timeout = 120f;
//         while (!callbackReceived && timeout > 0f)
//         {
//             timeout -= Time.deltaTime;
//             yield return null;
//         }

//         if (callbackReceived && !string.IsNullOrEmpty(authCode))
//         {
//             yield return StartCoroutine(ExchangeCodeForToken(authCode));
//             if (gameScreen != null)
//             {
//                 gameScreen.SetActive(true);
//             }
//             Debug.Log("Sign-in successful! Game screen enabled.");
//         }
//         else
//         {
//             Debug.LogError("Sign-in failed or timed out.");
//         }
//     }

//     private string BuildAuthUrl()
//     {
//         string baseAuthUri = "https://accounts.google.com/o/oauth2/v2/auth";
//         var authUriQueryParams = new Dictionary<string, string>
//         {
//             ["client_id"] = clientId,
//             ["redirect_uri"] = redirectUri,
//             ["response_type"] = "code",
//             ["scope"] = "openid email profile", // openid ensures id_token
//             ["access_type"] = "offline",
//             ["prompt"] = "select_account"
//         };

//         var queryString = string.Join("&", authUriQueryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
//         return $"{baseAuthUri}?{queryString}";
//     }

//     private void StartThreadForListener()
//     {
//         authCode = null;
//         callbackReceived = false;
//         var thread = new Thread(ListenForCallback);
//         thread.IsBackground = true;
//         thread.Start();
//     }

//     private void ListenForCallback()
//     {
//         using (var listener = new HttpListener())
//         {
//             string prefix = redirectUri.EndsWith("/") ? redirectUri : redirectUri + "/";
//             listener.Prefixes.Add(prefix);
//             listener.Start();

//             try
//             {
//                 var context = listener.GetContext();
//                 string code = context.Request.QueryString["code"];

//                 lock (lockObject)
//                 {
//                     authCode = code;
//                     callbackReceived = true;
//                 }

//                 string responseString = @"
//                     <html><body><h1>Login Successful!</h1><p>Close this tab.</p><script>window.close();</script></body></html>";
//                 byte[] buffer = Encoding.UTF8.GetBytes(responseString);
//                 context.Response.ContentLength64 = buffer.Length;
//                 context.Response.ContentType = "text/html";
//                 context.Response.OutputStream.Write(buffer, 0, buffer.Length);
//                 context.Response.OutputStream.Close();
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError($"Listener error: {e.Message}");
//                 lock (lockObject)
//                 {
//                     callbackReceived = true;
//                 }
//             }
//             finally
//             {
//                 listener.Stop();
//             }
//         }
//     }

//     private IEnumerator ExchangeCodeForToken(string code)
//     {
//         string tokenUrl = "https://oauth2.googleapis.com/token";

//         WWWForm form = new WWWForm();
//         form.AddField("code", code);
//         form.AddField("client_id", clientId);
//         form.AddField("client_secret", clientSecret);
//         form.AddField("redirect_uri", redirectUri);
//         form.AddField("grant_type", "authorization_code");

//         using (UnityWebRequest www = UnityWebRequest.Post(tokenUrl, form))
//         {
//             yield return www.SendWebRequest();

//             if (www.result == UnityWebRequest.Result.Success)
//             {
//                 string jsonResponse = www.downloadHandler.text;
//                 TokenResponse token = JsonUtility.FromJson<TokenResponse>(jsonResponse);

//                 if (token != null && !string.IsNullOrEmpty(token.access_token) && !string.IsNullOrEmpty(token.id_token))
//                 {
//                     // Store tokens
//                     PlayerPrefs.SetString("GoogleAccessToken", token.access_token);
//                     PlayerPrefs.SetString("GoogleIdToken", token.id_token); // New: Store ID token
//                     if (!string.IsNullOrEmpty(token.refresh_token))
//                     {
//                         PlayerPrefs.SetString("GoogleRefreshToken", token.refresh_token);
//                     }
//                     PlayerPrefs.Save();

//                     // New: Sign in to Firebase Auth with Google credential
//                     yield return StartCoroutine(SignInToFirebase(token.id_token, token.access_token));

//                     Debug.Log("Token and Firebase Auth successful.");
//                 }
//                 else
//                 {
//                     Debug.LogError("Failed to parse token response (missing id_token?). Ensure 'openid' scope.");
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Token exchange failed: {www.error} - {www.downloadHandler.text}");
//             }
//         }
//     }

//     // New: Firebase sign-in coroutine
//     private IEnumerator SignInToFirebase(string idToken, string accessToken)
//     {
//         Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);

//         var signInTask = auth.SignInAndRetrieveDataWithCredentialAsync(credential);
//         yield return signInTask.ContinueWithOnMainThread(task =>
//         {
//             if (task.IsCanceled)
//             {
//                 Debug.LogError("Firebase SignIn was canceled.");
//                 return;
//             }
//             if (task.IsFaulted)
//             {
//                 Debug.LogError("Firebase SignIn failed: " + task.Exception);
//                 return;
//             }

//             FirebaseUser user = task.Result.User;
//             Debug.Log($"Firebase User signed in: {user.DisplayName} ({user.Email})");

//             // New: Store user details in Firestore
//             StartCoroutine(StoreUserInFirestore(user));
//         });
//     }

//     // New: Write to Firestore
//     private IEnumerator StoreUserInFirestore(FirebaseUser user)
//     {
//         FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

//         DocumentReference docRef = db.Collection("users").Document(user.Email); // Key by email

//         Dictionary<string, object> userData = new Dictionary<string, object>
//         {
//             { "name", user.DisplayName },
//             { "email", user.Email },
//             { "loginTime", FieldValue.ServerTimestamp }, // Use server timestamp for accuracy
//             { "lastActive", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") } // Or add more fields
//         };

//         var writeTask = docRef.SetAsync(userData);
//         yield return writeTask.ContinueWithOnMainThread(task =>
//         {
//             if (task.IsFaulted)
//             {
//                 Debug.LogError("Firestore write failed: " + task.Exception);
//             }
//             else
//             {
//                 Debug.Log("User data stored in Firestore successfully!");
//             }
//         });
//     }
// }




using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

[System.Serializable]
public class TokenResponse
{
    public string access_token;
    public string id_token;
    public int expires_in;
    public string token_type;
    public string scope;
    public string refresh_token;
}

public class GoogleSignIn : MonoBehaviour
{
    [Header("Google OAuth Settings")]
    public string clientId = "YOUR_CLIENT_ID.apps.googleusercontent.com";
    public string clientSecret = "YOUR_CLIENT_SECRET";
    public string redirectUri = "http://localhost:8080";

    [Header("UI")]
    public GameObject displayNamePanel;
    public GameObject mainMenuPanel;

    [Header("Firebase")]
    public FirebaseAuth auth;

    private string authCode;
    private bool callbackReceived = false;
    private readonly object lockObject = new object();
    private bool firebaseInitialized = false;
    private FirebaseUser currentUser;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Firebase init failed: {task.Exception}");
                return;
            }

            DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                firebaseInitialized = true;
                Debug.Log("Firebase Auth initialized successfully!");
            }
            else
            {
                Debug.LogError($"Firebase dependencies not resolved: {dependencyStatus}");
            }
        });
    }

    public void OnSignInButtonClick()
    {
        if (!firebaseInitialized)
        {
            Debug.LogError("Firebase not initialized yet. Please wait.");
            return;
        }
        StartCoroutine(SignInCoroutine());
    }

    private IEnumerator SignInCoroutine()
    {
        string authUrl = BuildAuthUrl();
        Debug.Log($"Opening browser for OAuth: {authUrl}");
        Application.OpenURL(authUrl);

        StartThreadForListener();

        float timeout = 120f;
        while (!callbackReceived && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (callbackReceived && !string.IsNullOrEmpty(authCode))
        {
            yield return StartCoroutine(ExchangeCodeForToken(authCode));
            Debug.Log("Sign-in successful! Game screen enabled.");
        }
        else
        {
            Debug.LogError("Sign-in failed or timed out.");
        }
    }

    private string BuildAuthUrl()
    {
        string baseAuthUri = "https://accounts.google.com/o/oauth2/v2/auth";
        var authUriQueryParams = new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["redirect_uri"] = redirectUri,
            ["response_type"] = "code",
            ["scope"] = "openid email profile",
            ["access_type"] = "offline",
            ["prompt"] = "select_account"
        };

        var queryString = string.Join("&", authUriQueryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        return $"{baseAuthUri}?{queryString}";
    }

    private void StartThreadForListener()
    {
        authCode = null;
        callbackReceived = false;
        var thread = new Thread(ListenForCallback);
        thread.IsBackground = true;
        thread.Start();
    }

    private void ListenForCallback()
    {
        using (var listener = new HttpListener())
        {
            string prefix = redirectUri.EndsWith("/") ? redirectUri : redirectUri + "/";
            listener.Prefixes.Add(prefix);
            try
            {
                listener.Start();
                Debug.Log($"Listening for OAuth callback on {prefix}");
                var context = listener.GetContext();
                string code = context.Request.QueryString["code"];

                lock (lockObject)
                {
                    authCode = code;
                    callbackReceived = true;
                    Debug.Log($"Received auth code: {code}");
                }

                string responseString = @"
                    <html><body><h1>Login Successful!</h1><p>Close this tab.</p><script>window.close();</script></body></html>";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.ContentType = "text/html";
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception e)
            {
                Debug.LogError($"Listener error: {e.Message}\n{e.StackTrace}");
                lock (lockObject)
                {
                    callbackReceived = true;
                }
            }
            finally
            {
                listener.Stop();
            }
        }
    }

    private IEnumerator ExchangeCodeForToken(string code)
    {
        string tokenUrl = "https://oauth2.googleapis.com/token";
        WWWForm form = new WWWForm();
        form.AddField("code", code);
        form.AddField("client_id", clientId);
        form.AddField("client_secret", clientSecret);
        form.AddField("redirect_uri", redirectUri);
        form.AddField("grant_type", "authorization_code");

        using (UnityWebRequest www = UnityWebRequest.Post(tokenUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                TokenResponse token = JsonUtility.FromJson<TokenResponse>(jsonResponse);

                if (token != null && !string.IsNullOrEmpty(token.access_token) && !string.IsNullOrEmpty(token.id_token))
                {
                    Debug.Log($"Access Token: {token.access_token.Substring(0, Math.Min(token.access_token.Length, 20))}...");
                    Debug.Log($"ID Token: {token.id_token.Substring(0, Math.Min(token.id_token.Length, 20))}...");
                    PlayerPrefs.SetString("GoogleAccessToken", token.access_token);
                    PlayerPrefs.SetString("GoogleIdToken", token.id_token);
                    if (!string.IsNullOrEmpty(token.refresh_token))
                    {
                        Debug.Log($"Refresh Token: {token.refresh_token.Substring(0, Math.Min(token.refresh_token.Length, 20))}...");
                        PlayerPrefs.SetString("GoogleRefreshToken", token.refresh_token);
                    }
                    PlayerPrefs.Save();

                    string tokenInfoUrl = $"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={token.id_token}";
                    using (UnityWebRequest tokenCheck = UnityWebRequest.Get(tokenInfoUrl))
                    {
                        yield return tokenCheck.SendWebRequest();
                        if (tokenCheck.result == UnityWebRequest.Result.Success)
                        {
                            Debug.Log($"Token info: {tokenCheck.downloadHandler.text}");
                        }
                        else
                        {
                            Debug.LogError($"Token validation failed: {tokenCheck.error}");
                            yield break;
                        }
                    }

                    yield return StartCoroutine(SignInToFirebase(token.id_token, token.access_token));
                }
                else
                {
                    Debug.LogError($"Token response issue: access_token={(token?.access_token ?? "null")}, id_token={(token?.id_token ?? "null")}");
                }
            }
            else
            {
                Debug.LogError($"Token exchange failed: {www.error} - {www.downloadHandler.text}");
            }
        }
    }

    private IEnumerator SignInToFirebase(string idToken, string accessToken)
    {
        Debug.Log($"Attempting Firebase sign-in with id_token: {idToken.Substring(0, Math.Min(idToken.Length, 20))}...");
        Credential credential;
        try
        {
            credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create Google credential: {ex.Message}\n{ex.StackTrace}");
            yield break;
        }

        var signInTask = auth.SignInAndRetrieveDataWithCredentialAsync(credential);
        yield return new WaitUntil(() => signInTask.IsCompleted);

        if (signInTask.IsCanceled)
        {
            Debug.LogError("Firebase SignIn was canceled.");
            yield break;
        }
        if (signInTask.IsFaulted)
        {
            Debug.LogError($"Firebase SignIn failed: {signInTask.Exception}");
            foreach (var innerException in signInTask.Exception.InnerExceptions)
            {
                Debug.LogError($"Inner exception: {innerException.Message}\n{innerException.StackTrace}");
            }
            yield break;
        }

        AuthResult authResult = signInTask.Result;
        currentUser = authResult.User;
        Debug.Log($"Firebase User signed in: {currentUser.DisplayName} ({currentUser.Email}) (UID: {currentUser.UserId})");

        // Check Firestore for displayName to determine first-time user
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("users").Document(currentUser.Email);
        var getTask = docRef.GetSnapshotAsync();
        yield return new WaitUntil(() => getTask.IsCompleted);

        bool isFirstTimeUser = true;
        if (getTask.IsFaulted)
        {
            Debug.LogError($"Failed to check user document: {getTask.Exception}");
        }
        else
        {
            DocumentSnapshot snapshot = getTask.Result;
            if (snapshot.Exists && snapshot.ContainsField("displayName"))
            {
                isFirstTimeUser = false;
                string displayName = snapshot.GetValue<string>("displayName");
                Debug.Log($"Existing user with display name: {displayName}");
                // Store displayName for use in game (e.g., GameManager)
                // Example: GameManager.Instance.PlayerDisplayName = displayName;
            }
        }

        if (isFirstTimeUser)
        {
            Debug.Log("First-time user detected. Enabling display name screen.");
            if (displayNamePanel != null)
            {
                displayNamePanel.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Existing user detected. Going to main menu.");
            GoToMainMenu();
        }
    }

    private void GoToMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        Debug.Log("Proceeding to main menu.");
    }
}