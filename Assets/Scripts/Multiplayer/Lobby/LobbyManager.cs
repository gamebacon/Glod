using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Steamworks.Data;
using Steamworks;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public TMP_InputField lobbyIDText;
    public static LobbyManager instance { get; private set; } // Singleton instance

    private Lobby currentLobby; // Fully qualify the type name
    public bool IsHost { get; private set; } // Indicate if this player is the host
    public SteamId HostSteamId => currentLobby.Owner.Id; // Get the host's Steam ID

    public bool lobbyPartnerDisconnected;

    public static Dictionary<ulong, int> steamIdToClientId = new Dictionary<ulong, int>();

    public SteamId lobbyOwnerSteamId;

    public bool hasStarted = false;

    public LobbyVisuals lobbyVisuals;

    public int lobbySize = 10;

    private void Awake()
    {
        // Ensure only one instance of LobbyManager exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        lobbyVisuals = GetComponent<LobbyVisuals>();

        DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
    }

    void Start()
    {
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoinedCallback;
        SteamMatchmaking. OnLobbyMemberLeave += OnLobbyMemberLeaveCallback;

    }

    void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoinedCallback;
        SteamClient.Shutdown();
    }

    private void OnLobbyMemberJoinedCallback(Lobby lobby, Friend friend) 
    {

        Console.AddMessage($"Join callback: {friend.Name} joined the lobby!");
        if (friend.Id != SteamManager.instance.playerSteamId) {
            SteamManager.instance.AcceptP2P(friend.Id);
        }
        lobbyVisuals.AddPlayer(friend.Name, friend.Id.ToString());
        currentLobby = lobby;
        lobbyOwnerSteamId = lobby.Owner.Id.Value;
        // lobbyPartnerDisconnected = false;
    }

    private void OnLobbyMemberLeaveCallback(Lobby lobby, Friend friend) 
    {
        Console.AddMessage($"Leave callback: {friend.Name} left the lobby!");
        if (friend.Id != SteamManager.instance.playerSteamId) {
            SteamManager.instance.CloseP2P(friend.Id);
        }
        lobbyVisuals.RemovePlayer(friend.Id.ToString());
    }

public async void CreateLobby()
{
    try {
        var createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync(4);
        if (!createLobbyOutput.HasValue)
        {
            Debug.Log("Lobby created but not correctly instantiated");
            throw new Exception();
        }
        lobbyPartnerDisconnected = false;
        var hostedMultiplayerLobby = createLobbyOutput.Value;
        hostedMultiplayerLobby.SetPublic();
        hostedMultiplayerLobby.SetJoinable(true);
        // currentLobby.SetData(staticDataString, lobbyParameters)
        currentLobby = hostedMultiplayerLobby;
    } catch (Exception e) {
        Debug.Log("Failed to create multiplayer lobby");
        Debug.Log(e.ToString());
    }
}

  public void StartGame()
  {
    if(SteamClient.SteamId.Value != currentLobby.Owner.Id.Value)
    {
      Debug.LogError((object) "not owner, so cant start lobby");
    }
    else
    {
      Console.AddMessage("Starting lobby"); // here
      Debug.Log(Server.clients.Count);
        foreach (Client client in Server.clients.Values)
        {
            Debug.Log(client);
            Debug.Log(client?.player);
            if (client?.player != null)
            {
                ServerSend.StartGame(client.player.id);
            }
        }
        currentLobby.SetJoinable(false);
        hasStarted = true;
        LocalClient.serverOwner = true;
        Debug.Log(SteamManager.instance.playerSteamId);
        LocalClient.instance.serverHost = SteamManager.instance.playerSteamId;
  }
}

private void OnLobbyCreated(Result result, Lobby lobby)
{
    if (result != Result.OK)
    {
        Console.AddMessage("Failed to create lobby.");
        return;
    }

    currentLobby = lobby;
    Console.AddMessage("Lobby created successfully!");
}


 private void OnLobbyEntered(Lobby lobby)
    {
        currentLobby = lobby;
        Console.AddMessage("Joined lobby with ID: " + lobby.Id);
        foreach(Friend f in lobby.Members) {
            Console.AddMessage(f.Name);
        }
        lobbyVisuals.AddPlayer(SteamManager.instance.playerName, SteamManager.instance.playerSteamIdString);
    }


  public void LeaveLobby()
  {
    try
    {
        if (currentLobby.Owner.Id.Value == SteamManager.instance.playerSteamId.Value)
            steamIdToClientId = new Dictionary<ulong, int>();
            hasStarted = false;
            lobbyVisuals.RemovePlayer(SteamManager.instance.playerSteamIdString);
        }
    catch
    {
      Debug.Log("Steam lobby doesn't exist...");
    }

    try
    {
      currentLobby.Leave();
      Debug.Log("Lobby left successfully");
      Console.AddMessage("Left lobby");
    }
    catch
    {
      Debug.Log("Error leaving current lobby");
    }
    try
    {
      SteamNetworking.CloseP2PSessionWithUser(lobbyOwnerSteamId);
    }
    catch
    {
      Debug.Log("Error closing P2P session with opponent");
    }
    currentLobby = new Lobby();
  }

public void JoinLobby()
{
    // Try to parse the text to ulong
    if (ulong.TryParse(lobbyIDText.text, out ulong result))
    {
        Console.AddMessage("Parsing successful. Attempting to join lobby with ID: " + result);

        Lobby lobby = new Lobby((SteamId)result);

        // Assuming you have a method to check if the lobby is valid
        Console.AddMessage("Lobby found: " + lobby.Id);
        lobby.Join();
    }
    else
    {
        Console.AddMessage("Parsing failed. Ensure lobby ID is a valid number.");
    }
}

  private void InitLobbyClients()
  {
    Server.clients = new Dictionary<int, Client>();
    for (int index = 0; index < lobbySize; ++index) 
    {
      Server.clients[index] = new Client(index);
    }
  }

}

