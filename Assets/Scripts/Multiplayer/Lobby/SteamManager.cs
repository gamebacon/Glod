using System.Collections.Generic;
using System;
using UnityEngine;
using Steamworks;
using Steamworks.Data;



public class SteamManager : MonoBehaviour {

    public static SteamManager instance;
    public bool isRealInstance;
    public uint appId = 480;

    public string playerName;
    public SteamId playerSteamId;
    public string playerSteamIdString;

    public List<Lobby> activeLobbies;

    public bool connectedToSteam;


    public void Awake() {
        if (instance == null) {
            instance = this;
            isRealInstance = true;
            DontDestroyOnLoad(gameObject);

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
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

   void Start()
    {
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

    public void AcceptP2P(SteamId opponentId)
    {
        try
        {
        SteamNetworking.AcceptP2PSessionWithUser(opponentId);
        Debug.Log($"{opponentId} enter p2p session");
        }
        catch
        {
        Debug.Log((object) "Unable to accept P2P Session with user");
        }
    }

    public void CloseP2P(SteamId opponentId)
    {
        try
        {
        SteamNetworking.CloseP2PSessionWithUser(opponentId);
        }
        catch
        {
        Debug.Log((object) "Unable to close P2P Session with user");
        }
    }
}