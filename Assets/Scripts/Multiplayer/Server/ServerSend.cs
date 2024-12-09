using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public class ServerSend
{

    public static int test = 1337;
    private static P2PSend TCPvariant = P2PSend.Reliable;
    private static P2PSend UDPVariant = P2PSend.Unreliable;
    private static void SendTCPData(int toClient, Packet p)


  {
    Debug.Log($"TCP Send {p}");
    Packet packet = new Packet();
    packet.SetBytes(p.CloneBytes());
    packet.WriteLength();
    SteamPacketManager.SendPacket(Server.clients[toClient].player.steamId.Value, packet, ServerSend.TCPvariant, SteamPacketManager.NetworkChannel.ToClient);
  }


  public static void DisconnectPlayer(int player)
  {
    using (Packet packet = new Packet(5))
    {
      packet.Write(player);
      ServerSend.SendTCPDataToAll(packet);
    }
  }

  public static void Welcome(int toClient, string msg)
  {
    using (Packet packet = new Packet(1))
    {
      packet.Write(msg);
      packet.Write(NetworkManager.Clock);
      packet.Write(toClient);
      ServerSend.SendTCPData(toClient, packet);
    }
  }

  public static void PlayerReady(int fromClient, bool ready)
  {
    using (Packet packet = new Packet(16))
    {
      packet.Write(fromClient);
      packet.Write(ready);
      ServerSend.SendTCPDataToAll(packet);
    }
  }


  public static void PlayerPosition(Player player, int t)
  {
    using (Packet packet = new Packet(3))
    {
      packet.Write(player.id);
      packet.Write(player.pos);
      ServerSend.SendUDPDataToAll(player.id, packet);
    }
  }

  public static void PlayerRotation(Player player)
  {
    using (Packet packet = new Packet(4))
    {
      packet.Write(player.id);
      packet.Write(player.yOrientation);
      packet.Write(player.xOrientation);
      ServerSend.SendUDPDataToAll(player.id, packet);
    }
  }
  
  public static void PingPlayer(int player, string ms)
  {
    using (Packet packet = new Packet(8))
    {
      packet.Write(player);
      packet.Write(ms);
      ServerSend.SendUDPData(player, packet);
    }
  }

  public static void SendPing(int fromClient, Vector3 pos, string username)
  {
    using (Packet packet = new Packet(41))
    {
      packet.Write(pos);
      packet.Write(username);
      ServerSend.SendUDPDataToAll(fromClient, packet);
    }
  }


  public static void PlayerReady(int fromClient, bool ready, int toClient)
  {
    using (Packet packet = new Packet(16))
    {
      packet.Write(fromClient);
      packet.Write(ready);
      ServerSend.SendTCPData(toClient, packet);
    }
  }

    public static void SpawnPlayer(int toClient, Player player, Vector3 pos)
  {
    using (Packet packet = new Packet(2))
    {
      Debug.Log($"spawning player, id: {player.id} sending to {toClient}");
      packet.Write(player.id);
      packet.Write(player.username);

      Vector3 vector3 = new Vector3(player.color.r, player.color.g, player.color.b);

      packet.Write(vector3);
      player.pos = pos;
      packet.Write(pos);
      packet.Write(player.yOrientation);
      ServerSend.SendTCPData(toClient, packet);
    }
  }


  public static void SendChatMessage(int fromClient, string username, string msg)
  {
    using (Packet packet = new Packet(40))
    {
      packet.Write(fromClient);
      packet.Write(username);
      packet.Write(msg);
      ServerSend.SendUDPDataToAll(fromClient, packet);
    }
  }


    public static void PlayerFinishedLoading(int playerId)
  {
    using (Packet packet = new Packet(51))
    {
      packet.Write(playerId);
      ServerSend.SendTCPDataToAll(packet);
    }
  }


  public static void StartGame(int playerLobbyId)
  {
    using (Packet packet = new Packet(13))
    {
      packet.Write(playerLobbyId); // lobbyid
      packet.Write(123); // seed
      packet.Write(123); // gamemode
      packet.Write(123); // firnedly fire
      packet.Write(123); // difficulty
      packet.Write(123); // gmaelength
      packet.Write(123); // multiplayer
      List<Player> playerList = new List<Player>();
      for (int key = 0; key < Server.clients.Values.Count; ++key)
      {
        if (Server.clients[key] != null && Server.clients[key].player != null)
          playerList.Add(Server.clients[key].player);
      }
      packet.Write(playerList.Count); // player ocunt
      foreach (Player player in playerList)
      {
        packet.Write(player.id);
        packet.Write(player.username);
      }
      ServerSend.SendTCPData(playerLobbyId, packet);
    }
  }
  /*
    public static void StartGame(int playerLobbyId)
  {
    Debug.Log("Sending start packet to " + playerLobbyId);
    using (Packet packet = new Packet(13))
    {
      packet.Write(playerLobbyId);
      List<Player> playerList = new List<Player>();
      for (int key = 0; key < Server.clients.Values.Count; ++key)
      {
        if (Server.clients[key] != null && Server.clients[key].player != null) {
            playerList.Add(Server.clients[key].player);
        }
      }
      packet.Write(playerList.Count);
      foreach (Player player in playerList)
      {
        packet.Write(player.id);
        packet.Write(player.username);
      }
      Debug.Log("Sending start game packet");
      ServerSend.SendTCPData(playerLobbyId, packet);
    }
  }
  */


  public static void ConnectionSuccessful(int toClient)
  {
    using (Packet packet = new Packet(9))
      ServerSend.SendTCPData(toClient, packet);
  }



  private static void SendTCPDataToAll(Packet packet)
  {
    packet.WriteLength();
    if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
    {
      for (int key = 1; key < Server.MaxPlayers; ++key)
        Server.clients[key].tcp.SendData(packet);
    }
    else
    {
      foreach (Client client in Server.clients.Values)
      {
        if (client?.player != null)
          SteamPacketManager.SendPacket((SteamId) client.player.steamId.Value, packet, ServerSend.TCPvariant, SteamPacketManager.NetworkChannel.ToClient);
      }
    }
  }

  private static void SendTCPDataToAll(int exceptClient, Packet packet)
  {
    packet.WriteLength();
    if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
    {
      for (int key = 1; key < Server.MaxPlayers; ++key)
      {
        if (key != exceptClient)
          Server.clients[key].tcp.SendData(packet);
      }
    }
    else
    {
      foreach (Client client in Server.clients.Values)
      {
        if (client?.player != null && LobbyManager.steamIdToClientId[client.player.steamId.Value] != exceptClient)
          SteamPacketManager.SendPacket((SteamId) client.player.steamId.Value, packet, ServerSend.TCPvariant, SteamPacketManager.NetworkChannel.ToClient);
      }
    }
  }

  public static void SendTCPDataToSteamId(SteamId steamId, Packet packet)
  {
    Packet p = new Packet();
    p.SetBytes(packet.CloneBytes());
    p.WriteLength();
    SteamPacketManager.SendPacket(steamId, p, ServerSend.TCPvariant, SteamPacketManager.NetworkChannel.ToClient);
  }

  private static void SendTCPDataToAll(int[] exceptClients, Packet packet)
  {
    packet.WriteLength();
    if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
    {
      for (int key = 1; key < Server.MaxPlayers; ++key)
      {
        bool flag = false;
        foreach (int exceptClient in exceptClients)
        {
          if (key == exceptClient)
            flag = true;
        }
        if (!flag)
          Server.clients[key].tcp.SendData(packet);
      }
    }
    else
    {
      foreach (Client client in Server.clients.Values)
      {
        if (client?.player != null)
        {
          bool flag = false;
          foreach (int exceptClient in exceptClients)
          {
            if (LobbyManager.steamIdToClientId[client.player.steamId.Value] == exceptClient)
              flag = true;
          }
          if (!flag)
            SteamPacketManager.SendPacket((SteamId) client.player.steamId.Value, packet, ServerSend.TCPvariant, SteamPacketManager.NetworkChannel.ToClient);
        }
      }
    }
  }


  private static void SendUDPData(int toClient, Packet packet)
  {
    Packet packet1 = new Packet();
    packet1.SetBytes(packet.CloneBytes());
    packet1.WriteLength();
    if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
      Server.clients[toClient].udp.SendData(packet1);
    else
      SteamPacketManager.SendPacket((SteamId) Server.clients[toClient].player.steamId.Value, packet1, ServerSend.UDPVariant, SteamPacketManager.NetworkChannel.ToClient);
  }

  private static void SendUDPDataToAll(Packet packet)
  {
    packet.WriteLength();
    if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
    {
      for (int key = 1; key < Server.MaxPlayers; ++key)
        Server.clients[key].udp.SendData(packet);
    }
    else
    {
      foreach (Client client in Server.clients.Values)
      {
        if (client?.player != null)
          SteamPacketManager.SendPacket((SteamId) client.player.steamId.Value, packet, ServerSend.UDPVariant, SteamPacketManager.NetworkChannel.ToClient);
      }
    }
  }

  private static void SendUDPDataToAll(int exceptClient, Packet packet)
  {
    packet.WriteLength();
    if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
    {
      for (int key = 1; key < Server.MaxPlayers; ++key)
      {
        if (key != exceptClient)
          Server.clients[key].udp.SendData(packet);
      }
    }
    else
    {
      foreach (Client client in Server.clients.Values)
      {
        if (client?.player != null && LobbyManager.steamIdToClientId[client.player.steamId.Value] != exceptClient)
          SteamPacketManager.SendPacket((SteamId) client.player.steamId.Value, packet, ServerSend.UDPVariant, SteamPacketManager.NetworkChannel.ToClient);
      }
    }
  }

    internal static void HitEntity(int fromClient, int entityId)
    {

        using (Packet packet = new Packet(11))
        {
          packet.Write(fromClient);
          packet.Write(entityId);
          ServerSend.SendTCPDataToAll(packet);
        }
    }
}
