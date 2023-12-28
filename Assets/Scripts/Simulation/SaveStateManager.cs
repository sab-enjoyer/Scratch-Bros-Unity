using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// Stores the data of a frame into a class. The data includes players, projectiles, hitboxes, and the gameframe.
public class SaveState
{
    public int gameFrame;
    public int timeLeft;
    public PlayerSaveData[] playerSaveData;
    public ProjectileSaveData[] projectiles;
    public Hitbox[] hitboxes;

    public SaveState(int _gameFrame, int _timeLeft, PlayerSaveData[] _playerSaveData, ProjectileSaveData[] _projectileSaveData, Hitbox[] _hitboxes)
    {
        gameFrame = _gameFrame;
        timeLeft = _timeLeft;
        playerSaveData = _playerSaveData;
        projectiles = _projectileSaveData;
        hitboxes = _hitboxes;
    }

}

public class PlayerSaveData
{
    public byte charIndex;
    public byte pIndex;
    public Vector2 pos;
    public Vector2 speed;
    public bool onGround;
    public byte doubleJump, state;
    public int stocks;
    public int charge, superArmor;
    //public bool up, down, left, right, attack, shieldPressed;

    public int frame, dir, shield, invin, ledgeFrame, damage;

    public PlayerInput playerInput;

    public PlayerSaveData(Player _player)
    {
        charIndex = _player.charIndex;
        pIndex = _player.pIndex;
        pos = _player.pos;
        speed = _player.speed;
        onGround = _player.onGround;
        doubleJump = _player.doubleJump;

        state = _player.state;
        frame = _player.frame;
        dir = _player.dir;
        stocks = _player.stocks;
        shield = _player.shield;
        invin = _player.invin;
        ledgeFrame = _player.ledgeFrame;
        damage = _player.damage;
        charge = _player.charge;
        superArmor = _player.superArmor;
        playerInput = _player.inputs.ToPlayerInput();
    }
}

public class ProjectileSaveData
{
    public Vector2 pos, speed;
    public int dir, frame;
    public byte type, creator, listID, touching;

    public ProjectileSaveData(Projectile _projectile)
    {
        pos = _projectile.pos;
        speed = _projectile.speed;
        type = _projectile.type;
        creator = _projectile.creator;
        dir = _projectile.dir;
        frame = _projectile.frame;
        listID = _projectile.listID;
        touching = _projectile.touching;
    }
}

[System.Serializable]
public struct PlayerInput
{
    public bool up, down, left, right, attack, shield;
}

public class SaveStateManager : MonoBehaviour
{
    private Render render;
    public SaveState saveState;
    private void Start()
    {
        render = GameObject.Find("Render").GetComponent<Render>();
    }


    public SaveState SaveToState()
    {
        List<PlayerSaveData> playerList = new();

        for (int i = 0; i < render.players.Count; i++)
        {
            playerList.Add(new PlayerSaveData(render.players[i]));
        }

        List<ProjectileSaveData> projectileList = new();

        for (int i = 0; i < render.projectiles.Count; i++)
        {
            projectileList.Add(new ProjectileSaveData(render.projectiles[i]));
        }

        return new SaveState(render.gameFrame, render.timeLeft, playerList.ToArray(), projectileList.ToArray(), render.hitboxes.ToArray());
    }

    public bool LoadState()
    {
        if (saveState != null)
        {
            render.ReplaceRenderLists(saveState);
            return true;
        }
        else
        {
            return false;

        }
    }
}
