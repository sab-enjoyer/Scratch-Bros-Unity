using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private Render render;
    public int cursorId;
    public GameObject cursor;
    public int optionAmt;
    public float[] cursorPositions;

    private void Awake()
    {
        render = GameObject.Find("Render").GetComponent<Render>();
    }

    private void OnEnable()
    {
        cursorId = 0;
    }

    private void LateUpdate()
    {
        if (Input.GetButtonDown("Up1") || Input.GetButtonDown("Up2"))
            cursorId = (int)Mathf.Repeat(cursorId - 1, optionAmt);

        if (Input.GetButtonDown("Down1") || Input.GetButtonDown("Down2"))
            cursorId = (int)Mathf.Repeat(cursorId + 1, optionAmt);

        if (Input.GetButtonDown("Left1") || Input.GetButtonDown("Left2"))
        {
            if (render.savedFrames.ContainsKey(render.gameFrame - 1))
            {
                SaveState saveState = render.savedFrames[render.gameFrame - 1];
                render.ReplaceRenderLists(saveState, true);

                render.RunFrame();
            }
        }

        if (Input.GetButtonDown("Right1") || Input.GetButtonDown("Right2"))
            render.RunFrame();

        cursor.GetComponent<RectTransform>().anchoredPosition = new Vector3(28f, cursorPositions[cursorId]);
    }
}
