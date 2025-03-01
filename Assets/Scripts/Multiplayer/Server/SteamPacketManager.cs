using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class SteamPacketManager : MonoBehaviour
{
    private void Start()
    {
        Server.InitializeServerPackets();
        LocalClient.InitializeClientData();
    }

    private void Update()
    {
        SteamClient.RunCallbacks();
        CheckForPackets();
    }

    private void CheckForPackets()
    {
        foreach (NetworkChannel channel in System.Enum.GetValues(typeof(NetworkChannel)))
        {
            while (SteamNetworking.IsP2PPacketAvailable((int)channel))
            {
                HandlePacket(SteamNetworking.ReadP2PPacket((int)channel), channel);
            }
        }
    }

    private static void HandlePacket(P2Packet? p2Packet, NetworkChannel channel)
    {
        if (!p2Packet.HasValue)
        {
            Debug.LogWarning("No packet data received.");
            return;
        }

        SteamId senderSteamId = p2Packet.Value.SteamId.Value;
        byte[] data = p2Packet.Value.Data;

        // Validate sender if client isn't server owner
        if (!LocalClient.serverOwner && senderSteamId.Value != LocalClient.instance.serverHost.Value)
        {
            Debug.LogWarning($"Packet denied: Sender {new Friend(senderSteamId).Name} is not the server.");
            return;
        }

        // Parse and validate the packet
        Packet packet = new Packet(data);
        if (packet.Length() != packet.ReadInt() + 4)
        {
            Debug.LogError("Incomplete packet received.");
            return;
        }

        int key = packet.ReadInt();
        // Debug.Log($"Packet received. Key: {key}, Channel: {channel}");

        // Route packet based on channel
        if (channel == NetworkChannel.ToClient)
        {
            if (senderSteamId.Value != LocalClient.instance.serverHost.Value)
            {
                Debug.LogWarning("Packet to client ignored: Sender is not the server.");
                return;
            }

            // Debug.Log($"[To Client] Packet: {key}");
            LocalClient.packetHandlers[key](packet);
        }
        else if (channel == NetworkChannel.ToServer)
        {
            //Debug.Log($"[To Server] Packet: {key}");
            if (LobbyManager.steamIdToClientId.TryGetValue(senderSteamId.Value, out int clientId))
            {
                Server.PacketHandlers[key](clientId, packet);
            }
            else
            {
                Debug.LogError("Sender not found in client ID mapping.");
            }
        }
    }

    public static void SendPacket(SteamId steamId, Packet packet, P2PSend p2pSend, NetworkChannel channel)
    {
        byte[] packetData = packet.CloneBytes();
        int length = packet.Length();

        // Simulate handling locally if sending to self
        if (steamId.Value == SteamManager.instance.playerSteamId.Value)
        {
            // Debug.Log($"[Local] Handling packet locally: {packet}");
            HandlePacket(new P2Packet
            {
                SteamId = steamId,
                Data = packetData
            }, channel);
        }
        else
        {
            // Debug.Log($"[Send] Packet to {steamId.Value}, Channel: {channel}, Size: {length}");
            SteamNetworking.SendP2PPacket(steamId.Value, packetData, length, (int)channel, p2pSend);
        }
    }

    private void OnApplicationQuit()
    {
        CloseConnections();
    }

    public static void CloseConnections()
    {
        foreach (ulong steamId in LobbyManager.steamIdToClientId.Keys)
        {
            SteamNetworking.CloseP2PSessionWithUser((SteamId)steamId);
        }

        try
        {
            SteamNetworking.CloseP2PSessionWithUser(LocalClient.instance.serverHost);
        }
        catch
        {
            Debug.LogWarning("Failed to close P2P session with host.");
        }

        SteamClient.Shutdown();
        Debug.Log("Steam client shutdown complete.");
    }

    public enum NetworkChannel
    {
        ToClient = 0,
        ToServer = 1
    }
}
