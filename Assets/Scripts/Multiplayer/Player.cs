using System.Text;
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
    public Color color = Color.black;
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

      public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Player Info:");
        sb.AppendLine($"ID: {id}");
        sb.AppendLine($"Username: {username}");
        sb.AppendLine($"SteamID: {steamId}");
        sb.AppendLine($"Has Joined: {hasJoined}");
        sb.AppendLine($"Is Loading: {isLoading}");
        sb.AppendLine($"Is Ready: {isReady}");
        sb.AppendLine($"Color: {ColorUtility.ToHtmlStringRGBA(color)}");
        sb.AppendLine($"Orientation (Y): {yOrientation}");
        sb.AppendLine($"Orientation (X): {xOrientation}");
        sb.AppendLine($"Position: {pos}");
        sb.AppendLine($"Is Dead: {isDead}");
        sb.AppendLine($"Last Ping Time: {lastPingTime:F2} seconds");

        return sb.ToString();
    }

    public void PingPlayer() => this.lastPingTime = Time.time;
    
}
