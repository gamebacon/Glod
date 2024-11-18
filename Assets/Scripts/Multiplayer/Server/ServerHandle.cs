using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
  public static void WelcomeReceived(int fromClient, Packet packet)
  {
    int num = packet.ReadInt();
    string playerName = packet.ReadString();
    Color color = new Color(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
    if (fromClient != num)
      Debug.Log((object) "Something went very wrong in ServerHandle");
    if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
    {
      Debug.Log((object) string.Format("{0} connected successfully and is now player {1}.", (object) Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint, (object) fromClient));
      Server.clients[fromClient].StartClient(playerName, color);
    }
    ServerSend.ConnectionSuccessful(fromClient);
    Server.clients[fromClient].SendIntoGame();
  }

  public static void JoinRequest(int fromClient, Packet packet)
  {
    if (Server.clients[fromClient].player.hasJoined)
    {
      Debug.LogError((object) ("Player already joined: " + (object) fromClient));
    }
    else
    {
      Server.clients[fromClient].SendIntoGame();
      return; // ??????
      Debug.Log(packet.ToString());

      // worng data? (removing id)
      int playerId = packet.ReadInt();

      Debug.LogError((object) ("Player wants to join, id: " + (object) fromClient));
      Server.clients[fromClient].player.hasJoined = true;
      Server.clients[fromClient].player.username = packet.ReadString();
      ServerSend.Welcome(fromClient, "weclome");
    }
  }



  public static void PlayerFinishedLoading(int fromClient, Packet packet)
  {
    Debug.Log((object) ("Player finished loading: " + (object) fromClient));
    Server.clients[fromClient].player.isReady = true;
    ServerSend.PlayerFinishedLoading(fromClient);
    int num = 0;
    int nPlayers = 0;
    foreach (Client client in Server.clients.Values)
    {
      if (client?.player != null)
      {
        ++nPlayers;
        if (client.player.isReady)
          ++num;
      }
    }
    if (num < nPlayers)
      return;
    Debug.Log((object) ("ready players: " + (object) num + " / " + (object) nPlayers));
    List<Vector3> spawnPositions = /* GameManager.gameSettings.gameMode == GameSettings.GameMode.Versus ? GameManager.instance.FindVersusSpawnPositions(nPlayers) : */ GameManager.instance.FindSurvivalSpawnPositions(nPlayers);
    if (num < nPlayers)
      return;
    GameManager.instance.SendPlayersIntoGame(nPlayers);
  }

  public static void PlayerDisconnect(int fromClient, Packet packet)
  {
    if (Server.clients[fromClient].player == null)
      return;
    ServerHandle.DisconnectPlayer(fromClient);
  }

  public static void DisconnectPlayer(int fromClient)
  {
    ServerSend.DisconnectPlayer(fromClient);
    try
    {
      ServerSend.SendChatMessage(-1, "Server", Server.clients[fromClient].player.username + " disconnected");
    }
    catch
    {
      Debug.LogError((object) "Failed to send disconnect message to clients");
    }
    Server.clients[fromClient] = (Client) null;
  }

  public static void SpawnPlayersRequest(int fromClient, Packet packet)
  {
    Debug.Log((object) "received request to spawn players");
    if (Server.clients[fromClient].player == null)
      return;
    Server.clients[fromClient].SendIntoGame();
  }

  public static void PlayerRequestedSpawns(int fromClient, Packet packet) => Debug.LogError((object) "Player requested spawns, but method is not implemented");

  public static void StartedLoading(int fromClient, Packet packet)
  {
    if (Server.clients[fromClient].player.isLoading)
      return;
    Server.clients[fromClient].player.isLoading = true;
  }

  public static void Ready(int fromClient, Packet packet)
  {
    if (Server.clients[fromClient].player == null)
      return;
    Debug.Log((object) ("Recevied ready from player: " + (object) fromClient));
    bool ready = packet.ReadBool();
    ServerSend.PlayerReady(fromClient, ready);
    Server.clients[fromClient].player.isReady = ready;
  }

  public static void PingReceived(int fromClient, Packet packet)
  {
    if (Server.clients[fromClient].player == null)
      return;
    string ms = packet.ReadString();
    Server.clients[fromClient].player.PingPlayer();
    ServerSend.PingPlayer(fromClient, ms);
  }

  public static void ReceiveChatMessage(int fromClient, Packet packet)
  {
    if (Server.clients[fromClient].player == null)
      return;
    string msg = packet.ReadString();
    string username = GameManager.players[fromClient].username;
    ServerSend.SendChatMessage(fromClient, username, msg);
  }

  public static void StartGameTest(int fromClient, Packet packet)
  {
    if (Server.clients[fromClient].player == null)
      return;

      Debug.Log("Test start game serv handle!");
  }

  public static void LoadingFinTest(int fromClient, Packet packet)
  {
    if (Server.clients[fromClient].player == null)
      return;

      Debug.Log("Test loading fin serv handle!");
  }

  public static void ReceivePing(int fromClient, Packet packet)
  {
    if (Server.clients[fromClient].player == null)
      return;
    Vector3 pos = packet.ReadVector3();
    string username = GameManager.players[fromClient].username;
    ServerSend.SendPing(fromClient, pos, username);
  }

}