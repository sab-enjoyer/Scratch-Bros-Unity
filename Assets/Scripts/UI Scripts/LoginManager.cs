using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    private GameObject loginScreen, nameScreen, load;

    private InputManager inputs = new();

    public bool LoggingIn
    {
        get { return _loggingIn; }
        set { 
            _loggingIn = value;
            load.SetActive(_loggingIn);
        }
    }
    private bool _loggingIn = false;

    public bool hasLoggedIn = false;

    private void Start()
    {
        load.SetActive(false);
        nameScreen.SetActive(false);
        loginScreen.SetActive(true);
    }

    private void Update()
    {
        if (LoggingIn || hasLoggedIn)
            return;

        inputs.GetMenuInputs(-1);

        if (inputs.attack)
        {
            Login();
        }
        else if (inputs.shield)
        {
            SceneManager.LoadScene("Title");
        }
    }

    public async void Login()
    {
        LoggingIn = true;
        await Authentication.Login();

        if (AuthenticationService.Instance.IsSignedIn)
        {
            loginScreen.SetActive(false);
            if (PlayerPrefs.GetString("username") != string.Empty)
                SceneManager.LoadScene("Character Select");
            else
                nameScreen.SetActive(true);

            LoggingIn = false;
            hasLoggedIn = true;
        }
    }
}
