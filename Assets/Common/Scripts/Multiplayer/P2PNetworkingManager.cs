using UnityEngine;
using Steamworks;

public class P2PNetworkingManager : MonoBehaviour
{
    private bool isHost;
    private SteamId hostSteamId;

    void Start()
    {
        if (LobbyManager.instance.IsHost)
        {
            isHost = true;
            hostSteamId = SteamClient.SteamId;
            StartHost();
        }
        else
        {
            isHost = false;
            hostSteamId = LobbyManager.instance.HostSteamId; // Get the host's Steam ID
            StartClient();
        }

        Debug.Log(isHost);
    }

    void OnDestroy()
    {
        /*
        // Close any active connections
        foreach (var connection in SteamNetworking.Connections)
        {
            connection.Close();
        }
        */

        // Shutdown the Steam client
        SteamClient.Shutdown();
    }

    private void StartHost()
    {
    }

    private void StartClient()
    {
    }
}
