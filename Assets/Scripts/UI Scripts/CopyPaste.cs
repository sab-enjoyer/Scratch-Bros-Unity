using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopyPaste : MonoBehaviour
{
    [SerializeField]
    private TMP_Text joinCode;

    private Image myImage;
    private void Awake()
    {
        myImage = gameObject.GetComponent<Image>();
    }

    private void OnMouseEnter()
    {
        myImage.color = new Color(myImage.color.r, myImage.color.g, myImage.color.b, 0.5f);
    }

    private void OnMouseExit()
    {
        myImage.color = new Color(myImage.color.r, myImage.color.g, myImage.color.b, 1f);
    }
    public void CopyCode()
    {
        GUIUtility.systemCopyBuffer = joinCode.text.Substring(joinCode.text.Length - 6);
    }
}
