using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile
{
    public Vector2 pos, speed;
    public int dir, frame;
    public byte type, creator, listID, touching;
    public GameObject renderer;
    public ProjRendererPooler rendererPooler;

    private Render render;
    public Projectile(byte _listID)
    {
        listID = _listID;
        speed = Vector2.zero;
        type = 0;
        creator = 0;
        dir = 1;
        frame = 0;
        touching = 0;

        render = GameObject.Find("Render").GetComponent<Render>();
        rendererPooler = GameObject.Find("Proj Renderer Pooler").GetComponent<ProjRendererPooler>();
    }

    public void LoadStateData(ProjectileSaveData _projectile)
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

    public void Render() // CALLED EVERY FRAME
    {
        if (type == 0) // if projectile is empty
            return;
        if (renderer == null)
        {
            renderer = rendererPooler.SpawnFromPool();
            renderer.GetComponent<ProjectileRenderer>().myProjectile = this;
        }
        if (!renderer.activeSelf)
            renderer.SetActive(true);
        Player myPlayer = render.players[creator];

        if (type == 1)
        {
            pos.x += dir * 0.5f;
            frame++;
            if (frame < 35)
            {
                render.CreateProjectileHitbox(listID, 0f, 0f, 2f, 2f, 0.25f * dir * myPlayer.dir, 0.25f, 6);
                DetectPlayers(new Vector2(2f, 2f));
                if (touching > 0 && frame < 34)
                    frame = 34;
            }
            else if (frame == 40)
                type = 0;
        }
        else if (type == 2)
        {
            frame++;
            if (frame < 100)
            {
                pos.y = myPlayer.pos.y - (myPlayer.hurtbox.size.y / 2);
                pos.x = myPlayer.pos.x;
                if (myPlayer.state != 23) frame = 100;
            }
            else
            {
                pos.y += (frame - 100)/10;
                if (frame > 120) type = 0;
            }
        }
    }

    public void DetectPlayers(Vector2 size)
    {
        int mult = GameUtils.collisionMult;
        touching = 0;
        for (int i = 0; i < render.players.Count; i++)
        {
            Player setPlayer = render.players[i];

            if (creator != i)
            {
                if (Collision.IsBoxesCollided(Collision.RoundVector2ToInt(pos - setPlayer.hurtbox.offset, mult), Collision.RoundVector2ToInt(size, mult), Collision.RoundVector2ToInt(setPlayer.pos, mult), Collision.RoundVector2ToInt(setPlayer.hurtbox.size, mult)))
                {
                    touching = 1;
                }

            }
        }

        for (int i = 0; i < render.groundBoxes.Length; i++)
        {
            if (render.groundBoxes[i].size.y != 0)
            {
                GroundBox setGroundBox = render.groundBoxes[i];
                if (Collision.IsBoxesCollided(Collision.RoundVector2ToInt(pos, mult), Collision.RoundVector2ToInt(size, mult), Collision.RoundVector2ToInt(setGroundBox.pos, mult), Collision.RoundVector2ToInt(setGroundBox.size, mult)))
                {
                    if (touching == 1)
                        touching = 3;
                    else
                        touching = 2;
                }

            }
        }
    }
}