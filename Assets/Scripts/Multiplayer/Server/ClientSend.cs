using UnityEngine;
using Steamworks;
using System;

public class ClientSend : MonoBehaviour
{

  public static int packetsSent;
  public static int bytesSent;


  private static void SendUDPData(Packet packet)
  {
    ClientSend.bytesSent += packet.Length();
    ++ClientSend.packetsSent;
    packet.WriteLength();

    SteamPacketManager.SendPacket(
        LocalClient.instance.serverHost.Value,
        packet,
        P2PSend.Unreliable,
        SteamPacketManager.NetworkChannel.ToServer
    );

  }

    
  public static void PlayerPosition(Vector3 pos)
  {
    try
    {
      using (Packet packet = new Packet(3))
      {
        packet.Write(pos);
        ClientSend.SendUDPData(packet);
      }
    }
    catch (Exception ex)
    {
      Debug.Log((object) ex);
    }
  }
    
}
