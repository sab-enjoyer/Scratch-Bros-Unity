using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public static class Constants
{
    public const string nameKey = "n";
    public const string charKey = "c";
    public const string readyKey = "r";
    public const string mapKey = "m";
    public const string gameStartKey = "s";
    public const string relayConnectKey = "o";

    public const int maxPlayers = 2;
    public const string enviroment = "production";
    public const float heartbeatTimerMax = 15f;
    public const float lobbyUpdateTimerMax = 2f;
}

public struct RelayHostData
{
    public string JoinCode;
    public string IPv4Address;
    public ushort Port;
    public Guid AllocationID;
    public byte[] AllocationIDBytes;
    public byte[] ConnectionData;
    public byte[] Key;
}
public struct RelayJoinData
{
    public string JoinCode;
    public string IPv4Address;
    public ushort Port;
    public Guid AllocationID;
    public byte[] AllocationIDBytes;
    public byte[] ConnectionData;
    public byte[] HostConnectionData;
    public byte[] Key;
}

public struct NetworkString : INetworkSerializable
{
    private FixedString32Bytes info;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref info);
    }

    public override string ToString()
    {
        return info.ToString();
    }

    public static implicit operator string(NetworkString s) => s.ToString();
    public static implicit operator NetworkString(string s) => new NetworkString() { info = new FixedString32Bytes(s) };
}

/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyOrchestrator : MonoBehaviour
{
    [SerializeField]
    private CharacterSelect charSelect;

    [SerializeField]
    private Button createButton, joinButton;

    [SerializeField]
    private TMP_InputField joinCodeInput;

    [SerializeField]
    private TMP_Text nameText, joinCodeText;

    [SerializeField]
    private GameObject lobbyObject;

    private UnityTransport _transport;

    private Lobby currentLobby;
    private static CancellationTokenSource heartbeatSource, updateLobbySource;


    public UnityTransport Transport
    {
        get
        {
            if (_transport == null)
                _transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            return  _transport;
        }
        set { _transport = value; }
    }

    public async void CreateGame()
    {
        lobbyObject.SetActive(true);
        createButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        joinCodeInput.gameObject.SetActive(false);

        Allocation a = await RelayService.Instance.CreateAllocationAsync(Constants.maxPlayers);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
        string name = PlayerPrefs.GetString("username");

        var options = new CreateLobbyOptions();
        options.Data = new Dictionary<string, DataObject>()
        {
            {Constants.joinKey, new DataObject(DataObject.VisibilityOptions.Member, joinCode) },
            {Constants.nameKey, new DataObject(DataObject.VisibilityOptions.Member, name) },
            {Constants.charKey, new DataObject(DataObject.VisibilityOptions.Member, charSelect.selectedCharacters[0].ToString()) },
            {Constants.mapKey, new DataObject(DataObject.VisibilityOptions.Member, GameManager.instance.mapID.ToString()) }
        };

        currentLobby = await Lobbies.Instance.CreateLobbyAsync(name + "'s room", 2, options);

        Transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
        NetworkManager.Singleton.StartHost();

        nameText.text = name + "'s room";
        joinCodeText.text = "Join Code: " + joinCode;

        Heartbeat();
        PeriodicallyRefreshLobby();
    }

    public async void JoinGame()
    {
        lobbyObject.SetActive(true);
        createButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        joinCodeInput.gameObject.SetActive(false);

        bool joinFailed = false;
        string joinCode = joinCodeInput.text;
        JoinLobbyByCodeOptions options = new();

        Debug.Log(joinCode + ", length is " + joinCode.Length);
        try
        {
            currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode, options);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            joinFailed = true;
        }
        if (!joinFailed)
        {
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

            NetworkManager.Singleton.StartClient();
            nameText.text = currentLobby.Data[Constants.nameKey].Value + "'s room";
            joinCodeText.text = "Join Code: " + currentLobby.Data[Constants.joinKey].Value;
        }
    }

    private async void Heartbeat()
    {
        heartbeatSource = new CancellationTokenSource();
        while (!heartbeatSource.IsCancellationRequested && currentLobby != null)
        {
            Debug.Log("refreshed");
            await Lobbies.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            await Task.Delay(Constants.HeartbeatInterval * 1000);
        }
    }

    private async void PeriodicallyRefreshLobby()
    {
        updateLobbySource = new CancellationTokenSource();
        await Task.Delay(Constants.LobbyRefreshRate * 1000);
        while (!updateLobbySource.IsCancellationRequested && currentLobby != null)
        {
            currentLobby = await Lobbies.Instance.GetLobbyAsync(currentLobby.Id);
            await Task.Delay(Constants.LobbyRefreshRate * 1000);
        }
    }
}*/