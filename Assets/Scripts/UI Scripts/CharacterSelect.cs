using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField]
    private float selectedBrightness;

    [SerializeField]
    private GameObject IconPrefab;

    [SerializeField]
    private GameObject[] cursors;

    [SerializeField]
    private Sprite fullCursor, halfCursor1, halfCursor2, randomIcon;

    [SerializeField]
    public CSSCharacter[] characterDisplays;

    [SerializeField]
    private Image BG;

    [SerializeField]
    private Color onlineBGColor;

    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private LobbyUI lobbyUI;

    private CharacterData[] characters;

    [HideInInspector]
    public List<int> selectedCharacters = new();
    private List<GameObject> icons = new();
    private List<Image> cursorImages = new();
    public List<bool> playerReady = new();
    private int minChar;
    private void Start()
    {
        if (GameManager.instance == null)
            return;

        if (GameManager.instance.gameType == GameManager.GameTypes.Online)
        {
            gameObject.GetComponent<RectTransform>().localPosition = new Vector2(-530, 0);
            BG.color = onlineBGColor;
            minChar = 0;
        }
        else
            minChar = -1;

        // create icons for each character
        characters = GameManager.instance.characters;
        for (int i = minChar; i < characters.Length; i++)
        {
            GameObject icon = Instantiate(IconPrefab, transform);
            if (i == -1)
                icon.transform.GetChild(0).GetComponent<Image>().sprite = randomIcon;
            else
                icon.transform.GetChild(0).GetComponent<Image>().sprite = characters[i].icon;
            icons.Add(icon);
        }
        
        // set starting selected characters to characters from previous game, and set player ready to true if bot or false otherwise
        selectedCharacters.Clear();
        for (int i = 0; i < GameManager.instance.playerCharacterIds.Count; i++)
        {
            selectedCharacters.Add(GameManager.instance.playerCharacterIds[i]+1);
            if (GameManager.instance.playerTypes[i] == GameManager.PlayerTypes.Local || GameManager.instance.playerTypes[i] == GameManager.PlayerTypes.Online)
                playerReady.Add(false);
            else
            {
                playerReady.Add(true);
                selectedCharacters[i] = GameManager.instance.gameSettings.cpuChar + 1;
            }
        }

        // create cursors
        for (int i = 0; i < cursors.Length; i++)
        {
            if (GameManager.instance.playerTypes[i] != GameManager.PlayerTypes.Local)
            {
                Destroy(cursors[i]);
                cursors = new GameObject[]{ cursors[0]};
            }
            else
            {
                cursorImages.Add(cursors[i].GetComponent<Image>());
                cursorImages[i].color = GameManager.instance.playerColors[i+1];
            }
        }

        foreach (CSSCharacter character in characterDisplays)
        {
            character.gameObject.SetActive(!(character.playerID == 1 && GameManager.instance.playerTypes[character.playerID] == GameManager.PlayerTypes.Online));
        }
    }

    private void Update()
    {
        for (int i = 0; i < cursors.Length; i++)
        {
            if (GameManager.instance.playerTypes[i] != GameManager.PlayerTypes.Local)
                return;

            // get inputs from input manager
            InputManager inputs = new();
            if (!inputField.isFocused)
            {
                if (GameManager.instance.gameType != GameManager.GameTypes.Player2)
                    inputs.GetMenuInputs(-1);
                else
                    inputs.GetMenuInputs(i);
            }
            else
                inputs.setFalseInputs();

            // set player ready to true/false, or leave game
            if (Time.timeSinceLevelLoad > 0.1f)
            {
                if (inputs.attack)
                {
                    playerReady[i] = true;
                    TryUpdatePlayer(1);

                }
                else if (inputs.shield)
                {
                    if (playerReady[i])
                    {
                        playerReady[i] = false;
                        TryUpdatePlayer(1);
                    }
                    else
                    {
                        // prevent leaving screen if in lobby
                        bool temp = true;
                        if (lobbyUI != null)
                        {
                            if (lobbyUI.myLobby != null) temp = false;
                        }

                        if (temp)
                        {
                            if (GameManager.instance.gameType == GameManager.GameTypes.Online)
                                SceneManager.LoadScene("Title");
                            else
                                SceneManager.LoadScene("Stage Select");

                        }
                    }
                }

            }

            // Move cursor left/right
            if (!playerReady[i])
            {
                if (inputs.left)
                {
                    selectedCharacters[i] = (byte)Mathf.Repeat(selectedCharacters[i] - 1, icons.Count);
                    // update player data if in lobby
                    TryUpdatePlayer(0);
                }
                else if (inputs.right)
                {
                    selectedCharacters[i] = (byte)Mathf.Repeat(selectedCharacters[i] + 1, icons.Count);
                    // update player data if in lobby
                    TryUpdatePlayer(0);
                }


                cursors[i].transform.position = icons[selectedCharacters[i]].transform.position;
            }

            // set cursor sprite and color
            if (selectedCharacters[1 - i] == selectedCharacters[i] && cursors.Length != 1)
            {
                if (i == 0) cursorImages[i].sprite = halfCursor1;
                else cursorImages[i].sprite = halfCursor2;
            }
            else
                cursorImages[i].sprite = fullCursor;

            if (!playerReady[i])
                cursorImages[i].color = GameManager.instance.playerColors[i + 1];
            else
            {
                Color color = GameManager.instance.playerColors[i + 1];
                cursorImages[i].color = new Color(color.r - selectedBrightness, color.g - selectedBrightness, color.b - selectedBrightness);
            }
        }

        // set data for character names and portraits
        for (int i = 0; i < characterDisplays.Length; i++)
        {
            characterDisplays[i].character = selectedCharacters[i];
            characterDisplays[i].playerType = GameManager.instance.playerTypes[i];
            characterDisplays[i].ready = playerReady[i];
        }

        // start game if both players are ready
        if (playerReady[0] && playerReady[1])
        {
            for (int i = 0; i < selectedCharacters.Count; i++)
            {
                selectedCharacters[i]--;
                if (selectedCharacters[i] == -1)
                    selectedCharacters[i] = Random.Range(0, characters.Length);
                if (i == 1)
                    GameManager.instance.gameSettings.cpuChar = selectedCharacters[i];
            }
            GameManager.instance.playerCharacterIds.Clear();
            for (int i = 0; i < selectedCharacters.Count; i++)
                GameManager.instance.playerCharacterIds.Add((byte)selectedCharacters[i]);
            SceneManager.LoadScene("Game");
        }
    }
    public void TryUpdatePlayer(int type)
    {
        if (lobbyUI != null)
            if (lobbyUI.myLobby != null)
                lobbyUI.UpdatePlayer(type);
    }
}
