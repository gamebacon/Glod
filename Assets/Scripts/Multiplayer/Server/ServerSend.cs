using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class ServerSend
{

    private static P2PSend TCPvariant = P2PSend.Reliable;
    private static void SendTCPData(int toClient, Packet p)
  {
    Packet packet = new Packet();
    packet.SetBytes(p.CloneBytes());
    packet.WriteLength();
    SteamPacketManager.SendPacket(Server.clients[toClient].player.steamId.Value, packet, ServerSend.TCPvariant, SteamPacketManager.NetworkChannel.ToClient);
  }
    
    public static void StartGame(int playerLobbyId)
  {
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
}
