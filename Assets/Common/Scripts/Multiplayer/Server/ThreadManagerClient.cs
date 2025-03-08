using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManagerClient : MonoBehaviour
{
  private static readonly List<Action> executeOnMainThread = new List<Action>();
  private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
  private static bool actionToExecuteOnMainThread = false;

  private void Update() => ThreadManagerClient.UpdateMain();

  public static void ExecuteOnMainThread(Action _action)
  {
    if (_action == null)
    {
      Debug.Log((object) "No action to execute on main thread!");
    }
    else
    {
      lock (ThreadManagerClient.executeOnMainThread)
      {
        ThreadManagerClient.executeOnMainThread.Add(_action);
        ThreadManagerClient.actionToExecuteOnMainThread = true;
      }
    }
  }

  public static void UpdateMain()
  {
    if (!ThreadManagerClient.actionToExecuteOnMainThread)
      return;
    ThreadManagerClient.executeCopiedOnMainThread.Clear();
    lock (ThreadManagerClient.executeOnMainThread)
    {
      ThreadManagerClient.executeCopiedOnMainThread.AddRange((IEnumerable<Action>) ThreadManagerClient.executeOnMainThread);
      ThreadManagerClient.executeOnMainThread.Clear();
      ThreadManagerClient.actionToExecuteOnMainThread = false;
    }
    for (int index = 0; index < ThreadManagerClient.executeCopiedOnMainThread.Count; ++index)
      ThreadManagerClient.executeCopiedOnMainThread[index]();
  }
}
