using UnityEngine;
using System.Collections.Generic;
using Steamworks;

public class GameManager : MonoBehaviour
{
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    private List<Vector3> spawnPositions;


    public GameState gameState; 

    

    public static GameManager instance;

    void Start()
    {
        if (instance == null) {
            instance = this;
        }
    }

public List<Vector3> FindSurvivalSpawnPositions(int size) {
    List<Vector3> list = new List<Vector3>();

    list.Add(new Vector3(213, 20, 134));
    list.Add(new Vector3(413, 20, 24));
    list.Add(new Vector3(213, 20, 134));
    list.Add(new Vector3(113, 20, 434));

    return list;
}


  public void SendPlayersIntoGame(List<Vector3> spawnPositions)
  {
    int index = 0;
    foreach (Client client1 in Server.clients.Values)
    {
      if (client1?.player != null)
      {
        foreach (Client client2 in Server.clients.Values)
        {
          if (client2?.player != null)
          {
            ServerSend.SpawnPlayer(client1.id, client2.player, this.spawnPositions[index] + Vector3.up);
            ++index;
          }
        }
      }
    }
  }

}


 public enum GameState {
        Lobby,
        Game,
        GameOver,
    }
