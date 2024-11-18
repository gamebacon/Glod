// Decompiled with JetBrains decompiler
// Type: NetworkManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8DB17789-D6D5-48DE-86AB-E696A5FF6B2B
// Assembly location: D:\SteamLibrary\steamapps\common\Muck\Muck_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class NetworkManager : MonoBehaviour
{
  public static NetworkManager instance;

  public static float Clock { get; set; }

  public static float CountDown { get; set; }

  private void Update() => NetworkManager.Clock += Time.deltaTime;

  public int GetSpawnPosition(int id) => id;

  private void Awake()
  {
    if ((Object) NetworkManager.instance == (Object) null)
    {
      NetworkManager.instance = this;
    }
    else
    {
      if (!((Object) NetworkManager.instance != (Object) this))
        return;
      Debug.Log((object) "Instance already exists, destroying object");
      Object.Destroy((Object) this);
    }
  }

  private void Start()
  {
  }

  public void StartServer(int port) => Server.Start(40, port);

  private void OnApplicationQuit() => Server.Stop();

  public void DestroyPlayer(GameObject g) => Object.Destroy((Object) g);
}
