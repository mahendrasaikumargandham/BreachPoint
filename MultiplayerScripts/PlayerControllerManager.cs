using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using HashTable = ExitGames.Client.Photon.Hashtable;

public class PlayerControllerManager : MonoBehaviourPunCallbacks
{
    PhotonView view;
    GameObject controller;
    public int playerTeam;
    private Dictionary<int, int> playerTeams = new Dictionary<int, int>(); 

    void Awake() {
        view = GetComponent<PhotonView>();
    }

    void Start() 
    {
        if(view.IsMine) {
            CreateController();
        }
    }

    void CreateController() {
        if(PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team")) {
            playerTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            Debug.Log("Player's team: " +playerTeam);
        }
        AssignPlayerToSpawnArea(playerTeam);
    }

    void AssignPlayerToSpawnArea(int team) {
        GameObject spawnArea1 = GameObject.Find("SpawnArea1");
        GameObject spawnArea2 = GameObject.Find("SpawnArea2");

        if(spawnArea1 == null || spawnArea2 == null) {
            Debug.LogError("Spawn Area is null");
            return;
        }

        Transform spawnAreaPoint = null;

        if(team == 1) {
            spawnAreaPoint = spawnArea1.transform.GetChild(Random.Range(0, spawnArea1.transform.childCount));
        }
        else if(team == 2) {
            spawnAreaPoint = spawnArea2.transform.GetChild(Random.Range(0, spawnArea2.transform.childCount));
        }

        if(spawnAreaPoint != null) {
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnAreaPoint.position, spawnAreaPoint.rotation, 0, new object[]{view.ViewID});
            Debug.Log("Player is instantiated");
        }
        else {
            Debug.LogError("No available spawn points for Team's " + team);
        }
    }

    void AssignTeamsToAllPlayers() {
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            if(player.CustomProperties.ContainsKey("Team")) {
                int team = (int)player.CustomProperties["Team"];
                playerTeams[player.ActorNumber] = team;
                Debug.Log(player.NickName + "'s Team : " +team);
                AssignPlayerToSpawnArea(team);
            }
        }
    }

    public void Die() {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        AssignTeamsToAllPlayers();
    }
}
