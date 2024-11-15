using UnityEngine;
using Steamworks;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        try
        {
            SteamClient.Init(480);
        }
        catch ( System.Exception e )
        {
            Console.instance.AddMessage(e.ToString());
            // Something went wrong - it's one of these:
            //
            //     Steam is closed?
            //     Can't find steam_api dll?
            //     Don't have permission to play app?
            //
        }
    }

    void OnDestroy()
    {
        SteamClient.Shutdown();  // Proper shutdown of Steamworks when done
    }

}
