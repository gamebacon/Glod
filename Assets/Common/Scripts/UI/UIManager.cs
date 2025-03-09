using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SelectGamePanel selectGamePanel; // Select Game Panel controller
    [SerializeField] private LobbyPanel lobbyPanel; // Create Game Panel controller
    [SerializeField] private MainMenu menuPanel; // Create Game Panel controller

    [SerializeField] private float _camTransitionDuration;
    private CameraMenuTransition _cameraMenuTransition;

    private MenuState menuState = MenuState.MainMenu;

     void OnEnable()
    {
        _cameraMenuTransition = GetComponent<CameraMenuTransition>();

        lobbyPanel.panel.SetActive(false);
        lobbyPanel.leaveButton.onClick.AddListener(HandleLeaveLobby);
        lobbyPanel.startButton.onClick.AddListener(HandleStartGame);

        selectGamePanel.panel.SetActive(false);
        selectGamePanel.createGame.onClick.AddListener(HandleCreateLobby);
        selectGamePanel.joinGame.onClick.AddListener(HandleJoinLobby);
        selectGamePanel.backToMenu.onClick.AddListener(GoToMainMenu);

        menuPanel.panel.SetActive(true);
        menuPanel.playButton.onClick.AddListener(HandlePlayButton);
        menuPanel.quitButton.onClick.AddListener(HandleQuitGame);
    }

    private void HandlePlayButton() 
    {
        GoToSelectGame();
    } 
    private void HandleCreateLobby() 
    {

        if (GameManager.GetInstance().isSinglePlayer)
        {
            GameManager.GetInstance().StartSinglePlayerGame();
        } 
        else
        {
            lobbyPanel.startButton.gameObject.SetActive(true);
            LobbyManager.instance.CreateLobby();
            GoToLobby();
        } 

    } 
    private void HandleJoinLobby() 
    {
        Debug.Log("jon lobby");
        LobbyManager.instance.JoinLobby();
        lobbyPanel.startButton.gameObject.SetActive(false);
        GoToLobby();
    } 

    private void HandleLeaveLobby() 
    {
        LobbyManager.instance.LeaveLobby();
        GoToSelectGame();
    } 

    private void HandleStartGame() 
    {
        LobbyManager.instance.StartGame();
    } 

    private void HandleQuitGame() 
    {
        Application.Quit();
    } 

    private void GoToMainMenu() {

        menuState = MenuState.MainMenu;
        _cameraMenuTransition.GoToPos(menuPanel.cameraPos, _camTransitionDuration);
        selectGamePanel.panel.SetActive(false);
        menuPanel.panel.SetActive(true);
    }
    private void GoToLobby() {

        menuState = MenuState.GameLobby;
        _cameraMenuTransition.GoToPos(lobbyPanel.cameraPos, _camTransitionDuration);
        selectGamePanel.panel.SetActive(false);
        lobbyPanel.panel.SetActive(true);
    }

    private void GoToSelectGame() {

        menuState = MenuState.CreateOrJoin;
        _cameraMenuTransition.GoToPos(selectGamePanel.cameraPos, _camTransitionDuration);
        lobbyPanel.panel.SetActive(false);
        menuPanel.panel.SetActive(false);
        selectGamePanel.panel.SetActive(true);
    }

}

public enum MenuState {
    MainMenu,
    CreateOrJoin,
    GameLobby,
}

[System.Serializable]
public class SelectGamePanel
{
    [SerializeField] public GameObject panel; // The main panel GameObject
    [SerializeField] public Button createGame;  // Example button reference
    [SerializeField] public Button joinGame;
    [SerializeField] public Button backToMenu;
    [SerializeField] public Transform cameraPos;

}

[System.Serializable]
public class LobbyPanel
{
    [SerializeField] public GameObject panel; 
    [SerializeField] public Button startButton;  
    [SerializeField] public Button leaveButton;

    [SerializeField] public Transform cameraPos;

}

[System.Serializable]
public class MainMenu 
{
    [SerializeField] public GameObject panel; // The main panel GameObject
    [SerializeField] public Button playButton;  // Example button reference
    [SerializeField] public Button optionsButton;
    [SerializeField] public Button quitButton;

    [SerializeField] public Transform cameraPos;

}
