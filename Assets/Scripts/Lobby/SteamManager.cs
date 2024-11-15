using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    private static SteamManager _instance;

    public static SteamManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("SteamManager is not initialized. Make sure it's in the scene!");
            }
            return _instance;
        }
    }

    private bool _isInitialized = false;

    void Awake()
    {
        // Ensure only one instance exists
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize Steamworks
        if (!_isInitialized)
        {
            try
            {
                if (!SteamClient.IsValid)
                {
                    SteamClient.Init(480);
                    Debug.Log("Steam initialized successfully.");
                    _isInitialized = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Steam initialization failed: " + e);
                Application.Quit(); // Optional: Close the game if Steam fails to initialize
            }
        }
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _isInitialized = false;
            SteamClient.Shutdown();
            Debug.Log("Steam shut down.");
        }
    }
}
