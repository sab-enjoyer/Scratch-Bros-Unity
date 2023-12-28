using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LobbyUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private GameObject room;

    [SerializeField]
    private Button createButton;

    [SerializeField]
    private Button joinButton, leaveButton, startGame;

    [SerializeField]
    private TMP_InputField joinCodeInput;

    [SerializeField]
    private TMP_Text nameText, joinCodeText;

    [SerializeField]
    private OnlinePlayerDisplay[] players;

    [SerializeField]
    private TMP_Text usernameDisplay;

    [SerializeField]
    private GameObject startScreen;

    [Header("Data References")]
    [SerializeField]
    private CharacterSelect charSelect;

    public Lobby myLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private bool isHost = false;
    private bool isLeaving = false;

    private void Awake()
    {
        //Destroy these gameobjects if gamemode is not online
        if (GameManager.instance.gameType != GameManager.GameTypes.Online)
        {
            Destroy(NetworkManager.Singleton.gameObject);
            Destroy(gameObject);
            Destroy(RelayManager.instance.gameObject);
        }
        else
        {
            NetworkManager.Singleton.RunInBackground = true;
            setButtons(true);
            usernameDisplay.text = "username: " + GameManager.instance.GetUsername();
            startScreen.SetActive(false);
        }
    }

    public async void CreateGame()
    {
        try
        {
            myLobby = await LobbyOrchestrator.CreateLobby(charSelect.selectedCharacters[0], charSelect.playerReady[0]);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            myLobby = null;
        }
        if (myLobby != null)
        {
            isHost = true;
            joinCodeText.text = "join code: " + myLobby.LobbyCode;
            setButtons(false);
            UpdatePlayer(0);
        }
    }

    public async void JoinGame()
    {
        try
        {
            myLobby = await LobbyOrchestrator.JoinLobby(joinCodeInput.text,  charSelect.selectedCharacters[0], charSelect.playerReady[0]);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            myLobby = null;
        }
        if (myLobby != null)
        {
            isHost = false;
            joinCodeText.text = "join code: " + myLobby.LobbyCode;
            setButtons(false);
            UpdatePlayer(0);
        }
    }

    public async void LeaveGame()
    {
        isLeaving = true;
        //NetworkManager.Singleton.Shutdown();
        setButtons(true);
        try
        {
            if (isHost)
            {
                if (myLobby.Players.Count > 1)
                {
                    await LobbyService.Instance.UpdateLobbyAsync(myLobby.Id, new UpdateLobbyOptions
                    {
                        HostId = myLobby.Players[1].Id
                    });
                }
                isHost = false;
                Debug.Log("destroyed lobby");
            } 
            else
            {
                Debug.Log("left lobby");
            }
            await LobbyService.Instance.RemovePlayerAsync(myLobby.Id, Authentication.PlayerId);
            myLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        isLeaving = false;
    }

    public async void StartGame()
    {
        if (!isHost) // dont think its possible to access this button without being host but its better to be safe
            return;

        try
        {
            await RelayManager.instance.SetupRelay();
            string joinCode = RelayManager.instance.JoinCode;

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(myLobby.Id, new UpdateLobbyOptions
            {
                IsLocked = true,
                Data = new Dictionary<string, DataObject>
                {
                    {Constants.gameStartKey, new DataObject(DataObject.VisibilityOptions.Member, joinCode) },
                }
            });
            myLobby = lobby;

            UpdatePlayer(2);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    private void DisableNetworkManager(Scene current)
    {
        if (current.name != "Title")
            return;
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
        Destroy(NetworkManager.Singleton.gameObject);
    }

    private void OnEnable()
    {
        if (GameManager.instance.gameType == GameManager.GameTypes.Online)
            SceneManager.sceneUnloaded += DisableNetworkManager;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= DisableNetworkManager;
    }

    private void Update()
    {
        if (!isLeaving)
        {
            if (players[0].IsReady() && players[1].IsReady())
                startGame.gameObject.SetActive(true);
            else
                startGame.gameObject.SetActive(false);
            HandleLobbyHeartbeat();
            HandleLobbyPollForUpdates();
        }
        else
            startGame.gameObject.SetActive(false);
    }

    private async void HandleLobbyHeartbeat()
    {
        if (myLobby != null && isHost)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                // Send heartbeat to lobby so it won't close of inactivity
                heartbeatTimer = Constants.heartbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(myLobby.Id);
                Debug.Log("heartbeat");
            }
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (myLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                lobbyUpdateTimer = Constants.lobbyUpdateTimerMax;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(myLobby.Id);
                myLobby = lobby;

                nameText.text = myLobby.Players[0].Data[Constants.nameKey].Value + "'s room";
                // update player data
                foreach (OnlinePlayerDisplay playerDisplay in players)
                {
                    if (myLobby.Players.Count > playerDisplay.playerId)
                    {
                        playerDisplay.username = myLobby.Players[playerDisplay.playerId].Data[Constants.nameKey].Value;
                        playerDisplay.character = int.Parse(myLobby.Players[playerDisplay.playerId].Data[Constants.charKey].Value);
                        playerDisplay.ready = bool.Parse(myLobby.Players[playerDisplay.playerId].Data[Constants.readyKey].Value);
                    }
                    else
                        playerDisplay.username = "";
                }

                // Check if game has started and can still start
                string joinCode = myLobby.Data[Constants.gameStartKey].Value;
                if (joinCode != "0")
                {
                    startScreen.SetActive(true);
                    charSelect.characterDisplays[0].gameObject.SetActive(false);
                    Debug.Log("Checking if ready to start");
                    try
                    {
                        //join relay if not host
                        if (!isHost && !NetworkManager.Singleton.IsClient)
                        {
                            await RelayManager.instance.JoinRelay(joinCode);
                            UpdatePlayer(2);
                        }

                        //start if all players have connected to relay
                        if (myLobby.Players[0].Data[Constants.relayConnectKey].Value != "none" && myLobby.Players[0].Data[Constants.relayConnectKey].Value != "none")
                        {
                            Debug.Log("WERE STARTING, " + myLobby.Players[0].Data[Constants.relayConnectKey].Value);
                            // Setup players for game
                            SetupGame();
                            //start game
                            SceneManager.LoadSceneAsync("Game");
                        }
                    }
                    catch (RelayServiceException e)
                    {
                        Debug.Log(e);
                    }
                }
            }
        }
    }

    public async void UpdatePlayer(int type)
    {
        if (!isLeaving && myLobby != null)
        {
            try
            {
                if (type == 0)
                {
                    // update character
                    await LobbyService.Instance.UpdatePlayerAsync(myLobby.Id, Authentication.PlayerId, new UpdatePlayerOptions
                    { Data = new Dictionary<string, PlayerDataObject>{{Constants.charKey, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, charSelect.selectedCharacters[0].ToString()) }}});
                }
                else if (type == 1)
                {
                    // update player ready
                    await LobbyService.Instance.UpdatePlayerAsync(myLobby.Id, Authentication.PlayerId, new UpdatePlayerOptions
                    { Data = new Dictionary<string, PlayerDataObject>{{Constants.readyKey, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, charSelect.playerReady[0].ToString()) }}});
                }
                else if (type == 2)
                {
                    //update player connected to relay
                    await LobbyService.Instance.UpdatePlayerAsync(myLobby.Id, Authentication.PlayerId, new UpdatePlayerOptions
                    { Data = new Dictionary<string, PlayerDataObject> { { Constants.relayConnectKey, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, RelayManager.instance.Transport.ServerClientId.ToString()) } } });
                    Debug.Log(RelayManager.instance.Transport.ServerClientId.ToString());
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

        }
    }

    private void setButtons(bool Bool)
    {
        createButton.interactable = Bool;
        joinButton.interactable = Bool;
        joinCodeInput.interactable = Bool;
        room.SetActive(!Bool);
        //lobbyData.gameObject.SetActive(!Bool);
    }

    private void SetupGame()
    {
        GameManager.instance.gameSettings = new GameSettings()
        {
            stocks = 5,
            time = 0
        };
        RelayManager.instance.AddtoConnectedPlayers(ulong.Parse(myLobby.Players[0].Data[Constants.relayConnectKey].Value));
        RelayManager.instance.AddtoConnectedPlayers(ulong.Parse(myLobby.Players[1].Data[Constants.relayConnectKey].Value));
        // set characters to those set in lobby, as well as set player types
        GameManager.instance.playerCharacterIds.Clear();
        GameManager.instance.playerTypes.Clear();
        for (int i = 0; i < 2; i++)
        {
            GameManager.instance.playerCharacterIds.Add(byte.Parse(myLobby.Players[i].Data[Constants.charKey].Value));
            if (myLobby.Players[i].Id == Authentication.PlayerId)
                GameManager.instance.playerTypes.Add(GameManager.PlayerTypes.Local);
            else
                GameManager.instance.playerTypes.Add(GameManager.PlayerTypes.Online);
        }

        //get map
        GameManager.instance.mapID = int.Parse(myLobby.Data[Constants.mapKey].Value);
    }
}
