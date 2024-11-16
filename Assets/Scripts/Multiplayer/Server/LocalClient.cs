using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LocalClient : MonoBehaviour
{

    public static LocalClient instance;

    public static bool isServerOwner;
    public SteamId serverHost;
    public static Dictionary<int, LocalClient.PacketHandler> packetHandlers;
    public delegate void PacketHandler(Packet packet);



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
}
