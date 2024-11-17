using UnityEngine;
using System.Collections.Generic;

public class Server
{
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public static Dictionary<int, Server.PacketHandler> PacketHandlers;
    public delegate void PacketHandler(int fromClient, Packet packet);
    
}
