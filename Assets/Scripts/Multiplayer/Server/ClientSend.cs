using UnityEngine;
using Steamworks;
using System;

public class ClientSend : MonoBehaviour
{

  public static int packetsSent;
  public static int bytesSent;


  private static void SendTCPData(Packet packet)
  {
    ClientSend.bytesSent += packet.Length();
    ++ClientSend.packetsSent;
    packet.WriteLength();
    if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
      LocalClient.instance.tcp.SendData(packet);
    else
      SteamPacketManager.SendPacket((SteamId) LocalClient.instance.serverHost.Value, packet, P2PSend.Reliable, SteamPacketManager.NetworkChannel.ToServer);
  }
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

  public static void PingServer()
  {
    try
    {
      using (Packet packet = new Packet(6))
      {
        packet.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        ClientSend.SendUDPData(packet);
      }
    }
    catch (Exception ex)
    {
      Debug.Log(ex);
    }
  }


  public static void PlayerFinishedLoading()
  {
    using (Packet packet = new Packet(29))
      ClientSend.SendTCPData(packet);
  }

    public static void PlayerPing(Vector3 pos)
  {
    try
    {
      using (Packet packet = new Packet(24))
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

    public static void PlayerDisconnect()
  {
    try
    {
      using (Packet packet = new Packet(5))
        ClientSend.SendTCPData(packet);
    }
    catch (Exception ex)
    {
      Debug.Log((object) ex);
    }
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

    

  public static void WelcomeReceived(int id, string username)
  {
    using (Packet packet = new Packet(1))
    {
      packet.Write(id);
      packet.Write(username);
      Color blue = Color.blue;
      packet.Write(blue.r);
      packet.Write(blue.g);
      packet.Write(blue.b);
      ClientSend.SendTCPData(packet);
    }
  }
    
}
