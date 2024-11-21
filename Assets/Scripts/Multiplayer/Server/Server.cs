using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
public class Server
{
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public static Dictionary<int, Server.PacketHandler> PacketHandlers;

  public static IPAddress ipAddress = IPAddress.Any;
   public static int Port { get; private set; }
     private static TcpListener tcpListener;
     private static UdpClient udpListener;

    public static int MaxPlayers { get; private set; }

    public delegate void PacketHandler(int fromClient, Packet packet);

    public static void Start(int maxPlayers, int port)
  {
    Server.MaxPlayers = maxPlayers;
    Server.Port = port;
    Debug.Log("Starting server.. ver.0.7");
    Server.InitializeServerData();
    Server.tcpListener = new TcpListener(Server.ipAddress, Server.Port);
    Debug.Log($"TclpListener on IP: {Server.ipAddress}");
    Server.tcpListener.Start();
    Server.tcpListener.BeginAcceptTcpClient(new AsyncCallback(Server.TCPConnectCallback), null);
    Server.udpListener = new UdpClient(Server.Port);
    Server.udpListener.BeginReceive(new AsyncCallback(Server.UDPReceiveCallback), null);
    Debug.Log("Server started on port:" + Server.Port);
    ThreadManagerServer.Instance.ResetGame();
  }


  private static void TCPConnectCallback(IAsyncResult result)
  {
    TcpClient socket = Server.tcpListener.EndAcceptTcpClient(result);
    Server.tcpListener.BeginAcceptTcpClient(new AsyncCallback(Server.TCPConnectCallback), (object) null);
    Debug.Log((object) string.Format("Incoming connection from {0}...", (object) socket.Client.RemoteEndPoint));
    for (int key = 1; key <= Server.MaxPlayers; ++key)
    {
      if (Server.clients[key].tcp.socket == null)
      {
        Server.clients[key].tcp.Connect(socket);
        return;
      }
    }
    Debug.Log((object) string.Format("{0} failed to connect: Server full! f", (object) socket.Client.RemoteEndPoint));
  }



  private static void UDPReceiveCallback(IAsyncResult result)
  {
    try
    {
      IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
      byte[] _data = Server.udpListener.EndReceive(result, ref remoteEP);
      Server.udpListener.BeginReceive(new AsyncCallback(Server.UDPReceiveCallback), (object) null);
      if (_data.Length < 4)
        return;
      using (Packet packetData = new Packet(_data))
      {
        int key = packetData.ReadInt();
        if (key == 0)
          return;
        if (Server.clients[key].udp.endPoint == null)
        {
          Server.clients[key].udp.Connect(remoteEP);
        }
        else
        {
          if (!(Server.clients[key].udp.endPoint.ToString() == remoteEP.ToString()))
            return;
          Server.clients[key].udp.HandleData(packetData);
        }
      }
    }
    catch (Exception ex)
    {
      Debug.Log((object) string.Format("Catching error receiving UDP data: {0}", (object) ex));
      Debug.Log((object) "This error message can be ignored if just closing server. Server has been closed successfully.");
    }
  }


  public static void InitializeServerPackets() => Server.PacketHandlers = new Dictionary<int, Server.PacketHandler>()
  {
    {
      3,
      new Server.PacketHandler(ServerHandle.PlayerPosition)
    },
    {
      13,
      new Server.PacketHandler(ServerHandle.StartGameTest)
    },
    {
      51,
      new Server.PacketHandler(ServerHandle.LoadingFinTest)
    },
    {
      1,
      new Server.PacketHandler(ServerHandle.WelcomeReceived)
    },
    {
      2,
      new Server.PacketHandler(ServerHandle.JoinRequest)
    },
    {
      33,
      new Server.PacketHandler(ServerHandle.StartedLoading)
    },
    {
      29,
      new Server.PacketHandler(ServerHandle.PlayerFinishedLoading)
    },
    {
      5,
      new Server.PacketHandler(ServerHandle.PlayerDisconnect)
    },
    {
      6,
      new Server.PacketHandler(ServerHandle.PingReceived)
    },
    {
      9,
      new Server.PacketHandler(ServerHandle.PlayerRequestedSpawns)
    },
    {
      8,
      new Server.PacketHandler(ServerHandle.Ready)
    },
    {
      23,
      new Server.PacketHandler(ServerHandle.ReceiveChatMessage)
    },
    {
      24,
      new Server.PacketHandler(ServerHandle.ReceivePing)
    },
  };

  public static void InitializeServerData()
  {
    for (int index = 1; index <= Server.MaxPlayers; ++index) {
      Server.clients.Add(index, new Client(index));
    }

    Server.InitializeServerPackets();
    Debug.Log("Initialized Packets.");
  }
  public static void Stop()
  {
    Server.tcpListener.Stop();
    Server.udpListener.Close();
  }

    public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
  {
    try
    {
      if (clientEndPoint == null) {
        return;
      }
      Server.udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
    }
    catch (Exception ex)
    {
      Debug.Log(string.Format("Error sending data to {0} via UDP: {1}.", clientEndPoint, ex));
    }
  }
    
}
