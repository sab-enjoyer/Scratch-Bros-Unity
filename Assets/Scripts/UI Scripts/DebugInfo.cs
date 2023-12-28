using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugInfo : MonoBehaviour
{
    private TMP_Text text;
    private RectTransform rectTransform;
    private Render render;
    private float timer = 0f;
    private string fps;
    public int focusedPlayerIdx = 0;

    void Start()
    {
        text = gameObject.GetComponent<TMP_Text>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        render = GameObject.Find("Render").GetComponent<Render>();
    }

    // Update is called once per frame
    void Update()
    {
        if (render.showBoxes && GameManager.instance.gameType == GameManager.GameTypes.Training)
        {
            if (render.Paused)
                rectTransform.anchoredPosition = new Vector2(128f, -338f);
            else
                rectTransform.anchoredPosition = new Vector2(128f, -50f);

            if (Input.GetKeyDown(KeyCode.F12))
            {
                focusedPlayerIdx = (int)Mathf.Repeat(focusedPlayerIdx+1f, render.playerAmount);
            }

            text.text = "DEBUG INFO";
            text.text += "\n";
            if (timer > 0.5f)
            {
                fps = "FPS: " + Mathf.Round((1 / Time.unscaledDeltaTime)*10)/10;
                timer = 0f;

            }
            else
            {
                timer += Time.deltaTime;
            }
            Player focusedPlayer = render.players[focusedPlayerIdx];
            text.text += fps;
            text.text += "\n";
            text.text += "Focused on player " + (focusedPlayerIdx + 1) + " (F12 to switch players)";
            text.text += "\n";
            text.text += "position x: " + focusedPlayer.pos.x + ", y: " + focusedPlayer.pos.y;
            text.text += "\n";
            text.text += "speed x: " + Mathf.Round(focusedPlayer.speed.x*100)/100 + ", y: " + Mathf.Round(focusedPlayer.speed.y/100)/100;
            text.text += "\n";
            text.text += "state: " + focusedPlayer.state + " (" + GameUtils.stateNames[focusedPlayer.state] + "), frame: " + focusedPlayer.frame;
            text.text += "\n";
            text.text += "double jump: " + focusedPlayer.doubleJump;
            text.text += "\n";
            text.text += "charge: " + focusedPlayer.charge + ", super armor: " + focusedPlayer.superArmor;

        }
        else
        {
            text.text = "";
        }

    }
}
