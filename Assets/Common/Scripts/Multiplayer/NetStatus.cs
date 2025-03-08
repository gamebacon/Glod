using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetStatus : MonoBehaviour
{
  private static LinkedList<int> pings = new LinkedList<int>();
  private static int pingBuffer = 2;

  private void Awake()
    {

    if (GameManager.GetInstance().isSinglePlayer) 
    {
        return;
    }

     InvokeRepeating("SlowUpdate", 1f, 1f);
    }


  private void SlowUpdate()
  {
    ClientSend.PingServer();
  }

  public static void AddPing(int p)
  {
    NetStatus.pings.AddFirst(p);
    if (NetStatus.pings.Count <= NetStatus.pingBuffer)
      return;
    NetStatus.pings.RemoveLast();
  }

  public static int GetPing() => NetStatus.pings.Count > 0 ? (int) NetStatus.pings.Average() : 0;
}
