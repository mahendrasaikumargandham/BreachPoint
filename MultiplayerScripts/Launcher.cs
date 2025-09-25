// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Threading.Tasks;
// using Firebase.Auth;
// using Firebase.Firestore;
// using Photon.Pun;
// using UnityEngine.UI;
// using Photon.Realtime;
// using System.Linq;

// public class Launcher : MonoBehaviourPunCallbacks
// {
//     public static Launcher instance;

//     [SerializeField] InputField roomNameInputField;
//     [SerializeField] Text roomNameText;
//     [SerializeField] Text errorText;
//     [SerializeField] Transform roomListContent;
//     [SerializeField] GameObject roomListItemPrefab;
//     [SerializeField] Transform playerRedItem;
//     [SerializeField] Transform playerBlueItem;
//     [SerializeField] GameObject playerListItemPrefab;
//     public GameObject startButton;

//     int nextTeamNumber = 1;

//     void Awake()
//     {
//         instance = this;
//     }

//     void Start()
//     {
//         Debug.Log("Connecting to Photon...");
//         PhotonNetwork.ConnectUsingSettings();
//     }

//     // MODIFIED: This function now asynchronously sets the nickname before joining the lobby.
//     public async override void OnConnectedToMaster()
//     {
//         Debug.Log("Connected to Master Server. Setting Nickname...");
//         await SetNicknameFromFirebase(); // Wait for the nickname to be fetched and set

//         Debug.Log($"Nickname is now '{PhotonNetwork.NickName}'. Joining Lobby...");
//         PhotonNetwork.JoinLobby();
//         PhotonNetwork.AutomaticallySyncScene = true;
//     }

//     // MODIFIED: Removed the old random nickname assignment from here.
//     public override void OnJoinedLobby()
//     {
//         MenuManager.instance.OpenMenu("TitleMenu");
//         Debug.Log("Joined Lobby");
//         // The nickname has already been set in OnConnectedToMaster.
//     }

//     public void CreateRoom()
//     {
//         if (string.IsNullOrEmpty(roomNameInputField.text))
//         {
//             return;
//         }

//         PhotonNetwork.CreateRoom(roomNameInputField.text);
//         MenuManager.instance.OpenMenu("LoadingMenu");
//     }

//     public override void OnJoinedRoom()
//     {
//         MenuManager.instance.OpenMenu("RoomMenu");
//         roomNameText.text = PhotonNetwork.CurrentRoom.Name;

//         Player[] players = PhotonNetwork.PlayerList;

//         foreach (Transform child in playerRedItem)
//         {
//             Destroy(child.gameObject);
//         }
//         // Also clear the blue team list
//         foreach (Transform child in playerBlueItem)
//         {
//             Destroy(child.gameObject);
//         }

//         for (int i = 0; i < players.Count(); i++)
//         {
//             int teamNumber = GetNextTeamNumber();
//             Instantiate(playerListItemPrefab, (teamNumber % 2 == 1 ? playerRedItem : playerBlueItem)).GetComponent<PlayerListItem>().SetUp(players[i], teamNumber);
//         }

//         startButton.SetActive(PhotonNetwork.IsMasterClient);
//     }

//     public override void OnMasterClientSwitched(Player newMasterClient)
//     {
//         startButton.SetActive(PhotonNetwork.IsMasterClient);
//     }

//     public override void OnCreateRoomFailed(short returnCode, string errMessage)
//     {
//         errorText.text = "Room Generation Failed! " + errMessage;
//         MenuManager.instance.OpenMenu("ErrorMenu");
//     }

//     public void JoinRoom(RoomInfo info)
//     {
//         PhotonNetwork.JoinRoom(info.Name);
//         MenuManager.instance.OpenMenu("LoadingMenu");
//     }

//     public void StartGame()
//     {
//         PhotonNetwork.LoadLevel(1);
//     }

//     public void LeaveRoom()
//     {
//         PhotonNetwork.LeaveRoom();
//         MenuManager.instance.OpenMenu("LoadingMenu");
//     }

//     public override void OnLeftRoom()
//     {
//         MenuManager.instance.OpenMenu("TitleMenu");
//     }

//     public override void OnRoomListUpdate(List<RoomInfo> roomList)
//     {
//         foreach (Transform trans in roomListContent)
//         {
//             Destroy(trans.gameObject);
//         }

