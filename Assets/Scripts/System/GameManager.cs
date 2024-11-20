using System;
using UnityEngine;
using System.Collections.Generic;
using Steamworks;

public class GameManager : MonoBehaviour
{
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    private List<Vector3> spawnPositions;

    public static bool isConnected;

    public GameState gameState; 

    [SerializeField]
    private GameObject playerPrefab;

    

    public static GameManager instance;

  private void Awake()
  {
    if (GameManager.instance == null)
    {
      GameManager.instance = this;
    } else if (GameManager.instance != this) {
      Destroy(this);
    }

    GameManager.players = new Dictionary<int, PlayerManager>();
  }


    void Start()
    {
        AudioManager.Instance.Play("Menu");
    }

public List<Vector3> FindSurvivalSpawnPositions(int size) {
    List<Vector3> list = new List<Vector3>();

  for(int i = 0; i <= size; i++) {
    list.Add(new Vector3(213, 20, 134));
  }

    return list;
}


  public void SendPlayersIntoGame(int amount)
  {
    Debug.Log($"Init spawnpos {amount} ");
    spawnPositions = FindSurvivalSpawnPositions(amount);
    Invoke("SendPlayersIntoGameNow", 2f);
  }  

  private void SendPlayersIntoGameNow()
  {
    int index = 0;
    foreach (Client toClient in Server.clients.Values)
    {
      if (toClient?.player != null)
      {
        foreach (Client clientData in Server.clients.Values)
        {
          if (clientData?.player != null)
          {
            Debug.Log($"spawn player {toClient.id} -> {clientData.player.username} [size: {spawnPositions.Count} index: {index}]");
            ServerSend.SpawnPlayer(toClient.id, clientData.player, spawnPositions[index] + Vector3.up);
            ++index;
          }
        }
      }
    }
  }



  public void GameOver(int winnerId, float time = 4f)
  {
    Debug.Log("game over");
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

    public void d() {
      Debug.Log("init prefab player");
      Instantiate(playerPrefab); // , position, ) // Quaternion.Euler(0.0f, orientationY, 0.0f));
    }


    public void SpawnPlayer(
    int id,
    string username,
    Color color,
    Vector3 position,
    float orientationY)
  {

    if (GameManager.players.ContainsKey(id)) {
      Debug.Log("player already exi " + id);
      return;
    }

    Debug.Log("Instantiate preab invoke!");
    Instantiate(playerPrefab); // , position, ) // Quaternion.Euler(0.0f, orientationY, 0.0f));
    Invoke("d", 3);


    /*
    Debug.Log("Spawning player");

    PlayerManager playerManager;
    if (id == LocalClient.instance.myId) // is me
    {
      Instantiate<GameObject>(playerPrefab, position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
      // GET LOCAL playerManager = PlayerMovement.Instance.gameObject.GetComponent<PlayerManager>();
    }
    else
    {
      playerManager = Instantiate<GameObject>(playerPrefab, position, Quaternion.Euler(0.0f, orientationY, 0.0f)).GetComponent<PlayerManager>();
      playerManager.SetDesiredPosition(position);
    }
    playerManager.SetDesiredPosition(position);
    playerManager.id = id;
    playerManager.username = username;
    playerManager.color = color;
    GameManager.players.Add(id, playerManager);

    if ((GameManager.gameSettings.gameMode != GameSettings.GameMode.Versus || id != LocalClient.instance.myId) && GameManager.gameSettings.gameMode == GameSettings.GameMode.Versus)
      return;
    this.extraUi.InitPlayerStatus(id, username, playerManager);
    */

  }

}


 public enum GameState {
        Lobby,
        Game,
        GameOver,
    }
