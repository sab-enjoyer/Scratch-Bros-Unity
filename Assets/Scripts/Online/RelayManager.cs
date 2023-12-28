using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RelayManager : MonoBehaviour
{
    public static RelayManager instance;

    public bool IsRelayEnabled => Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    private UnityTransport _transport;

    private string _joinCode;
    public string JoinCode
    {
        get { return _joinCode; }
    }

    private List<ulong> _connectedPlayers = new();

    public IReadOnlyList<ulong> ConnectedPlayers
    {
        get { return _connectedPlayers; }
    }
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
    private void Awake()
    {
        if (instance == null && NetworkManager.Singleton != null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            _connectedPlayers.Clear();
        }
        else
            Destroy(gameObject);
    }

    private void OnClientDisconnect(ulong obj)
    {
        _connectedPlayers.Remove(obj);
        Debug.Log("client disconnected");
    }

    private void OnClientConnect(ulong obj)
    {
        _connectedPlayers.Add(obj);
        Debug.Log("client connected");
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            }
        }
    }

    public async Task SetupRelay()
    {
        Debug.Log("Relay server starting");

        // create allocation
        Allocation a = await RelayService.Instance.CreateAllocationAsync(Constants.maxPlayers);

        // get join code
        _joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
        Debug.Log("Got join code: " + JoinCode);

        // set transport data
        Transport.SetHostRelayData(
            a.RelayServer.IpV4,
            (ushort)a.RelayServer.Port,
            a.AllocationIdBytes,
            a.Key,
            a.ConnectionData
        );


        NetworkManager.Singleton.StartHost();
    }

    public async Task JoinRelay(string joinCode)
    {
        // get join allocation
        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCode);

        // set transport data
        Transport.SetClientRelayData(
            a.RelayServer.IpV4,
            (ushort)a.RelayServer.Port,
            a.AllocationIdBytes,
            a.Key,
            a.ConnectionData,
            a.HostConnectionData
        );


        NetworkManager.Singleton.StartClient();
        Debug.Log("Client joined game with join code: " + joinCode);
    }
    public void AddtoConnectedPlayers(ulong id)
    {
        _connectedPlayers.Add(id);
    }
}