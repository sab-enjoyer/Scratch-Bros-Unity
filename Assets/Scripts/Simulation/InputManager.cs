using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public bool up, down, left, right, attack, shield;

    public InputManager()
    {
        up = false;
        down = false;
        left = false;
        right = false;
        attack = false;
        shield = false;
    }

    public void GetGameInputs(Player player)
    {
        if (player.playerType == GameManager.PlayerTypes.Local)
        {
            if (player.pIndex == 0)
            {
                if (GameManager.instance.gameType == GameManager.GameTypes.Player2)
                {
                    up = Input.GetButton("Up1");
                    down = Input.GetButton("Down1");
                    left = Input.GetButton("Left1");
                    right = Input.GetButton("Right1");
                    attack = Input.GetButton("Attack1");
                    shield = Input.GetButton("Shield1");
                }
                else
                {
                    up = Input.GetButton("Up1") || Input.GetButton("Up2");
                    down = Input.GetButton("Down1") || Input.GetButton("Down2");
                    left = Input.GetButton("Left1") || Input.GetButton("Left2");
                    right = Input.GetButton("Right1") || Input.GetButton("Right2");
                    attack = Input.GetButton("Attack1") || Input.GetButton("Attack2") || Input.GetButton("Attack3") || Input.GetButton("Special3");
                    shield = Input.GetButton("Shield1") || Input.GetButton("Shield2") || Input.GetButton("Shield3") || Input.GetButton("Special3");
                }
            }
            else
            {
                up = Input.GetButton("Up2");
                down = Input.GetButton("Down2");
                left = Input.GetButton("Left2");
                right = Input.GetButton("Right2");
                attack = Input.GetButton("Attack2");
                shield = Input.GetButton("Shield2");
            }

        }
        else if (player.playerType == GameManager.PlayerTypes.Bot || player.playerType == GameManager.PlayerTypes.Dummy)
        {
            if (player.pos.x > 22.4f || player.playerType == GameManager.PlayerTypes.Dummy)
            {
                up = player.pos.y < -6.4f;
                down = player.state == 23;
                left = player.pos.x > 19.2f;
                right = player.pos.x < -19.2f;
                attack = player.pos.y < -8f && player.doubleJump == 0;
                shield = false;
            }
        }
    }

    public void GetMenuInputs(int pIndex)
    {
        if (pIndex == 0)
        {
            up = Input.GetButtonDown("Up1");
            down = Input.GetButtonDown("Down1");
            left = Input.GetButtonDown("Left1");
            right = Input.GetButtonDown("Right1");
            attack = Input.GetButtonDown("Attack1");
            shield = Input.GetButtonDown("Shield1");
        }
        else if (pIndex == 1)
        {
            up = Input.GetButtonDown("Up2");
            down = Input.GetButtonDown("Down2");
            left = Input.GetButtonDown("Left2");
            right = Input.GetButtonDown("Right2");
            attack = Input.GetButtonDown("Attack2");
            shield = Input.GetButtonDown("Shield2");
        }
        else if (pIndex == -1)
        {
            up = Input.GetButtonDown("Up1") || Input.GetButtonDown("Up2");
            down = Input.GetButtonDown("Down1") || Input.GetButtonDown("Down2");
            left = Input.GetButtonDown("Left1") || Input.GetButtonDown("Left2");
            right = Input.GetButtonDown("Right1") || Input.GetButtonDown("Right2");
            attack = Input.GetButtonDown("Attack1") || Input.GetButtonDown("Attack2") || Input.GetButton("Attack3");
            shield = Input.GetButtonDown("Shield1") || Input.GetButtonDown("Shield2") || Input.GetButton("Shield3");
        }
    }

    public void setFalseInputs()
    {
        up = false;
        down = false;
        left = false;
        right = false;
        attack = false;
        shield = false;
    }

    public PlayerInput ToPlayerInput()
    {
        return new PlayerInput
        {
            up = up,
            down = down,
            left = left,
            right = right,
            attack = attack,
            shield = shield
        };
    }

    public void SetToPlayerInput(PlayerInput playerInput)
    {
        up = playerInput.up;
        down = playerInput.down;
        left = playerInput.left;
        right = playerInput.right;
        attack = playerInput.attack;
        shield = playerInput.shield;
    }
}