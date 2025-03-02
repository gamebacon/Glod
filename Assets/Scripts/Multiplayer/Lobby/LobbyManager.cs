using System;
using System.Collections.Generic;
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
    }

    void Start()
    {
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoinedCallback;
        SteamMatchmaking. OnLobbyMemberLeave += OnLobbyMemberLeaveCallback;

    }

    private void InitLobby(Lobby lobby) {

        InitLobbyClients();
        steamIdToClientId = new Dictionary<ulong, int>();
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
        } else {
          Debug.Log($"{friend.Id} - {SteamManager.instance.playerSteamId}");
        }
        lobbyVisuals.AddPlayer(friend.Name, friend.Id.ToString());
        currentLobby = lobby;
        lobbyOwnerSteamId = lobby.Owner.Id.Value;
        // lobbyPartnerDisconnected = false;

        AddClient(friend);
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
        Debug.Log("Failed to create multiplayer lobby (Is steam running?)");
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
        Console.AddMessage("Starting lobby");

        /*
        *  Generate seed and provide to all clients when starting the game
        */
        int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        foreach (Client client in Server.clients.Values)
        {
            if (client?.player != null)
            {
                // Debug.Log($"Sending start packet to {client.player.ToString()}");
                ServerSend.StartGame(client.player.id, seed);
            } else {
            }
        }
        currentLobby.SetJoinable(false);
        hasStarted = true;
        LocalClient.serverOwner = true;
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

    Console.AddMessage("Lobby created successfully!");

    currentLobby = lobby;
    hasStarted = false;
    InitLobby(lobby);
    AddClient(new Friend(HostSteamId));
}


 private void OnLobbyEntered(Lobby lobby)
    {
        currentLobby = lobby;
        Console.AddMessage("Joined lobby with ID: " + lobby.Id);
        foreach(Friend f in lobby.Members) {
            Console.AddMessage(f.Name);
        }
        lobbyVisuals.AddPlayer(SteamManager.instance.playerName, SteamManager.instance.playerSteamIdString);
        LocalClient.serverOwner = true;
        LocalClient.instance.serverHost = lobby.Owner.Id.Value;
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


  private int FindAvailableClientId()
  {
    for (int key = 0; key < lobbySize; ++key)
    {
      if (!Server.clients[key].inLobby) {
        return key;
      }
    }
    return -1;
  }

  private void AddClient(Friend friend)
  {
    SteamId steamId = friend.Id.Value;
    int availableLobbyId = this.FindAvailableClientId();

    if (availableLobbyId == -1) {
      return;
    }

    // Debug.Log($"Found available id in steam as: {availableLobbyId} steam name: {friend.Name}");
    steamIdToClientId[steamId] = availableLobbyId;
    Client client = Server.clients[availableLobbyId];
    client.inLobby = true;
    client.player = new Player(availableLobbyId, friend.Name, UnityEngine.Color.black, steamId);
    // Debug.Log($"finished adding client. ({friend.Name}) ");
  }

}

