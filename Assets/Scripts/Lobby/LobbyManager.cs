using System;
using System.Threading.Tasks;
using UnityEngine;
using Steamworks.Data;
using Steamworks;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public TMP_InputField lobbyIDText;
    public static LobbyManager Instance { get; private set; } // Singleton instance

    private Lobby currentLobby; // Fully qualify the type name
    public bool IsHost { get; private set; } // Indicate if this player is the host
    public SteamId HostSteamId => currentLobby.Owner.Id; // Get the host's Steam ID

    public bool lobbyPartnerDisconnected;

    private void Awake()
    {
        // Ensure only one instance of LobbyManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
    }

    void Start()
    {
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
    }

    void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamClient.Shutdown(); // Proper shutdown of Steamworks
    }

public async void /* Task<bool> */ CreateLobby()
{
    try {
        var createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync(5);
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
        // return true;
    } catch (Exception e) {
        Debug.Log("Failed to create multiplayer lobby");
        Debug.Log(e.ToString());
        // return false;
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
    }

public void LeaveLobby()
{
    currentLobby.Leave();
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

}
