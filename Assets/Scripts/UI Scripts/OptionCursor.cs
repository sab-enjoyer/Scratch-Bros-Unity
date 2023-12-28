using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionCursor : MonoBehaviour
{
    [SerializeField]
    private Vector2[] cursorPositions;
    [SerializeField]
    private Sprite[] cursorSprites;
    private RectTransform rectTransform;
    private Image image;
    private InputManager inputs = new();
    [SerializeField]
    private GameObject displayNumber;
    [SerializeField]
    private RectTransform canvas;
    [SerializeField]
    private Image charImage;
    [SerializeField]
    private Sprite randomSprite;
    [SerializeField]
    private Sprite[] bgSprites;
    [SerializeField]
    private Image bgSpriteRef;

    private List<OptionNumber> numbers = new();
    private GameSettings gameSettings = GameManager.instance.gameSettings;
    public int id;
    private int minID, maxID;

    private void Awake()
    {
        id = 0;
        rectTransform = gameObject.GetComponent<RectTransform>();
        image = gameObject.GetComponent<Image>();

        for (int i = 0; i < cursorPositions.Length-1; i++)
        {
            GameObject obj = Instantiate(displayNumber, canvas);
            numbers.Add(obj.GetComponent<OptionNumber>());
            obj.GetComponent<RectTransform>().anchoredPosition = cursorPositions[i];
        }

        minID = 0;
        maxID = 3;
        if (GameManager.instance.gameType == GameManager.GameTypes.Training)
        {
            numbers[0].show = false;
            numbers[1].show = false;
            bgSpriteRef.sprite = bgSprites[2];
            id = 2;
            minID = 2;
        }
        else if (GameManager.instance.gameType == GameManager.GameTypes.Player2)
        {
            numbers[2].show = false;
            bgSpriteRef.sprite = bgSprites[0];
            maxID = 1;
        }
        else
        {
            bgSpriteRef.sprite = bgSprites[1];
        }
    }

    private void Update()
    {
        inputs.GetMenuInputs(-1);
        int temp = 0;
        if (inputs.up)
            temp--;
        else if (inputs.down)
            temp++;

        if (temp != 0)
        {
            id += temp;
            id = (int)Mathf.Repeat(id, maxID+1);
            while (id < minID)
            {
                id++;
            }
        }
        rectTransform.anchoredPosition = cursorPositions[id];
        image.sprite = cursorSprites[id];

        int input = 0;
        if (inputs.left)
            input = -1;
        else if (inputs.right)
            input = 1;

        if (id == 0)
            gameSettings.stocks = (int)Mathf.Repeat(gameSettings.stocks + input, 10);
        else if (id == 1)
            gameSettings.time = (int)Mathf.Repeat(gameSettings.time + input, 10);
        else if (id == 2)
            gameSettings.cpuLevel = (int)Mathf.Repeat(gameSettings.cpuLevel + input - 1, 5) + 1;
        else if (id == 3)
            gameSettings.cpuChar = (int)Mathf.Repeat(gameSettings.cpuChar + input + 1, GameManager.instance.characters.Length+1) - 1;

        numbers[0].number = gameSettings.stocks;
        numbers[1].number = gameSettings.time;
        numbers[2].number = gameSettings.cpuLevel;
        if (gameSettings.cpuChar == -1)
            charImage.sprite = randomSprite;
        else
            charImage.sprite = GameManager.instance.characters[gameSettings.cpuChar].icon;

        if (GameManager.instance.gameType == GameManager.GameTypes.Player2)
            charImage.color = new Color(1f, 1f, 1f, 0.5f);
        else
            charImage.color = Color.white;

        if (Time.timeSinceLevelLoad > 0.1f)
        {
            if (inputs.attack || Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.instance.gameSettings = gameSettings;
                SceneManager.LoadScene("Stage Select");
            }
            else if (inputs.shield)
                SceneManager.LoadScene(GameManager.instance.gameType == GameManager.GameTypes.Training ? "Vault" : "Title");
        }
    }
}
