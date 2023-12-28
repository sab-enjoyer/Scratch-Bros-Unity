using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public static class LobbyOrchestrator
{
    public static async Task<Lobby> CreateLobby(int character, bool ready)
    {
        Lobby lobby;

        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions {
            IsPrivate = true,
            Player = GetPlayer(character, ready),
            Data = new Dictionary<string, DataObject>()
            {
                {Constants.mapKey, new DataObject(DataObject.VisibilityOptions.Member, "0") },
                {Constants.gameStartKey, new DataObject(DataObject.VisibilityOptions.Member, "0") }
            }
        };

        lobby = await LobbyService.Instance.CreateLobbyAsync("n", Constants.maxPlayers, lobbyOptions);

        Debug.Log("Created lobby");

        return lobby;
    }

    public static async Task<Lobby> JoinLobby(string joinCode, int character, bool ready)
    {
        Lobby lobby;

        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
        {
            Player = GetPlayer(character, ready)
        };

        lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode, options);

        Debug.Log("Joined lobby with code: " + lobby.LobbyCode);

        return lobby;
    }

    public static Unity.Services.Lobbies.Models.Player GetPlayer(int character, bool ready)
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {Constants.nameKey, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, GameManager.instance.GetUsername()) },
                        {Constants.charKey, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, character.ToString()) },
                        {Constants.readyKey, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ready.ToString()) },
                        {Constants.relayConnectKey, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "none") }

                    }
        };
    }
}