//         for (int i = 0; i < roomList.Count; i++)
//         {
//             if (roomList[i].RemovedFromList)
//             {
//                 continue;
//             }
//             Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
//         }
//     }

//     public override void OnPlayerEnteredRoom(Player newPlayer)
//     {
//         int teamNumber = GetNextTeamNumber();
//         GameObject playerItem = Instantiate(playerListItemPrefab, (teamNumber % 2 == 1 ? playerRedItem : playerBlueItem));
//         playerItem.GetComponent<PlayerListItem>().SetUp(newPlayer, teamNumber);
//     }

//     private int GetNextTeamNumber()
//     {
//         int teamNumber = nextTeamNumber;
//         nextTeamNumber = 3 - nextTeamNumber;
//         return teamNumber;
//     }

//     public void QuitGame()
//     {
//         Application.Quit();
//     }

//     private async Task SetNicknameFromFirebase()
//     {
//         FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

//         // If a user is not logged in, assign a random guest name and finish
//         if (user == null)
//         {
//             PhotonNetwork.NickName = "Player" + Random.Range(0, 10000).ToString("0000");
//             Debug.Log($"No user logged in. Assigned random NickName: {PhotonNetwork.NickName}");
//             return;
//         }

//         // If user is logged in, try to fetch their display name
//         try
//         {
//             FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
//             DocumentReference docRef = db.Collection("users").Document(user.Email);
//             DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

//             if (snapshot.Exists && snapshot.TryGetValue("displayName", out string displayName) && !string.IsNullOrEmpty(displayName))
//             {
//                 PhotonNetwork.NickName = displayName;
//                 Debug.Log($"Successfully set NickName from Firestore: {PhotonNetwork.NickName}");
//             }
//             else
//             {
//                 // Fallback for logged-in user without a display name set
//                 PhotonNetwork.NickName = "User" + Random.Range(0, 1000).ToString("000");
//                 Debug.LogWarning($"User {user.Email} has no displayName. Assigned random NickName: {PhotonNetwork.NickName}");
//             }
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"Error fetching nickname: {e.Message}. Assigning random name as a fallback.");
//             PhotonNetwork.NickName = "Player" + Random.Range(0, 10000).ToString("0000");
//         }
//     }
// }



































































































