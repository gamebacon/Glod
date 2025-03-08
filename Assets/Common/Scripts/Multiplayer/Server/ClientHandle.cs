using System;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
  public static void Welcome(Packet packet)
  {
    string str = packet.ReadString();
    double num = (double) packet.ReadFloat();
    int id = packet.ReadInt();
    Debug.Log((object) ("Message from server: " + str));
    UiManager.instance.ConnectionSuccessful();
    LocalClient.instance.myId = id;
    ClientSend.WelcomeReceived(id, LocalClient.instance.name);
    if (NetworkController.Instance.networkType != NetworkController.NetworkType.Classic) {
      return;
    }
    LocalClient.instance.udp.Connect(((IPEndPoint) LocalClient.instance.tcp.socket.Client.LocalEndPoint).Port);

    Console.AddMessage("CLIENT welcome");
  }

  public static void PlayerPosition(Packet packet)
  {
    int key = packet.ReadInt();
    Vector3 position = packet.ReadVector3();
    if (!GameManager.players.ContainsKey(key)) {
      Debug.Log("not find key plaer pos " + key);
      return;
    }
    GameManager.players[key].SetDesiredPosition(position);
  }

  public static void PlayerRotation(Packet packet)
  {
    int key = packet.ReadInt();
    if (!GameManager.players.ContainsKey(key))
      return;
    float orientationY = packet.ReadFloat();
    float orientationX = packet.ReadFloat();
    GameManager.players[key].SetDesiredRotation(orientationY, orientationX);
  }

  public static void Clock(Packet packet)
  {
    int index = packet.ReadInt();
    // LoadingScreen.Instance.players[index] = true;
  }

  // public static void PlayerFinishedLoading(Packet packet) => LoadingScreen.Instance.UpdateStatuses(packet.ReadInt());

// ??????????????
  public static void SpawnPlayer(Packet packet)
  {

    Console.AddMessage("CLIENT spawning player");

    int id = packet.ReadInt();
    string username = packet.ReadString();
    Vector3 vector3 = packet.ReadVector3();
    Vector3 position = packet.ReadVector3();
    float orientationY = packet.ReadFloat();
    GameManager.instance.SpawnPlayer(id, username, new Color(vector3.x, vector3.y, vector3.z), position, orientationY);
    GameManager.instance.StartGame();
  }

/*
  public static void StartGame(Packet packet)
  {
    // if (NetworkController.Instance.loading) {
      // return;
    // }

    LocalClient.instance.myId = packet.ReadInt(); // 1
    int playerCount = packet.ReadInt(); // 2


    // GameManager.gameSettings = new GameSettings(seed, gameMode, friendlyFire, difficulty, gameLength, multiplayer);

    // Console.AddMessage((object) "Game settings successfully loaded");
    // Console.AddMessage((object) ("loading game scene, assigned id: " + (object) LocalClient.instance.myId));

    NetworkController.Instance.nPlayers = playerCount;

    string[] names = new string[playerCount];
    for (int index = 0; index < playerCount; ++index)
    {
      packet.ReadInt();
      string str = packet.ReadString();
      names[index] = str;
    }

    NetworkController.Instance.LoadGame(names);
    // ClientSend.StartedLoading();
  }
  */  
  public static void StartGame(Packet packet)
  {
    /*
    if (NetworkController.Instance.loading)
      return;
    */

    LocalClient.instance.myId = packet.ReadInt();

    int seed = packet.ReadInt();
    int num1 = packet.ReadInt();
    int num2 = packet.ReadInt();
    int num3 = packet.ReadInt();
    int num4 = packet.ReadInt();
    int num5 = packet.ReadInt();
    int gameMode = num1;
    int friendlyFire = num2;
    int difficulty = num3;
    int gameLength = num4;
    int multiplayer = num5;

    GameManager.instance.gameSettings = new GameSettings(seed);

    /*
    Console.AddMessage((object) "Game settings successfully loaded");
    Console.AddMessage((object) ("loading game scene, assigned id: " + (object) LocalClient.instance.myId));
    */

    int length = packet.ReadInt();
    NetworkController.Instance.nPlayers = length;
    string[] names = new string[length];
    for (int index = 0; index < length; ++index)
    {
      packet.ReadInt();
      string str = packet.ReadString();
      names[index] = str;
    }
    NetworkController.Instance.LoadGame(names);
    // ClientSend.StartedLoading();
  }

  public static void ReceivePing(Packet packet)
  {
    packet.ReadInt();
    NetStatus.AddPing((int) (DateTime.Now - DateTime.Parse(packet.ReadString())).TotalMilliseconds);
  }

  public static void ReceiveStatus(Packet packet) => Console.AddMessage("received status");

  public static void ConnectionEstablished(Packet packet)
  {
    Console.AddMessage("connection has successfully been established. ready to enter game");
    GameManager.isConnected = true;
  }

/*
  public static void PlayerDied(Packet packet)
  {
    int id = packet.ReadInt();
    Vector3 pos = packet.ReadVector3();
    int num = packet.ReadInt();
    GameManager.instance.KillPlayer(id, pos);
    if (LocalClient.instance.myId != num || LocalClient.instance.myId == id || GameManager.gameSettings.gameMode != GameSettings.GameMode.Survival)
      return;
    AchievementManager.Instance.AddPlayerKill();
  }
  */
  public static void PlayerFinishedLoading(Packet packet) {
    Console.AddMessage("CLIENT fin loading");
   // LoadingScreen.Instance.UpdateStatuses(packet.ReadInt());
  }

  public static void Ready(Packet packet)
  {
    packet.ReadInt();
    packet.ReadBool();
  }

  public static void KickPlayer(Packet packet)
  {
    LobbyManager.instance.LeaveLobby();

    if (!GameManager.instance) {
      return;
    }

    GameManager.instance.LeaveGame();
  }

  public static void DisconnectPlayer(Packet packet)
  {
    int id = packet.ReadInt();

    Debug.Log(string.Format("Player {0} has disconnected", (object) id));

    if (id == LocalClient.instance.myId)
    {
      LobbyManager.instance.LeaveLobby();
      if (!GameManager.instance) {
        return;
      }

      GameManager.instance.LeaveGame();
    }
    else
    {
      if (!(bool) (UnityEngine.Object) GameManager.instance)
        return;
      GameManager.instance.DisconnectPlayer(id);
    }
  }
/*

  public static void ReceiveChatMessage(Packet packet)
  {
    int fromUser = packet.ReadInt();
    string fromUsername = packet.ReadString();
    string message = packet.ReadString();
    ChatBox.Instance.AppendMessage(fromUser, message, fromUsername);
  }
  */

  public static void ReceivePlayerPing(Packet packet)
  {
    Vector3 pos = packet.ReadVector3();
    string name = packet.ReadString();
    PingController.Instance.MakePing(pos, name, "");
  }

  public static void GameOver(Packet packet)
  {
    int winnerId = packet.ReadInt();
    GameManager.instance.GameOver(winnerId);
  }

  public static void EntityHit(Packet packet)
  {
    int sourceId = packet.ReadInt();
    int entityId = packet.ReadInt();
    Debug.Log($"[ClientHandle] {sourceId} hit {entityId}");
    ObjectManager.instance.Damage(10, entityId);
  }

}