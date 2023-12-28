using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameAnim : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite[] countDownSprites;

    [SerializeField]
    private Gradient fadeOut;

    private bool hasFaded = false;

    public void Countdown(int frame)
    {
        if (hasFaded) return;

        if (frame < 91)
        {
            image.sprite = countDownSprites[Mathf.FloorToInt(frame / 30)];
            Color color = fadeOut.Evaluate(((frame % 30 * 5) - 50)/100f);
            image.color = color;
        }
        else
        {
            hasFaded = true;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        for (int i = 0; i < 30; i++)
        {
            image.color = fadeOut.Evaluate(i / 30f);
            yield return null;
        }
        image.color = Color.clear;
    }

    public void ShowGameText()
    {
        StopAllCoroutines();
        image.sprite = countDownSprites[4];
        StartCoroutine(FadeOut());
    }
}