using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.Linq;
// FIX: Explicitly tell the compiler to use Photon's Hashtable for this script.
using Hashtable = ExitGames.Client.Photon.Hashtable; 

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    [SerializeField] InputField roomNameInputField;
    [SerializeField] Text roomNameText;
    [SerializeField] Text errorText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    
    // RENAMED for clarity
    [SerializeField] Transform redTeamContent; 
    [SerializeField] Transform blueTeamContent; 

    [SerializeField] GameObject playerListItemPrefab;
    public GameObject startButton;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public async override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server. Setting Nickname...");
        await SetNicknameFromFirebase();

        Debug.Log($"Nickname is now '{PhotonNetwork.NickName}'. Joining Lobby...");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.instance.OpenMenu("TitleMenu");
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text)) return;
        PhotonNetwork.CreateRoom(roomNameInputField.text, new RoomOptions { MaxPlayers = 10 }); // Example: max 10 players
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("RoomMenu");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        
        // When we join, assign ourselves to a team if we don't have one.
        // The Master Client will handle the initial assignment for new players.
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team") == false)
        {
            AssignPlayerToTeam(PhotonNetwork.LocalPlayer);
        }

        UpdatePlayerList();
    }

    // --- NEW: This is the core function for refreshing the entire UI ---
    void UpdatePlayerList()
    {
        // Clear existing player lists
        foreach (Transform child in redTeamContent) { Destroy(child.gameObject); }
        foreach (Transform child in blueTeamContent) { Destroy(child.gameObject); }

        // Loop through all players in the room
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Transform parentContent = null;
            if (player.CustomProperties.TryGetValue("Team", out object teamValue))
            {
                int team = (int)teamValue;
                parentContent = (team == 1) ? redTeamContent : blueTeamContent;
            }
            else
            {
                // If a player somehow has no team, put them in the red team by default.
                parentContent = redTeamContent;
            }

            GameObject playerItem = Instantiate(playerListItemPrefab, parentContent);
            playerItem.GetComponent<PlayerListItem>().SetUp(player); // We no longer set team here

            // --- IMPORTANT: Enable dragging only for the Master Client ---
            DraggablePlayer draggable = playerItem.GetComponent<DraggablePlayer>();
            draggable.playerInfo = player; // Give the draggable script the player's data
            draggable.SetDraggable(PhotonNetwork.IsMasterClient);
        }
        
        // Update the Start Button visibility
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    
    // --- NEW: This Photon callback is triggered whenever a player's properties change ---
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // If the "Team" property was changed, update the UI for everyone.
        if (changedProps.ContainsKey("Team"))
        {
            UpdatePlayerList();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // When the host changes, refresh the UI to enable/disable dragging.
        UpdatePlayerList();
    }

    // --- NEW: Called by the DropZone script ---
    public void RequestTeamChange(Player playerToMove, int newTeamID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Get current team counts
        int redTeamCount = PhotonNetwork.PlayerList.Count(p => (int)p.CustomProperties["Team"] == 1);
        int blueTeamCount = PhotonNetwork.PlayerList.Count(p => (int)p.CustomProperties["Team"] == 2);
        int playerCurrentTeam = (int)playerToMove.CustomProperties["Team"];

        // --- ENFORCE THE RULES ---
        // Don't allow the move if it would leave a team empty.
        if (playerCurrentTeam == 1 && redTeamCount == 1 && blueTeamCount > 0)
        {
            Debug.Log("Move denied: Cannot leave Red Team empty.");
            UpdatePlayerList(); // Snap the player back visually
            return;
        }
        if (playerCurrentTeam == 2 && blueTeamCount == 1 && redTeamCount > 0)
        {
            Debug.Log("Move denied: Cannot leave Blue Team empty.");
            UpdatePlayerList(); // Snap the player back visually
            return;
        }

        // If the move is valid, set the custom property. This triggers OnPlayerPropertiesUpdate for everyone.
        var properties = new Hashtable { { "Team", newTeamID } };
        playerToMove.SetCustomProperties(properties);
    }

    public override void OnCreateRoomFailed(short returnCode, string errMessage)
    {
        errorText.text = "Room Generation Failed! " + errMessage;
        MenuManager.instance.OpenMenu("ErrorMenu");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("TitleMenu");
    }



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent) { Destroy(trans.gameObject); }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList) continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    // --- MODIFIED: Handles new players joining the room ---
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // The Master Client is responsible for assigning the new player to a team.
        if (PhotonNetwork.IsMasterClient)
        {
            AssignPlayerToTeam(newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    // --- NEW: Logic for auto-balancing new players ---
    void AssignPlayerToTeam(Player player)
    {
        // Count players on each team
        int redTeamCount = PhotonNetwork.PlayerList.Count(p => p.CustomProperties.ContainsKey("Team") && (int)p.CustomProperties["Team"] == 1);
        int blueTeamCount = PhotonNetwork.PlayerList.Count(p => p.CustomProperties.ContainsKey("Team") && (int)p.CustomProperties["Team"] == 2);

        // Assign to the team with fewer players
        int teamToJoin = (redTeamCount <= blueTeamCount) ? 1 : 2;
        var properties = new Hashtable { { "Team", teamToJoin } };
        player.SetCustomProperties(properties);
    }
    
    // REMOVED GetNextTeamNumber as it's no longer needed.

    public void QuitGame() { Application.Quit(); }
    
    // Your Firebase method is fine and does not need changes.
    private async Task SetNicknameFromFirebase()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        // If a user is not logged in, assign a random guest name and finish
        if (user == null)
        {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 10000).ToString("0000");
            Debug.Log($"No user logged in. Assigned random NickName: {PhotonNetwork.NickName}");
            return;
        }

        // If user is logged in, try to fetch their display name
        try
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef = db.Collection("users").Document(user.Email);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists && snapshot.TryGetValue("displayName", out string displayName) && !string.IsNullOrEmpty(displayName))
            {
                PhotonNetwork.NickName = displayName;
                Debug.Log($"Successfully set NickName from Firestore: {PhotonNetwork.NickName}");
            }
            else
            {
                // Fallback for logged-in user without a display name set
                PhotonNetwork.NickName = "User" + Random.Range(0, 1000).ToString("000");
                Debug.LogWarning($"User {user.Email} has no displayName. Assigned random NickName: {PhotonNetwork.NickName}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error fetching nickname: {e.Message}. Assigning random name as a fallback.");
            PhotonNetwork.NickName = "Player" + Random.Range(0, 10000).ToString("0000");
        }
    }
}



