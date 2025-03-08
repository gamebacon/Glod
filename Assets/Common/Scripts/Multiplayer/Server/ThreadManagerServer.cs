using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManagerServer : MonoBehaviour
{
  private static readonly List<Action> executeOnMainThread = new List<Action>();
  private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
  private static bool actionToExecuteOnMainThread = false;
  public static ThreadManagerServer Instance;
  // private int minPlayerAmount = 3;

  public static void ExecuteOnMainThread(Action _action)
  {
    if (_action == null)
    {
      Debug.Log("No action to execute on main thread!");
    }
    else
    {
      lock (ThreadManagerServer.executeOnMainThread)
      {
        ThreadManagerServer.executeOnMainThread.Add(_action);
        ThreadManagerServer.actionToExecuteOnMainThread = true;
      }
    }
  }

  private void Awake()
  {
    ThreadManagerServer.Instance = this;
    this.InvokeRepeating("TimeoutUpdate", 1f, 1f);
  }

  public void GameOver()
  {
  }

  public void ResetGame()
  {
  }

  private void TimeoutUpdate()
  {
  }

  private void FixedUpdate() => ThreadManagerServer.UpdateMain();

  public static void UpdateMain()
  {
    if (!ThreadManagerServer.actionToExecuteOnMainThread)
      return;
    ThreadManagerServer.executeCopiedOnMainThread.Clear();
    lock (ThreadManagerServer.executeOnMainThread)
    {
      ThreadManagerServer.executeCopiedOnMainThread.AddRange((IEnumerable<Action>) ThreadManagerServer.executeOnMainThread);
      ThreadManagerServer.executeOnMainThread.Clear();
      ThreadManagerServer.actionToExecuteOnMainThread = false;
    }
    for (int index = 0; index < ThreadManagerServer.executeCopiedOnMainThread.Count; ++index)
      ThreadManagerServer.executeCopiedOnMainThread[index]();
  }
}