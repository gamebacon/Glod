using UnityEngine;
using System.Collections.Generic;
using Steamworks;

public class GameManager : MonoBehaviour
{
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    private List<Vector3> spawnPositions;

    public static bool isConnected;

    public GameState gameState; 

    

    public static GameManager instance;

    void Start()
    {
        if (instance == null) {
            instance = this;
        }
        AudioManager.Instance.Play("Menu");
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


  public void GameOver(int winnerId, float time = 4f)
  {
    Debug.Log("game over");
    
    /*
    this.winnerId = winnerId;
    this.Invoke("ShowEndScreen", time);
    MusicController.Instance.StopSong();
    AchievementManager.Instance.CheckGameOverAchievements(winnerId);
    */
  }

  public void StartGame()
  {
    /*
    LoadingScreen.Instance.Hide();
    this.lobbyCamera.SetActive(false);
    */

    GameManager.instance.gameState = GameState.Game;

    /*
    if (LocalClient.serverOwner) {
      GameLoop.Instance.StartLoop();
    }
    */

    /*
    Hotbar.Instance.UpdateHotbar();
    AchievementManager.Instance.StartGame(GameManager.gameSettings.difficulty);
    */
  }


  public void DisconnectPlayer(int id)
  {
    if (GameManager.players[id] != null && GameManager.players[id].gameObject != null)
    {
      Destroy(GameManager.players[id].gameObject);
      GameManager.players[id].isDead = true;
      GameManager.players[id].isDisconnected = true;
    }
    GameManager.players?.Remove(id);
  }


  public void LeaveGame()
  {
    /*
    if (LocalClient.serverOwner)
    {
      Debug.LogError((object) "Host left game");
      this.HostLeftGame();
    }
    else
      ClientSend.PlayerDisconnect();
    SteamManager.Instance.leaveLobby();
    SceneManager.LoadScene("Menu");
    LocalClient.instance.serverHost = new SteamId();
    LocalClient.serverOwner = false;
    */
  }


    public void SpawnPlayer(
    int id,
    string username,
    Color color,
    Vector3 position,
    float orientationY)
  {
    if (GameManager.players.ContainsKey(id)) {
      return;
    }

    Debug.Log("Spawning player");

    /*
    PlayerManager component;
    if (id == LocalClient.instance.myId)
    {
      Instantiate<GameObject>(localPlayerPrefab, position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
      component = PlayerMovement.Instance.gameObject.GetComponent<PlayerManager>();
    }
    else
    {
      component = Instantiate<GameObject>(playerPrefab, position, Quaternion.Euler(0.0f, orientationY, 0.0f)).GetComponent<PlayerManager>();
      component.SetDesiredPosition(position);
    }
    component.SetDesiredPosition(position);
    component.id = id;
    component.username = username;
    component.color = color;
    GameManager.players.Add(id, component);
    if ((GameManager.gameSettings.gameMode != GameSettings.GameMode.Versus || id != LocalClient.instance.myId) && GameManager.gameSettings.gameMode == GameSettings.GameMode.Versus)
      return;
    this.extraUi.InitPlayerStatus(id, username, component);
    */


  }

}


 public enum GameState {
        Lobby,
        Game,
        GameOver,
    }
