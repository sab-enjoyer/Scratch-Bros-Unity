using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UsernameEditor : MonoBehaviour
{
    [SerializeField]
    private GameObject confirm;
    [SerializeField]
    private TMP_Text inputText;
    [SerializeField]
    private TMP_InputField inputField;

    private bool confirming = false;
    InputManager inputs = new();

    // Start is called before the first frame update
    void Start()
    {
        confirm.SetActive(false);
    }

    public void OnEndEdit()
    {
        if (inputText.text.Length < 4 || inputText.text.Length > 13)
            return;
        confirm.SetActive(true);
        confirming = true;
        inputField.interactable = false;
    }

    private void Update()
    {
        inputs.GetMenuInputs(-1);
        if (confirming)
        {
            if (inputs.attack)
            {
                PlayerPrefs.SetString("username", inputText.text.ToLower());
                PlayerPrefs.Save();
                SceneManager.LoadScene("Character Select");
            }
            else if (inputs.shield)
            {
                confirm.SetActive(false);
                confirming = false;
                inputField.interactable = true;
            }

        }
        else
        {
            if (inputs.attack && !inputField.isFocused)
            {
                EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
                inputField.OnPointerClick(new PointerEventData(EventSystem.current));
            }
        }
    }
}
