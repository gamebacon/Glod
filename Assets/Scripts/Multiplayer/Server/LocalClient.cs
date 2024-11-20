using Steamworks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class LocalClient : MonoBehaviour
{

    public static LocalClient instance;
      public static int dataBufferSize = 4096;

      public static bool serverOwner;
    public SteamId serverHost;
    public static Dictionary<int, LocalClient.PacketHandler> packetHandlers;
    public delegate void PacketHandler(Packet packet);

     public int myId;

    public LocalClient.TCP tcp;
    public LocalClient.UDP udp;
    private bool isConnected;

      public static int byteDown;

      public static int packetsReceived;


    // ???
    public string ip = "127.0.0.1";
      public int port = 26950;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else
        {
        if (!LocalClient.instance != this)
        {
            return;
        }

        Debug.Log("Instance already exists, destroying object");
        Destroy(this);
        }
    }
  private void Start() => this.StartProtocols();

  private void StartProtocols()
  {
    this.tcp = new LocalClient.TCP();
    this.udp = new LocalClient.UDP();
  }

      public static void InitializeClientData()
  {
    LocalClient.packetHandlers = new Dictionary<int, LocalClient.PacketHandler>()
    {
      {
        1,
        new LocalClient.PacketHandler(ClientHandle.Welcome)
      },
      {
        2,
        new LocalClient.PacketHandler(ClientHandle.SpawnPlayer)
      },
      {
        8,
        new LocalClient.PacketHandler(ClientHandle.ReceivePing)
      },
      {
        11,
        new LocalClient.PacketHandler(ClientHandle.ReceiveStatus)
      },
      {
        14,
        new LocalClient.PacketHandler(ClientHandle.Clock)
      },
      {
        51,
        new LocalClient.PacketHandler(ClientHandle.PlayerFinishedLoading)
      },
      {
        12,
        new LocalClient.PacketHandler(ClientHandle.GameOver)
      },
      {
        5,
        new LocalClient.PacketHandler(ClientHandle.DisconnectPlayer)
      },
      {
        16,
        new LocalClient.PacketHandler(ClientHandle.Ready)
      },
      {
        13,
        new LocalClient.PacketHandler(ClientHandle.StartGame)
      },
      /*
      {
        40,
        new LocalClient.PacketHandler(ClientHandle.ReceiveChatMessage)
      },
      */
      {
        41,
        new LocalClient.PacketHandler(ClientHandle.ReceivePlayerPing)
      },
    };
    Debug.Log((object) "Initializing packets.");
  }



      public void ConnectToServer(string ip, string username)
  {
        this.ip = ip;
        this.StartProtocols();
        LocalClient.InitializeClientData();
        this.isConnected = true;
        this.tcp.Connect();
  }





  public void Disconnect()
  {
    if (isConnected) {
      return;
    }

    ClientSend.PlayerDisconnect();

    isConnected = false;
    tcp.socket.Close();
    udp.socket.Close();
    Debug.Log("Disconnected from server.");
  }






  public class TCP
  {
    public TcpClient socket;
    private NetworkStream stream;
    private Packet receivedData;
    private byte[] receiveBuffer;

    public void Connect()
    {
      this.socket = new TcpClient()
      {
        ReceiveBufferSize = LocalClient.dataBufferSize,
        SendBufferSize = LocalClient.dataBufferSize
      };
      this.receiveBuffer = new byte[LocalClient.dataBufferSize];
      this.socket.BeginConnect(LocalClient.instance.ip, LocalClient.instance.port, new AsyncCallback(this.ConnectCallback), (object) this.socket);
    }

    private void ConnectCallback(IAsyncResult result)
    {
      this.socket.EndConnect(result);
      if (!this.socket.Connected)
        return;
      this.stream = this.socket.GetStream();
      this.receivedData = new Packet();
      this.stream.BeginRead(this.receiveBuffer, 0, LocalClient.dataBufferSize, new AsyncCallback(this.ReceiveCallback), (object) null);
    }

    public void SendData(Packet packet)
    {
      try
      {
        if (this.socket == null)
          return;
        this.stream.BeginWrite(packet.ToArray(), 0, packet.Length(), (AsyncCallback) null, (object) null);
      }
      catch (Exception ex)
      {
        Debug.Log((object) string.Format("Error sending data to server via TCP: {0}", (object) ex));
      }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
      try
      {
        int length = this.stream.EndRead(result);
        if (length <= 0)
        {
          LocalClient.instance.Disconnect();
        }
        else
        {
          byte[] data = new byte[length];
          Array.Copy((Array) this.receiveBuffer, (Array) data, length);
          this.receivedData.Reset(this.HandleData(data));
          this.stream.BeginRead(this.receiveBuffer, 0, LocalClient.dataBufferSize, new AsyncCallback(this.ReceiveCallback), (object) null);
        }
      }
      catch
      {
        this.Disconnect();
      }
    }

    private bool HandleData(byte[] data)
    {
      ++LocalClient.packetsReceived;
      int packetLength = 0;
      this.receivedData.SetBytes(data);
      if (this.receivedData.UnreadLength() >= 4)
      {
        packetLength = this.receivedData.ReadInt();
        if (packetLength <= 0)
          return true;
      }
      while (packetLength > 0 && packetLength <= this.receivedData.UnreadLength())
      {
        byte[] packetBytes = this.receivedData.ReadBytes(packetLength);
        ThreadManagerClient.ExecuteOnMainThread((Action) (() =>
        {
          using (Packet packet = new Packet(packetBytes))
          {
            int key = packet.ReadInt();
            LocalClient.byteDown += packetLength;
            Debug.Log("received packet: " + key);
            LocalClient.packetHandlers[key](packet);
          }
        }));
        packetLength = 0;
        if (this.receivedData.UnreadLength() >= 4)
        {
          packetLength = this.receivedData.ReadInt();
          if (packetLength <= 0)
            return true;
        }
      }
      return packetLength <= 1;
    }

    private void Disconnect()
    {
      LocalClient.instance.Disconnect();
      this.stream = (NetworkStream) null;
      this.receivedData = (Packet) null;
      this.receiveBuffer = (byte[]) null;
      this.socket = (TcpClient) null;
    }
  }

  public class UDP
  {
    public UdpClient socket;
    public IPEndPoint endPoint;

    public UDP() => this.endPoint = new IPEndPoint(IPAddress.Parse(LocalClient.instance.ip), LocalClient.instance.port);

    public void Connect(int localPort)
    {
      this.socket = new UdpClient(localPort);
      this.socket.Connect(this.endPoint);
      this.socket.BeginReceive(new AsyncCallback(this.ReceiveCallback), (object) null);
      using (Packet packet = new Packet())
        this.SendData(packet);
    }

    public void SendData(Packet packet)
    {
      try
      {
        packet.InsertInt(LocalClient.instance.myId);
        if (this.socket == null)
          return;
        this.socket.BeginSend(packet.ToArray(), packet.Length(), (AsyncCallback) null, (object) null);
      }
      catch (Exception ex)
      {
        Debug.Log((object) string.Format("Error sending data to server via UDP: {0}", (object) ex));
      }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
      try
      {
        byte[] data = this.socket.EndReceive(result, ref this.endPoint);
        this.socket.BeginReceive(new AsyncCallback(this.ReceiveCallback), (object) null);
        if (data.Length < 4)
        {
          LocalClient.instance.Disconnect();
          Debug.Log((object) "UDP failed due to packets being split, in Client class");
        }
        else
          this.HandleData(data);
      }
      catch
      {
        this.Disconnect();
      }
    }

    private void HandleData(byte[] data)
    {
      ++LocalClient.packetsReceived;
      using (Packet packet = new Packet(data))
      {
        int _length = packet.ReadInt();
        LocalClient.byteDown += _length;
        data = packet.ReadBytes(_length);
      }
      ThreadManagerClient.ExecuteOnMainThread((Action) (() =>
      {
        using (Packet packet = new Packet(data))
        {
          int key = packet.ReadInt();
          LocalClient.packetHandlers[key](packet);
        }
      }));
    }

    private void Disconnect()
    {
      LocalClient.instance.Disconnect();
      this.endPoint = (IPEndPoint) null;
      this.socket = (UdpClient) null;
    }
  }
}
