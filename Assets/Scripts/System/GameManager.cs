using UnityEngine;
using Steamworks;

public class GameManager : MonoBehaviour
{
    public GameState gameState; 

    public static GameManager instance;

    void Start()
    {
        if (instance == null) {
            instance = this;
        }
    }
}
 public enum GameState {
        Lobby,
        Game,
        GameOver,
    }
