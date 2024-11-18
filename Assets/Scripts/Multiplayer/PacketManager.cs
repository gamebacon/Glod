using UnityEngine;
using Steamworks;

public class PacketManager : MonoBehaviour
{

       void Awake()
    {
        // Check every 0.05 seconds for new packets
        InvokeRepeating("ReceiveDataPacket", 0f, 0.05f);
    }


    void ReceiveDataPacket()
    {
        while (SteamNetworking.IsP2PPacketAvailable())
        {
            var packet = SteamNetworking.ReadP2PPacket();
            if (packet.HasValue)
            {
                HandleOpponentDataPacket(packet.Value.Data);
            }
        }
    }


    void HandleOpponentDataPacket(byte[] data)  {
    }
    
}