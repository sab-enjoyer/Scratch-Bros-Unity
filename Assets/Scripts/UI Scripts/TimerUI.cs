using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    private TMP_Text text;
    private Render render;
    private string timeString;

    private void Awake()
    {
        text = gameObject.GetComponent<TMP_Text>();
        render = GameObject.Find("Render").GetComponent<Render>();
    }

    private void Update()
    {
        if (render.timeLeft < 2)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        int time = Mathf.FloorToInt(render.timeLeft / 30);
        int seconds = time % 60;
        int minutes = Mathf.FloorToInt(time / 60);

        text.text = string.Format("{0}:{1:00}", minutes, seconds);
    }
}
