// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using Photon.Realtime;
// using Photon.Pun;

// public class PlayerListItem : MonoBehaviourPunCallbacks
// {
//     public Text playerUserName;
//     Player player;
//     int team;

//     public void SetUp(Player _player, int _team) {
//         player = _player;
//         team = _team;
//         playerUserName.text = _player.NickName;

//         ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
//         customProps["Team"] = _team;
//         _player.SetCustomProperties(customProps);
//     }

//     public override void OnPlayerLeftRoom(Player otherPlayer) {
//         if(player == otherPlayer) {
//             Destroy(gameObject);
//         }
//     }

//     public override void OnLeftRoom() {
//         Destroy(gameObject);
//     }
// }

















using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] Text playerUserName;
    
    // --- NEW: Add this line ---
    // Drag your new crown icon or "(Host)" text object here in the Inspector.
    [SerializeField] GameObject hostIndicator; 

    Player player;

    // The SetUp method is now responsible for showing the host indicator.
    public void SetUp(Player _player) {
        player = _player;
        playerUserName.text = _player.NickName;

        // --- NEW LOGIC ---
        // Photon's Player object has a handy IsMasterClient property.
        // We use it to activate or deactivate the indicator object.
        if (hostIndicator != null)
        {
            hostIndicator.SetActive(_player.IsMasterClient);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if(player == otherPlayer) {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom() {
        Destroy(gameObject);
    }
}



