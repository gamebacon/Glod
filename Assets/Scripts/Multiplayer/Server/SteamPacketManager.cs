using Steamworks;
using Steamworks.Data;
using UnityEngine;


public class SteamPacketManager : MonoBehaviour
{
  private void Start()
  {
    DontDestroyOnLoad(gameObject);
    Server.InitializeServerPackets();
    LocalClient.InitializeClientData();

    Debug.Log("server and client packets inited");
  }

    private void Update()
  {
    SteamClient.RunCallbacks();
    CheckForPackets();
  }

  private void CheckForPackets()
  {
    for (int channel = 0; channel < 2; ++channel)
    {
      if (SteamNetworking.IsP2PPacketAvailable(channel))
      {
        while (SteamNetworking.IsP2PPacketAvailable(channel))
          SteamPacketManager.HandlePacket(SteamNetworking.ReadP2PPacket(channel), channel);
      }
    }
  }
    

    private static void HandlePacket(P2Packet? p2Packet, int channel)
  {

    if (!p2Packet.HasValue)
    {
      Debug.Log("No packet data");
      return;
    }

    SteamId steamId = p2Packet.Value.SteamId.Value;

    byte[] data = p2Packet.Value.Data;
    if (!LocalClient.serverOwner && steamId.Value != LocalClient.instance.serverHost.Value)
    {
      Debug.LogError($"Received packet from someone other than server: {new Friend(steamId).Name}\nDenying packet...");
    }
    else
    {
      Packet packet = new Packet();
      packet.SetBytes(data);
      if (packet.Length() != packet.ReadInt() + 4)  {
        Debug.LogError((object) "didnt read entire packet");
      }

      int key = packet.ReadInt();
      Debug.Log($"Get: {key}");

      /* 
        To client
      */
      if (channel == 0)
      {
        if (steamId.Value != LocalClient.instance.serverHost.Value) {
          return;
        }
        Debug.Log($"[To Client] [Receive] packet: {key}!");

        LocalClient.packetHandlers[key](packet);
      } else {
        /* 
          To Server
        */
        Debug.Log($"[To Server] packet: {key}!");
        Server.PacketHandlers[key](LobbyManager.steamIdToClientId[steamId.Value], packet);
      }
    }
  }

    public static void SendPacket(SteamId steamId, Packet p, P2PSend p2pSend, SteamPacketManager.NetworkChannel channel)
    {
        int length = p.Length();
        byte[] numArray = p.CloneBytes();
        Packet packet = new Packet(numArray);
        if (steamId.Value != SteamManager.instance.playerSteamId.Value) {
            /*
            Debug.Log($"[Send packet] to: {steamId.Value} size: {p.ReadInt(false)}");
            */
            Debug.Log("Send: " + p.ToString());
            SteamNetworking.SendP2PPacket(steamId.Value, numArray, length, (int) channel, p2pSend);
        }
        else
        SteamPacketManager.HandlePacket(
            new P2Packet?(new P2Packet()
            {
                SteamId = (SteamId) steamId.Value,
                Data = numArray
            }), (int) channel);
    }


  private void OnApplicationQuit() => SteamPacketManager.CloseConnections();

  public static void CloseConnections()
  {
    foreach (ulong key in LobbyManager.steamIdToClientId.Keys)
      SteamNetworking.CloseP2PSessionWithUser((SteamId) key);
    try
    {
      SteamNetworking.CloseP2PSessionWithUser(LocalClient.instance.serverHost);
    }
    catch
    {
      Debug.Log((object) "Failed to close p2p with host");
    }
    SteamClient.Shutdown();
  }

  public enum NetworkChannel
  {
    ToClient,
    ToServer,
  }
}
