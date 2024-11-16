using System.Collections.Generic;
using System;
using UnityEngine;
using Steamworks;
using Steamworks.Data;



public class SteamManager : MonoBehaviour {

    public static SteamManager Instance;
    public bool isRealInstance;
    public uint appId = 480;

    public string playerName;
    public SteamId playerSteamId;
    public string playerSteamIdString;

    public List<Lobby> activeLobbies;

    public bool connectedToSteam;


    public void Awake() {
        if (Instance == null) {
            isRealInstance = true;
            DontDestroyOnLoad(gameObject);
            Instance = this;
            playerName = "";
            try {
                SteamClient.Init(appId, true);
                if(!SteamClient.IsValid) {
                    throw new Exception("Invalid steam client!");
                }
                playerName = SteamClient.Name;
                playerSteamId = SteamClient.SteamId;
                playerSteamIdString = playerSteamId.ToString();
                connectedToSteam = true;
                activeLobbies = new List<Lobby>();
                Console.AddMessage("Connected to steam");

            } catch (Exception e) {

            }
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

   void Start()
    {
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoinedCallback;
        /*
        // Callbacks
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreatedCallback;
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallback;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEnteredCallback;
        SteamMatchmaking.OnChatMessage += OnChatMessageCallback;
        SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnectedCallback;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeaveCallback;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequestedCallback;
        SteamApps.OnDlcInstalled += OnDlcInstalledCallback;
        SceneManager.sceneLoaded += OnSceneLoaded;
        */
    }

    void OnLobbyMemberJoinedCallback(Lobby lobby, Friend friend) 
    {
        Console.AddMessage($"{friend.Name} joined the lobby!");
        if (friend.Id != playerSteamId) {
            AcceptP2P(friend.Id);
        }
    }

    void Update()
    {
        SteamClient.RunCallbacks();
    } 

       void OnDisable()
    {
        if (isRealInstance)
        {
            // leaveLobby();
            SteamClient.Shutdown();
        }
    }

    private void AcceptP2P(SteamId opponentId)
    {
        try
        {
        SteamNetworking.AcceptP2PSessionWithUser(opponentId);
        }
        catch
        {
        Debug.Log((object) "Unable to accept P2P Session with user");
        }
    }
}