using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scratch Bros/Map Data")]
public class MapData : ScriptableObject
{
    public string mapName;

    public Sprite sprite;

    public AudioClip song;

    public Material material;

    public Vector2 offset;

    public Vector3 stageSelectOffset;

    public GroundBox[] groundBoxes;

    public ParallaxData parallax1, parallax2, parallax3, foreground;

    public Sprite mapIcon;

    public Sprite mapNameSprite;
}
