using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Header : MonoBehaviour
{
    private Dictionary<GameManager.GameTypes, string> gameTypeNames = new()
    {
        {GameManager.GameTypes.Player2, "2 Player" },
        {GameManager.GameTypes.Player1, "1 Player" },
        {GameManager.GameTypes.Training, "Training" },
        {GameManager.GameTypes.Online, "Online" }
    };
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Vault" || SceneManager.GetActiveScene().name == "Settings")
            gameObject.GetComponent<TMP_Text>().text = SceneManager.GetActiveScene().name;
        else
            gameObject.GetComponent<TMP_Text>().text = gameTypeNames[GameManager.instance.gameType];
    }
}
