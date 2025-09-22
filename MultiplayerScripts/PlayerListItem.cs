using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    public Text playerUserName;
    Player player;
    int team;

    public void SetUp(Player _player, int _team) {
        player = _player;
        team = _team;
        playerUserName.text = _player.NickName;

        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps["Team"] = _team;
        _player.SetCustomProperties(customProps);
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
