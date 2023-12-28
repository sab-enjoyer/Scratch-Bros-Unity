using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class CSSCharacter : MonoBehaviour
{
    [SerializeField]
    private Image nameDisplay;
    [SerializeField]
    private SpriteRenderer portraitDisplay, readyIcon;

    [SerializeField]
    private Sprite[] readyIconSprites;

    [SerializeField]
    private Sprite randomName, randomPortrait;

    public int playerID;
    public int character;
    public bool ready;
    public PlayerTypes playerType;

    private void Update()
    {
        if (character == 0 && (instance.gameType != GameTypes.Online))
        {
            nameDisplay.sprite = randomName;
            portraitDisplay.sprite = randomPortrait;
        }
        else
        {
            nameDisplay.sprite = instance.characters[character- (instance.gameType == GameTypes.Online ? 0 : 1)].nameSprite;
            portraitDisplay.sprite = instance.characters[character-(instance.gameType == GameTypes.Online ? 0 : 1)].portrait;
        }
        if (playerID == 1)
        {
            portraitDisplay.flipX = character != 0;
        }

        readyIcon.gameObject.SetActive(ready);
        if (ready)
        {
            if (playerType != PlayerTypes.Local)
                readyIcon.sprite = readyIconSprites[0];
            else if (playerID == 0)
                readyIcon.sprite = readyIconSprites[1];
            else
                readyIcon.sprite = readyIconSprites[2];
        }
    }
}
