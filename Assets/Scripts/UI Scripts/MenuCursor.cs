using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCursor : MonoBehaviour
{
    public Vector2 offset;

    private MenuOptions menuOptions;
    InputManager inputs = new();

    [HideInInspector]
    public int id = 0;

    void Start()
    {
        menuOptions = GameObject.Find("Menu").GetComponent<MenuOptions>();

        //get rid of extra network managers
        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        inputs.GetMenuInputs(-1);
        if (inputs.up)
        {
            id = (int)Mathf.Repeat(id - 1, menuOptions.menuOptions.Length);
        }
        if (inputs.down)
        {
            id = (int)Mathf.Repeat(id + 1, menuOptions.menuOptions.Length);
        }

        MenuOption currentMenuOption = menuOptions.menuOptions[id];
        gameObject.transform.position = new Vector3(currentMenuOption.Position.x + offset.x, currentMenuOption.Position.y + offset.y);

        if (Time.timeSinceLevelLoad > 0.1f)
        {
            if (inputs.attack || Input.GetKeyDown(KeyCode.Space))
            {
                currentMenuOption.Activate();
            }
            if (inputs.shield && SceneManager.GetActiveScene().name == "Vault")
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
}
