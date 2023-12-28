using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnlinePlayerDisplay : MonoBehaviour
{
    [SerializeField]
    private Outline outline;

    [SerializeField]
    private Image displayChar;

    [SerializeField]
    private TMP_Text displayName, readyDisplay;

    [SerializeField]
    private Color readyColor, notReadyColor;

    [HideInInspector]
    public int character;

    [HideInInspector]
    public string username;

    [HideInInspector]
    public bool ready;

    public int playerId;

    private void Update()
    {
        if (string.IsNullOrEmpty(username))
        {
            displayName.text = "waiting on player...";
            outline.effectColor = GameManager.instance.playerColors[0];
            displayChar.gameObject.SetActive(false);
            readyDisplay.gameObject.SetActive(false);
        }
        else
        {
            displayName.text = username;
            outline.effectColor = GameManager.instance.playerColors[playerId + 1];
            displayChar.gameObject.SetActive(true);
            readyDisplay.gameObject.SetActive(true);
            displayChar.sprite = GameManager.instance.characters[character].icon;
            if (ready)
            {
                readyDisplay.color = readyColor;
                readyDisplay.text = "ready";
            }
            else
            {
                readyDisplay.color = notReadyColor;
                readyDisplay.text = "not ready";
            }
        }
    }

    public bool IsReady()
    {
        return ready && (!string.IsNullOrEmpty(username));
    }
}
