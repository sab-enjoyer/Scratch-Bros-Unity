using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public Image characterDisplay;
    public Image nameDisplay;
    private CharacterData[] characters;

    private Color[] playerColors;

    private void Start()
    {
        if (GameManager.instance != null)
        {
            playerColors = GameManager.instance.playerColors;
            characters = GameManager.instance.characters;
        }
        else
            return;
        if (GameManager.instance.playerStore.Count == 0) 
            return;

        int gameWinner = GameManager.instance.gameWinner;
        gameObject.GetComponent<Image>().color = playerColors[gameWinner + 1];
        characterDisplay.sprite = characters[GameManager.instance.playerStore[gameWinner].charIndex].portrait;
        characterDisplay.SetNativeSize();
        nameDisplay.sprite = characters[GameManager.instance.playerStore[gameWinner].charIndex].nameSprite;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene("Stage Select");
    }
}
