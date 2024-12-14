using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviour
{
  public NetworkController.NetworkType networkType;
  public GameObject steam;
  public GameObject classic;
  public int nPlayers;
  public static int maxPlayers = 10;
  public Lobby lobby;
  public string[] playerNames;
  public static NetworkController Instance;

  public bool loading { get; set; }

  private void Awake()
  {
    if (NetworkController.Instance)
    {
      Object.Destroy(gameObject);
    }
    else
    {
      NetworkController.Instance = this;
    }
  }

  public void LoadGame(string[] names)
  {
    this.loading = true;
    this.playerNames = names;
    /*
    LoadingScreen.Instance.Show();
    */
    Invoke("StartLoadingScene", 1);
    Debug.Log("Invoking game scene");
  }

  private void StartLoadingScene() {
    SceneManager.LoadScene("Game");
    ClientSend.PlayerFinishedLoading();
  }

  public enum NetworkType
  {
    Steam,
    Classic,
  }
}
