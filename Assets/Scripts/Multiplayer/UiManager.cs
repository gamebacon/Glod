using System;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
  public static UiManager instance;
  public GameObject lobbyCam;
  public GameObject startMenu;
  public TMP_InputField usernameField;
  public TMP_InputField ipField;
  public TMP_InputField portField;
  public GameObject server;

  private void Awake()
  {
    if ((UnityEngine.Object) UiManager.instance == (UnityEngine.Object) null)
    {
      UiManager.instance = this;
    }
    else
    {
      if (!((UnityEngine.Object) UiManager.instance != (UnityEngine.Object) this))
        return;
      Debug.Log((object) "Instance already exists, destroying object");
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }
  }

  private void Start()
  {
  }

  public void Host()
  {
    int port = int.Parse(this.portField.text);
    LocalClient.instance.port = port;
    NetworkManager.instance.StartServer(port);
    Server.ipAddress = IPAddress.Any;
    MonoBehaviour.print((object) ("hosting server on: " + (object) Server.ipAddress + " on port: " + this.portField.text));
    this.ConnectTest();
  }

  public void ConnectTest()
  {
    foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
    {
      if (address.AddressFamily == AddressFamily.InterNetwork)
      {
        MonoBehaviour.print((object) ("ip: " + address.ToString()));
        LocalClient.instance.ConnectToServer(address.ToString(), "bread");
        this.lobbyCam.SetActive(false);
        return;
      }
    }
    throw new Exception("No network adapters with an IPv4 address in the system!");
  }

  public void ConnectToServer()
  {
    LocalClient.serverOwner = false;
    LocalClient.instance.port = int.Parse(this.portField.text);
    LocalClient.instance.ConnectToServer(this.ipField.text, "bread");
    MonoBehaviour.print((object) ("connecting to ip:" + this.ipField.text));
    this.lobbyCam.SetActive(false);
    try
    {
      Color black = Color.black;
    }
    catch (Exception ex)
    {
      MonoBehaviour.print((object) ex);
    }
  }

  public void ConnectionSuccessful() => this.startMenu.SetActive(false);
}
