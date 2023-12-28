using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData : MonoBehaviour
{
    private Render render;

    public Hitbox[] hitboxList;
    private void Start()
    {
        render = GameObject.Find("Render").GetComponent<Render>();
    }

    public void NeutralGround(int pIndex)
    {
        Player player = render.players[pIndex]; 

        if (player.charData.charName == "Road Combatant")
        {
            player.frame++;
            if (player.frame == 10) render.CreateProjectile(2.4f, 0.4f, "fireball", player.pIndex);
            if (player.frame > 17) player.state = 0;
        }
        else if (player.charData.charName == "Medium Mike")
        {
            player.frame++;
            if (Mathf.RoundToInt(player.frame / 2) % 2 == 1) CreateHitbox(pIndex, 12);
            if (!player.inputs.attack || (player.inputs.shield || player.inputs.right || player.inputs.left || player.inputs.down))
            {
                player.state = 0;
                player.frame = 1;
            }
        }
    }

    public void SideGround(int pIndex)
    {
        Player player = render.players[pIndex];

        if (player.charData.charName == "Road Combatant")
        {
            player.frame++;
            if (player.frame < 20)
            {
                if (player.frame < 16) player.speed.x = player.dir * 1.7f;
                if (player.frame == 4) CreateHitbox(pIndex, 0);
                if (player.frame == 7) CreateHitbox(pIndex, 1);
                if (player.frame == 13) CreateHitbox(pIndex, 2);
            }
            else
                player.state = 0;
        }
        else if (player.charData.charName == "Medium Mike")
        {
            player.frame++;
            if (player.frame < 15)
            {
                if (player.frame == 3) player.speed.x = player.dir;
                else if (player.frame == 5) CreateHitbox(pIndex, 13);
                else if (player.frame == 10) CreateHitbox(pIndex, 14);
            }
            else player.state = 0;
        }
    }

    public void DownGround(int pIndex)
    {
        Player player = render.players[pIndex];

        if (player.charData.charName == "Road Combatant")
        {
            player.frame++;
            if (player.frame == 1)
            {
                CreateHitbox(pIndex, 3);
                CreateHitbox(pIndex, 4);
            }
            if (player.frame > 5)
            {
                player.frame = 0;
                player.state = 4;
            }
        }
        else if (player.charData.charName == "Medium Mike")
        {
            player.frame++;
            if (player.frame > 83 && 90 > player.frame) player.speed.x = player.dir * 0.5f;
            if (80 > player.frame && player.frame > 4)
            {
                if (!player.inputs.attack || player.frame == 79)
                {
                    CreateHitbox(pIndex, 15, player.frame);
                    player.frame = 80;
                }
                if (player.inputs.right) player.dir = 1;
                if (player.inputs.left) player.dir = -1;
            }
            if (player.frame > 90 && !player.inputs.attack) player.state = 0;
        }
    }

    public void NeutralAir(int pIndex)
    {
        Player player = render.players[pIndex];

        if (player.charData.charName == "Road Combatant")
        {
            player.frame++;
            if (8 > player.frame && player.frame > 2) CreateHitbox(pIndex, 5);
            if (player.frame > 10) player.state = 2;
        }
        else if (player.charData.charName == "Medium Mike")
        {
            player.frame++;
            if (player.frame > 2) CreateHitbox(pIndex, 16);
            if (player.frame > 5) player.state = 2;
        }
    }
    public void SideAir(int pIndex)
    {
        Player player = render.players[pIndex];

        if (player.charData.charName == "Road Combatant")
        {
            player.frame++;
            if (player.frame < 18)
            {
                if (player.frame < 14 && player.frame > 4)
                {
                    CreateHitbox(pIndex, 6);
                    player.speed.y = Mathf.Clamp(player.speed.y, -0.3f, 0);
                    player.speed.y += 0.25f;
                    player.speed.x += 0.2f * player.dir;
                }
                    
            }
            else
                player.state = 2;
        }
        else if (player.charData.charName == "Medium Mike")
        {
            player.frame++;
            if (player.frame < 12)
            {
                if (player.frame == 1) player.speed.y = 1.2f;
                player.speed.x = 1.2f * player.dir;
                if (player.frame == 4) CreateHitbox(pIndex, 17);
                if (player.frame == 8) CreateHitbox(pIndex, 18);
            }
            if (!player.inputs.attack && player.frame > 20) player.state = 0;
        }
    }

    public void DownAir(int pIndex)
    {
        Player player = render.players[pIndex];

        if (player.charData.charName == "Road Combatant")
        {
            player.frame++;
            if (player.frame > 4 && 7 > player.frame) CreateHitbox(pIndex, 7);
            if (player.frame > 12) player.state = 2;
        }
        else if (player.charData.charName == "Medium Mike")
        {
            player.frame++;
            if (player.frame > 2) CreateHitbox(pIndex, 19);
            if (player.frame > 5) player.state = 2;
        }
    }

    public void Recovery(int pIndex)
    {
        Player player = render.players[pIndex];

        if (player.charData.charName == "Road Combatant")
        {
            player.frame++;
            if (6 > player.frame)
            {
                if (player.frame < 2)
                    CreateHitbox(pIndex, 8);
                else
                {
                    player.speed.y = 3.0f;
                    player.speed.x = player.dir * 0.5f;
                    CreateHitbox(pIndex, 9);
                }
            }
            else
            {
                if (player.frame == 6) player.speed.y = 1.0f;
                if (player.frame > 12) player.state = 15;
            }
        }
        else if (player.charData.charName == "Medium Mike")
        {
            player.frame++;
            player.speed = new Vector2(0, 1.5f);
            CreateHitbox(pIndex, 20);
            if (player.frame > 10)
            {
                player.speed.y = 0.8f;
                player.state = 15;
            }

        }
    }
    public void Special(int pIndex)
    {
        Player player = render.players[pIndex];

        if (player.charData.charName == "Road Combatant")
        {
            player.frame++;
            player.speed.y += 0.1f;
            player.superArmor = 1;
            if (40 > player.frame && player.frame > 10)
            {
                if (!player.inputs.attack || player.frame == 39)
                {
                    if (player.inputs.right) player.dir = 1;
                    if (player.inputs.left) player.dir = -1;
                    player.speed.x = 3 * player.dir;
                    if (15 > player.frame) CreateHitbox(pIndex, 10);
                    else if (30 > player.frame) CreateHitbox(pIndex, 11);
                    else CreateHitbox(pIndex, 11);
                    player.frame = 40;
                }
            }
            if (player.frame == 42) player.speed.x = 0;
            if (player.frame > 50) player.state = 0;
        }
        else if (player.charData.charName == "Medium Mike")
        {
            player.frame++;
            if (player.charge < 200)
            {
                if (player.frame > 16) player.state = 0;
            }
            else
            {
                if (player.frame < 4) player.speed.y = 0;
                if (player.frame == 4)
                {
                    player.speed = new Vector2(player.onGround ? 0.5f : 1.2f * player.dir, 1.5f);
                    CreateHitbox(pIndex, player.onGround ? 21 : 20);
                }
                if (player.frame > 16)
                {
                    player.charge = 0;
                    player.state = 15;
                }
            }
        }
    }
    public void CreateHitbox(int pIndex, int id, int frame = default)
    {
        Hitbox getHitbox = hitboxList[id];
        int damage = getHitbox.damage;
        if (getHitbox.damage == -1)
        {
            if (getHitbox.name == "mm down z") damage = 13 + Mathf.RoundToInt(frame / 5);
        }
        render.CreatePlayerHitbox(pIndex, getHitbox.pos, getHitbox.size, getHitbox.multiplier, damage, getHitbox.type);
    }
}
