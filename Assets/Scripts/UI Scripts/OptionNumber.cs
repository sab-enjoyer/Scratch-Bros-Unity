using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionNumber : MonoBehaviour
{
    public int number;
    public bool show;
    [SerializeField] private Sprite[] numbersprites;
    [SerializeField] private Sprite infinite;
    private Image image;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
        show = true;
    }
    private void Update()
    {
        if (number == 0)
            image.sprite = infinite;
        else
            image.sprite = numbersprites[number];

        if (show)
            image.color = Color.black;
        else
            image.color = new Color(0f, 0f, 0f, 0.5f);
    }
}
