using UnityEngine;
using Steamworks;

public class Player
{

    public int id;
    public string username;
    public SteamId steamId;

    public bool hasJoined;
    public bool isLoading;
    public bool isReady;
    public Color color;
  public float yOrientation;
  public float xOrientation;
    public Vector3 pos;

    public bool isDead;

    public float lastPingTime;

  public Player(int id, string username, Color color)
  {
    this.id = id;
    this.username = username;
  }

  public Player(int id, string username, Color color, SteamId steamId)
  {
    this.id = id;
    this.username = username;
    this.steamId = steamId;
    isDead = false;
  }

    public void PingPlayer() => this.lastPingTime = Time.time;
    
}
