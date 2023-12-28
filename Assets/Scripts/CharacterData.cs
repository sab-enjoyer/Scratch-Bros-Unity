using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CharacterData", menuName ="Scratch Bros/Character Data")]
public class CharacterData : ScriptableObject
{
    public string charName;

    [Header("Physics")]
    public Vector2 collisionSize;
    public float weight;

    [Header("Movement")]
    public float fallSpeed;
    public float maxFallSpeed;
    public float runSpeed;
    public float airSpeed;
    public float slipperiness;
    public float jumpHeight;

    [Header("Misc")]
    public int shieldSize;
    public int rollSpeed;
    public int tauntLength;

    public Material[] altSkins;

    public PlayerHurtbox[] hurtboxes;

    public Sprite[] sprites;

    public Sprite portrait;
    public Sprite nameSprite;
    public Sprite icon;

    public AudioClip theme;
}

[System.Serializable]
public class PlayerHurtbox
{
    public Vector2 offset;
    public Vector2 size;
    public float state;
    public PlayerHurtbox()
    {
        offset = new();
        size = new();
    }

    public PlayerHurtbox(Vector2 _offset, Vector2 _size, float _state)
    {
        offset = _offset;
        size = _size;
        state = _state;
    }

    public PlayerHurtbox FixHurtboxDirection(int dir)
    {
        return new PlayerHurtbox(new Vector2(offset.x * dir, offset.y), size, state);
    }
}