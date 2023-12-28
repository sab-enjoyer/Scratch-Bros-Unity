using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Hitbox
{
    public string name;
    private Render render;
    private GameObject rendererObj;
    private LineRenderer lineRenderer;
    public Vector2 pos;
    public Vector2 size;
    public Vector2 multiplier;
    public int damage;
    [HideInInspector]
    public byte sender;
    [HideInInspector]
    public bool fresh;
    public byte type;


    public Hitbox(Player player, Vector2 _offset, Vector2 _size, Vector2 _mult, int _damage, byte _type = default)
    {
        render = GameObject.Find("Render").GetComponent<Render>();
        sender = player.pIndex;
        pos = new Vector2(player.pos.x + (_offset.x * player.dir) + player.speed.x, player.pos.y + _offset.y + player.speed.y); // Sets position to player position + offset + player speed
        size = _size;
        multiplier = _mult;
        damage = _damage;
        fresh = true;
        type = _type;

        if (render.showBoxes)
            CreateRenderer();
    }

    public Hitbox(Projectile projectile, Vector2 _offset, Vector2 _size, Vector2 _mult, int _damage, byte _type = default)
    {
        render = GameObject.Find("Render").GetComponent<Render>();
        sender = projectile.creator;
        pos = new Vector2(projectile.pos.x + (_offset.x * projectile.dir) + projectile.speed.x, projectile.pos.y + _offset.y + projectile.speed.y); // Sets position to player position + offset + player speed
        size = _size;
        multiplier = _mult;
        damage = _damage;
        fresh = true;
        type = _type;

        if (render.showBoxes)
            CreateRenderer();
    }

    public void CreateRenderer()
    {
        rendererObj = GameObject.Find("Hitbox Pooler").GetComponent<HitboxPooler>().SpawnFromPool();
        lineRenderer = rendererObj.GetComponent<LineRenderer>();

        if (sender % 2 == 0)
            lineRenderer.colorGradient = render.p1Gradient;
        else
            lineRenderer.colorGradient = render.p2Gradient;
        lineRenderer.SetPositions(GameUtils.DrawBox(pos, size).ToArray());
    }

    public void SetInactive()
    {
        if (rendererObj != null)
            rendererObj.SetActive(false);
    }
}