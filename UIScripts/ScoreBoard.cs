// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using Photon.Realtime;
// using Photon.Pun;

// public class ScoreBoard : MonoBehaviour
// {
//     public static ScoreBoard instance;
//     public Text blueTeamText;
//     public Text redTeamText;

//     public int blueTeamScore = 0;
//     public int redTeamScore = 0;

//     private PhotonView view;

//     void Awake() {
//         view = GetComponent<PhotonView>();
//         instance = this;
//     }

//     public void PlayerHasDied(int playerTeam) {
//         if(playerTeam == 2) {
//             blueTeamScore += 1;
//         }
//         if(playerTeam == 1) {
//             redTeamScore += 1;
//         }

//         view.RPC("UpdateScores", RpcTarget.All, blueTeamScore, redTeamScore);
//     }

//     [PunRPC]
//     void UpdateScores(int blueScore, int redScore) {
//         blueTeamScore = blueScore;
//         redTeamScore = redScore;

//         blueTeamText.text = blueTeamScore.ToString();
//         redTeamText.text = redTeamScore.ToString();
//     }
// }







// using UnityEngine;
// using UnityEngine.UI;
// using Photon.Pun;

// public class ScoreBoard : MonoBehaviour
// {
//     public static ScoreBoard instance;

//     // UI Texts as you defined them.
//     // We will now interpret them as:
//     // blueTeamText: Displays YOUR team's score.
//     // redTeamText: Displays the ENEMY team's score.
//     public Text blueTeamText;
//     public Text redTeamText;

//     // These variables will hold the TRUE, authoritative score for each team across the network.
//     // They are private to prevent other scripts from changing them directly.
//     private int actualBlueTeamScore = 0;
//     private int actualRedTeamScore = 0;

//     private PhotonView view;

//     void Awake()
//     {
//         // Standard singleton pattern to ensure there is only one scoreboard.
//         if (instance != null && instance != this)
//         {
//             Destroy(this.gameObject);
//         }
//         else
//         {
//             instance = this;
//         }

//         view = GetComponent<PhotonView>();
//     }

//     /// <summary>
//     /// Call this method when a player dies. Pass the team ID of the player who was killed.
//     /// This method ensures only the Master Client calculates the score to prevent cheating and sync issues.
//     /// </summary>
//     /// <param name="teamOfPlayerWhoDied">Team ID (1 for Blue, 2 for Red) of the player who died.</param>
//     public void PlayerHasDied(int teamOfPlayerWhoDied)
//     {
//         // Only the Master Client should have the authority to update the score.
//         if (!PhotonNetwork.IsMasterClient)
//             return;

//         if (teamOfPlayerWhoDied == 2) // A Red team player died, so the Blue team gets a point.
//         {
//             actualBlueTeamScore++;
//         }
//         else if (teamOfPlayerWhoDied == 1) // A Blue team player died, so the Red team gets a point.
//         {
//             actualRedTeamScore++;
//         }

//         // Use an RPC to send the new, authoritative scores to all players.
//         view.RPC("RPC_UpdateScores", RpcTarget.All, actualBlueTeamScore, actualRedTeamScore);
//     }

//     [PunRPC]
//     void RPC_UpdateScores(int blueScore, int redScore)
//     {
//         // Update the master score list on every client.
//         actualBlueTeamScore = blueScore;
//         actualRedTeamScore = redScore;

//         // Get the local player's team from their custom properties.
//         // IMPORTANT: Make sure you set this custom property when a player joins a team!
//         // Example when joining blue team:
//         // PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable {{"team", 1}});
//         if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object localPlayerTeamObj))
//         {
//             int localPlayerTeam = (int)localPlayerTeamObj;

//             // Now, display the scores relative to the local player's team.
//             if (localPlayerTeam == 1) // If I am on the Blue Team...
//             {
//                 // My team's score is the blue score.
//                 blueTeamText.text = actualBlueTeamScore.ToString();
//                 // The enemy's score is the red score.
//                 redTeamText.text = actualRedTeamScore.ToString();
//             }
//             else if (localPlayerTeam == 2) // If I am on the Red Team...
//             {
//                 // My team's score is the red score.
//                 blueTeamText.text = actualRedTeamScore.ToString();
//                 // The enemy's score is the blue score.
//                 redTeamText.text = actualBlueTeamScore.ToString();
//             }
//         }
//         else
//         {
//             // This is a fallback for spectators or if the "team" property hasn't been set yet.
//             // It will just show the absolute scores.
//             blueTeamText.text = blueScore.ToString();
//             redTeamText.text = redScore.ToString();
//         }
//     }
// }









using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon; // Required for Hashtable

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard instance;

    // blueTeamText = YOUR team's score
    // redTeamText = ENEMY team's score
    public Text blueTeamText;
    public Text redTeamText;

    private int actualBlueTeamScore = 0; // Authoritative score for Team 1
    private int actualRedTeamScore = 0; // Authoritative score for Team 2

    private PhotonView view;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        view = GetComponent<PhotonView>();
    }

    // --- CHANGE HERE: THIS IS THE NEW AUTHORITATIVE METHOD ---
    // This RPC is called BY a dying player and is ONLY RECEIVED by the Master Client.
    [PunRPC]
    void RPC_ReportDeath(int deadPlayerTeam)
    {
        // This check is redundant because the RPC target is MasterClient, but it's good for safety.
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log($"MasterClient received death report for a player on team {deadPlayerTeam}.");

        if (deadPlayerTeam == 2) // A player from Team 2 (Red) died.
        {
            actualBlueTeamScore++; // Team 1 (Blue) gets a point.
        }
        else if (deadPlayerTeam == 1) // A player from Team 1 (Blue) died.
        {
            actualRedTeamScore++; // Team 2 (Red) gets a point.
        }

        // Now, the Master Client broadcasts the new, correct scores to everyone.
        view.RPC("RPC_UpdateScoresForAll", RpcTarget.All, actualBlueTeamScore, actualRedTeamScore);
    }

    // This RPC is sent BY the Master Client and is RECEIVED by all players.
    [PunRPC]
    void RPC_UpdateScoresForAll(int blueScore, int redScore)
    {
        actualBlueTeamScore = blueScore;
        actualRedTeamScore = redScore;

        // Get the local player's team from their custom properties.
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object localPlayerTeamObj))
        {
            int localPlayerTeam = (int)localPlayerTeamObj;

            if (localPlayerTeam == 1) // If I am on Blue Team
            {
                blueTeamText.text = actualBlueTeamScore.ToString();
                redTeamText.text = actualRedTeamScore.ToString();
            }
            else if (localPlayerTeam == 2) // If I am on Red Team
            {
                // My team's score is the red score, shown in the "blue" text field.
                blueTeamText.text = actualRedTeamScore.ToString();
                // The enemy's score is the blue score, shown in the "red" text field.
                redTeamText.text = actualBlueTeamScore.ToString();
            }
        }
    }
}

