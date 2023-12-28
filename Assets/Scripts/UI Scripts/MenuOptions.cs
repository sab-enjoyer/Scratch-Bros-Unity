using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameManager;

[System.Serializable]
public class MenuOption
{
    public enum Type { onePlayer, twoPlayer, controls, offline, online, training, vault, meetTheCast, settings, about};
    public Type type;

    [HideInInspector]
    public int id;
    [HideInInspector]
    public GameObject uiObject;
    [HideInInspector]
    public Image image;
    [HideInInspector]
    public RectTransform transform;

    public void Update(Vector2 offset, float spacing, Sprite[] sprites)
    {
        uiObject.transform.position = new Vector3(offset.x, offset.y - (id * spacing));
        
        Sprite sprite;
        switch (type)
        {
            case Type.onePlayer:
                sprite = sprites[0];
                transform.localScale = Vector3.one;
                break;
            case Type.twoPlayer:
                sprite = sprites[1];
                transform.localScale = Vector3.one;
                break;
            case Type.controls:
                sprite = sprites[2];
                transform.localScale = Vector3.one;
                break;
            case Type.offline:
                sprite = sprites[3];
                transform.localScale = Vector3.one;
                break;
            case Type.online:
                sprite = sprites[4];
                transform.localScale = Vector3.one;
                break;
            case Type.training:
                sprite = sprites[5];
                transform.localScale = Vector3.one;
                break;
            case Type.vault:
                sprite = sprites[6];
                transform.localScale = Vector3.one;
                break;
            case Type.meetTheCast:
                sprite = sprites[7];
                transform.localScale = new Vector3(2, 2, 2);
                break;
            case Type.settings:
                sprite = sprites[8];
                transform.localScale = new Vector3(2, 2, 2);
                break;
            case Type.about:
                sprite = sprites[9];
                transform.localScale = new Vector3(2, 2, 2);
                break;

            default:
                sprite = sprites[0];
                break;
        }
        image.sprite = sprite;
    }

    public Vector2 Position
    {
        get { return uiObject.transform.position; }
    }

    public void Activate()
    {
        if (type == Type.twoPlayer)
        {
            instance.gameSettings.stocks = 3;
            instance.gameType = GameTypes.Player2;
            ReplacePlayerTypes(PlayerTypes.Local, PlayerTypes.Local);
        }
        else if (type == Type.onePlayer)
        {
            instance.gameSettings.stocks = 3;
            instance.gameType = GameTypes.Player1;
            ReplacePlayerTypes(PlayerTypes.Local, PlayerTypes.Bot);
        }
        else if (type == Type.training)
        {
            instance.gameSettings.stocks = 0;
            instance.gameType = GameTypes.Training;
            ReplacePlayerTypes(PlayerTypes.Local, PlayerTypes.Dummy);
        }
        else if (type == Type.online)
        {
            instance.gameSettings.stocks = 3;
            instance.gameType = GameTypes.Online;
            ReplacePlayerTypes(PlayerTypes.Local, PlayerTypes.Online);
        }

        if (type == Type.online)
            SceneManager.LoadScene("Connect");
        else if (type == Type.vault)
            SceneManager.LoadScene("Vault");
        else if (type == Type.about || type == Type.meetTheCast)
            return;
        //else if (type == Type.settings)
        //    SceneManager.LoadScene("Settings");
        else
            SceneManager.LoadScene("Options");
    }
    public void ReplacePlayerTypes(PlayerTypes type1, PlayerTypes type2)
    {
        instance.playerTypes = new() { type1, type2 };
    }
}

public class MenuOptions : MonoBehaviour
{
    public GameObject menuOptionPrefab;
    public Vector2 offset;
    public float spacing;
    public Sprite[] menuSprites;

    [Header("Insert Menu Options Here")]
    public MenuOption[] menuOptions;

    private void Start()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            menuOptions[i].uiObject = Instantiate(menuOptionPrefab, transform);
            menuOptions[i].image = menuOptions[i].uiObject.GetComponent<Image>();
            menuOptions[i].transform = menuOptions[i].uiObject.GetComponent<RectTransform>();
            menuOptions[i].id = i;
            menuOptions[i].Update(offset, spacing, menuSprites);
        }
    }

    private void Update()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            menuOptions[i].Update(offset, spacing, menuSprites);
        }
    }

}
