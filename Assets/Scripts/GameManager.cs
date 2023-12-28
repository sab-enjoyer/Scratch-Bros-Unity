using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int mapID;
    public int gameWinner;
    public MapData[] maps;
    public CharacterData[] characters;
    [HideInInspector]
    public int[] playerSkinIDs;
    public List<byte> playerCharacterIds = new();
    public List<PlayerTypes> playerTypes = new();
    public List<Player> playerStore = new();

    public Color[] playerColors;

    public enum GameTypes { Player2, Player1, Training, Online};
    public enum PlayerTypes { Local, Bot, Dummy, Online};
    public GameTypes gameType;
    public GameSettings gameSettings;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            mapID = 0;
            playerCharacterIds.Add(0);
            playerCharacterIds.Add(0);
            Application.targetFrameRate = 30;
            PlayerPrefs.DeleteAll();
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name != "Game" && !(SceneManager.GetActiveScene().name == "Character Select" && gameType == GameTypes.Online))
            {
                Debug.Log("quitted" + (SceneManager.GetActiveScene().name == "Character Select" && gameType == GameTypes.Online));
                Application.Quit();
            }
        }
    }

    public string GetUsername()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("username")))
            return "unnamed";
        else
            return PlayerPrefs.GetString("username");
    }
}
