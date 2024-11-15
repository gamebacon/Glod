using UnityEngine;
using Steamworks.Data;
using Steamworks;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    private Lobby currentLobby; // Fully qualify the type name
    public TMP_InputField lobbyIDText;

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

    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobbyAsync(4); // Create a lobby with up to 4 players
    }

private void OnLobbyCreated(Result result, Lobby lobby)
{
    if (result != Result.OK)
    {
        Console.instance.AddMessage("Failed to create lobby.");
        return;
    }

    currentLobby = lobby;
    Console.instance.AddMessage("Lobby created successfully!");
}


    private void OnLobbyEntered(Lobby lobby)
    {
        currentLobby = lobby;
        Console.instance.AddMessage("Joined lobby with ID: " + lobby.Id);
        foreach(Friend f in lobby.Members) {
            Console.instance.AddMessage(f.Name);
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
        Console.instance.AddMessage("Parsing successful. Attempting to join lobby with ID: " + result);

        Lobby lobby = new Lobby((SteamId)result);

        // Assuming you have a method to check if the lobby is valid
        Console.instance.AddMessage("Lobby found: " + lobby.Id);
        lobby.Join();
    }
    else
    {
        Console.instance.AddMessage("Parsing failed. Ensure lobby ID is a valid number.");
    }
}

}
