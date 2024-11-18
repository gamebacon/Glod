using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class LobbyVisuals : MonoBehaviour
{
    public GameObject[] playerModels;
    public List<LobbyPlayer> lobbyPlayers;
    public Transform[] playerPos;

    [SerializeField]
    private GameObject lobbyPlayerPrefab;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AddPlayer(string playerName, string playerId) {
        Debug.Log($"{playerName} added as lobby player");
        int playerNum = lobbyPlayers.Count + 1;
        Transform parentPos = playerPos[playerNum - 1];
        GameObject obj = Instantiate(lobbyPlayerPrefab, parentPos);
        LobbyPlayer lobbyPlayer = obj.GetComponent<LobbyPlayer>();
        lobbyPlayers.Add(lobbyPlayer);
        lobbyPlayer.Init(playerName, playerId);
    }

public void RemovePlayer(string playerId) {
    Debug.Log($"{playerId} removed as lobby player");
    LobbyPlayer lobbyPlayer = lobbyPlayers.FirstOrDefault(p => p.playerId == playerId);
    if (lobbyPlayer != null) {
        lobbyPlayers.Remove(lobbyPlayer);
        Destroy(lobbyPlayer.gameObject);
    } else {
        Debug.LogWarning($"Player with ID {playerId} not found in the lobby.");
    }
}


}
