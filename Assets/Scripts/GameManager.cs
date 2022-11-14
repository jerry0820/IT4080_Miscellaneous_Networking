using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    public Player playerPrefab;
    public GameObject spawnPoints;

    private int spawnIndex = 0;
    private List<Vector3> availableSpawnPositions = new List<Vector3>();
    private List<Player> players = new List<Player>(); 

    private void Start(){
        GameData.dbgRun.StartGameWithSceneIfNotStarted();
    }

    public void Awake(){
        refreshSpawnPoints();
    }

    public override void OnNetworkSpawn(){
        if(IsHost){
            SpawnPlayers();
        }
    }

    private void refreshSpawnPoints() {
        Transform[] allPoint = spawnPoints.GetComponentsInChildren<Transform>();
        availableSpawnPositions.Clear();
        foreach(Transform point in allPoint){
            if (point != spawnPoints.transform){
            availableSpawnPositions.Add(point.localPosition);
            }
        }
    }

    public Vector3 GetNextSpawnLocation(){
        var newPosition = availableSpawnPositions[spawnIndex];
        newPosition.y = 1.5f;
        spawnIndex += 1;

        if (spawnIndex > availableSpawnPositions.Count - 1){
            spawnIndex = 0;
        }
        return newPosition;
    }

    private void SpawnPlayers() {
        foreach (PlayerInfo info in GameData.Instance.allPlayers){
            SpawnPlayers(info);
        }
    }
    private void SpawnPlayers(PlayerInfo info){
        Player playerSpawn = Instantiate(
            playerPrefab,
            GetNextSpawnLocation(),
            Quaternion.identity);

        playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(info.clientId);
        playerSpawn.PlayerColor.Value = info.color;
        players.Add(playerSpawn);
        //playerSpawn.Score.OnValueChanged += HostOnPlayerScoreChanged;
    }

    private void HostOnClientConnected(ulong clientId){
        int playerIndex = GameData.Instance.FindPlayerIndex(clientId);
        if(playerIndex != -1){
            PlayerInfo newPlayerInfo = GameData.Instance.allPlayers[playerIndex];
            SpawnPlayers(newPlayerInfo);
        }
    }
}