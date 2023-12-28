using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveLoadState : MonoBehaviour
{
    public SaveStateManager saveStateManager;
    private TMP_Text text;
    public Gradient fadeOut;
    public int fadeTime;
    private Render render;

    private void Start()
    {
        render = GameObject.Find("Render").GetComponent<Render>();
        text = gameObject.GetComponent<TMP_Text>();
        text.color = Color.clear;
    }

    private void LateUpdate()
    {
        if (render.Paused)
            return;

        if (Input.GetKeyDown(KeyCode.O))
        {
            saveStateManager.saveState = saveStateManager.SaveToState();
            text.text = "STATE SAVED";
            StopAllCoroutines();
            StartCoroutine(FadeText());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (saveStateManager.LoadState())
                text.text = "STATE LOADED";
            else
                text.text = "NO STATE TO LOAD";
            StopAllCoroutines();
            StartCoroutine(FadeText());
        }

    }

    private IEnumerator FadeText()
    {
        for (int i = 0; i < fadeTime; i++)
        {
            text.color = fadeOut.Evaluate((float)i / fadeTime);
            yield return null;
        }
        text.color = Color.clear;
    }

}
