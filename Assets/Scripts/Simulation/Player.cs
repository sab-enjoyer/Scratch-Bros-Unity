using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;
using static UnityEngine.ParticleSystem;

public class Player
{
    private GameObject playerObject;
    private LineRenderer lineRenderer;
    private SpriteRenderer spriteRenderer;
    private ParticleSystem hitEffect;
    private Render render;
    public CharacterData charData;
    public PlayerHurtbox hurtbox;
    private Transform transform;
    private List<TrailRenderer> myTrails = new();
    private List<ParticleSystem> myEffects = new();

    public PlayerTypes playerType;
    public byte charIndex;
    public byte pIndex;
    public Vector2 pos;
    public Vector2 speed;
    public bool onGround;
    public byte doubleJump, state;
    public int stocks;
    public InputManager inputs = new InputManager();
    public int dir;
    public int charge, superArmor;

    public int frame, shield, invin, ledgeFrame, damage;

    public byte altSkin = 0;

    public Player(byte _playerIndex, byte _charIndex, PlayerTypes _playerType)
    {
        pIndex = _playerIndex;
        charIndex = _charIndex;
        playerType = _playerType;
        pos = new Vector2(0f, 5f);
        speed = new Vector2(0, 0);
    }

    public void Init(bool setPosition)
    {
        altSkin = pIndex;
        playerObject = new GameObject("Player " + pIndex);
        playerObject.transform.position = pos;

        render = GameObject.Find("Render").GetComponent<Render>();
        charData = render.charStats[charIndex];

        if (setPosition)
            pos = GameUtils.SpawnPosition(pIndex, render.mapId);

        playerObject.transform.SetParent(render.transform);
        transform = playerObject.GetComponent<Transform>();
        transform.position = pos;
        lineRenderer = Object.Instantiate(render.boxRendererPrefab, playerObject.transform).GetComponent<LineRenderer>();
        spriteRenderer = playerObject.AddComponent<SpriteRenderer>();
        spriteRenderer.material = charData.altSkins[altSkin];
        hitEffect = Object.Instantiate(render.effectPrefabs[0], playerObject.transform).GetComponent<ParticleSystem>();

        if (instance.gameType == GameTypes.Training)
            state = 0;
        else
            state = 21;
        frame = 0;
        dir = (pIndex % 2 * -2) + 1;
        stocks = instance.gameSettings.stocks;
        damage = 0;
        shield = 150;
        charge = 0;
        superArmor = 0;
        onGround = true;

        hurtbox = FindHurtbox();
        if (charData.charName == "Road Combatant")
        {
            myTrails.Add(Object.Instantiate(render.effectPrefabs[1], playerObject.transform).GetComponent<TrailRenderer>());
            myEffects.Add(Object.Instantiate(render.effectPrefabs[2], playerObject.transform).GetComponent<ParticleSystem>());
        }
    }

    public void Draw()
    {
        if (!render.showBoxes)
            lineRenderer.enabled = false;
        else
        {
            lineRenderer.enabled = true;
            lineRenderer.startWidth = render.lineThickness;
            lineRenderer.endWidth = render.lineThickness;
            if (pIndex % 2 == 0)
                lineRenderer.colorGradient = render.p1Gradient;
            else
                lineRenderer.colorGradient = render.p2Gradient;

            lineRenderer.SetPositions(GameUtils.DrawBox(pos + hurtbox.offset, hurtbox.size).ToArray());

        }
    }

    public void Render() // CALLED EVERY FRAME THAT PLAYER IS ACTIVE
    {
        if (render != null)
            render = GameObject.Find("Render").GetComponent<Render>();

        if (Input.GetKeyDown(KeyCode.I))
        {
            altSkin++;
            altSkin = (byte)Mathf.Repeat(altSkin, charData.altSkins.Length);
            spriteRenderer.material = charData.altSkins[altSkin];
        }

        if (render.rollbackFrames < 1 && !render.Paused)
        {
            inputs.GetGameInputs(this);
        }
        hurtbox = FindHurtbox();
        if (!((state == 7 && 6 < frame) || invin > 0))
            CheckForAttacks();
        if (invin > 0)
            invin--;
        else
            invin = 0;

        damage = Mathf.Clamp(damage, 0, 999);
        superArmor = 0;
        shield++;
        shield = Mathf.Clamp(shield, 0, 150);

        ledgeFrame--;
        ledgeFrame = Mathf.Clamp(ledgeFrame, 0, 999);
        if (onGround)
        {
            if (state == 4) // crouch
            {
                frame++;
                if (!inputs.down)
                    state = 0;
                if (inputs.attack)
                {
                    state = 10;
                    frame = 0;
                }
                if (inputs.shield)
                {
                    state = 25;
                    frame = 0;
                }

            }
            else if (state == 5) // shield
            {
                frame++;
                shield -= 3;
                if (!inputs.shield)
                    state = 0;
                if (inputs.attack)
                {
                    state = 22;
                    frame = 0;
                }
                if (inputs.right)
                {
                    state = 17;
                    frame = 0;
                    speed.x = 3.7f;
                    dir = -1;
                    if (invin < 6)
                        invin = 6;
                }
                if (inputs.left)
                {
                    state = 17;
                    frame = 0;
                    speed.x = -3.7f;
                    dir = 1;
                    if (invin < 6)
                        invin = 6;
                }
                if (inputs.down)
                {
                    state = 25;
                    frame = 0;
                }
            }
            else if (state == 6) // shield break
            {
                frame++;
                if (frame > 100 + (int)damage)
                    state = 0;
            }
            else if (state == 7) // hitstun
            {
                frame--;
                if (inputs.right)
                    speed.x += 0.2f;
                if (inputs.left)
                    speed.x -= 0.2f;
                if (frame < 1)
                    state = 0;
            }
            else if (state == 8)
                render.attackData.NeutralGround(pIndex);
            else if (state == 9)
                render.attackData.SideGround(pIndex);
            else if (state == 10)
                render.attackData.DownGround(pIndex);
            else if (state == 17)
            {
                frame++;
                if (frame > charData.rollSpeed) state = 0;
            }
            else if (state == 19)
            {
                frame++;
                if (frame > 3) state = 0;
            }
            else if (state == 21)
            {
                frame++;
                if (frame > 90) state = 0;
            }
            else if (state == 22)
                render.attackData.Special(pIndex);
            else if (state == 25)
            {
                frame++;
                if(frame > charData.tauntLength)
                {
                    if (inputs.down && inputs.shield) frame = 0;
                    else state = 0;
                }
                if (frame > 10 && (inputs.right || inputs.left || inputs.up || inputs.down || inputs.attack || inputs.shield))
                {
                    if (!(inputs.down && inputs.shield)) state = 0;
                }
            }
            else
            {
                GroundMovement();
            }

            if (state == 17 && frame < 8)
                speed.x *= 0.75f;
            else
                speed.x *= charData.slipperiness;
            doubleJump = 1;
            if (shield < 1)
            {
                frame = 0;
                state = 6;
                shield = 150;
                Burst emitAmount = hitEffect.emission.GetBurst(0);
                emitAmount.count = 10;
                hitEffect.emission.SetBurst(0, emitAmount);
                hitEffect.Play();
            }
        }
        else
        {
            if (state == 11)
                render.attackData.NeutralAir(pIndex);
            else if (state == 12)
                render.attackData.SideAir(pIndex);
            else if (state == 13)
                render.attackData.DownAir(pIndex);
            else if (state == 14)
                render.attackData.Recovery(pIndex);
            else if (state == 16) // Airdodge
            {
                frame++;
                if (frame > 20)
                    state = 2;
            }
            else if (state == 18)
            {
                speed.y = 0 - charData.fallSpeed;
                frame++;
                if (frame > 11)
                {
                    if ((inputs.left && dir == -1) || (inputs.right && dir == 1) || inputs.up || inputs.attack || inputs.shield)
                    {
                        state = 19;
                        invin = 10;
                        frame = 0;
                    }
                    if ((inputs.left && dir == 1) || (inputs.right && dir == -1) || inputs.down || frame > 100)
                    {
                        state = 20;
                        frame = 1;
                        ledgeFrame = 30;
                    }
                }
            }
            else if (state == 21)
                frame++;
            else if (state == 22)
                render.attackData.Special(pIndex);
            else if (state == 7) // hitstun
            {
                Burst emitAmount = hitEffect.emission.GetBurst(0);
                emitAmount.count = Mathf.CeilToInt(Mathf.Sqrt(Mathf.Pow(speed.x, 2) + Mathf.Pow(speed.y, 2))/0.75f);
                hitEffect.emission.SetBurst(0, emitAmount);
                hitEffect.Play();
                frame--;
                if (frame < 1)
                    state = 0;
                if (inputs.right)
                    speed.x += 0.1f;
                if (inputs.left)
                    speed.x -= 0.1f;
                if (inputs.up)
                    speed.y += 0.1f;
                if (inputs.down)
                    speed.y -= 0.1f;
                if (charData.charName == "Medium Mike")
                {
                    // Play star effect here
                    charge += 2;
                    if (charge == 202) charge = 200;
                }
            }
            else if (state == 23) // spawn platform
            {
                frame++;
                if (frame < 32)
                    speed.y = (-1.6f + (frame / 20f)) + (-charData.fallSpeed);
                else
                {
                    speed.y = -charData.fallSpeed;
                    if (frame > 100 || inputs.right || inputs.left || inputs.up || inputs.down || inputs.attack || inputs.shield)
                    {
                        frame = 0;
                        state = 2;
                    }
                }
            }
            else if (state == 24) // dead
            {
                frame++;
                speed.y = -charData.fallSpeed;
            } 
            else
            {

                if (state == 3 || state == 15) // 15 = freefall
                {
                    frame++;
                }
                else
                {
                    if (state == 20)
                    {
                        frame++;
                        if (frame > 30)
                            state = 2;
                    }
                    else
                    {
                        if (frame > 0)
                            frame++;
                        state = 2;
                        if (!inputs.up)
                            frame = 0;
                        if (inputs.up && frame > 0 && frame < 6)
                            speed.y = ((frame * 0.1f) + 0.3f) * charData.jumpHeight;

                    }
                }
                if (inputs.up && ((doubleJump > 0 && frame == 0) || (state == 20 && frame > 0)))
                {
                    doubleJump--;
                    speed.y = charData.jumpHeight;
                    state = 3;
                    frame = 0;
                    if (inputs.right)
                        speed.x = 0.5f;
                    else if (inputs.left)
                        speed.x = -0.5f;
                }

                if (inputs.right)
                    speed.x += 0.1f;
                else if (inputs.left)
                    speed.x -= 0.1f;

                if (inputs.down)
                    speed.y -= 0.3f;

                if (state != 15)
                {
                    if (inputs.shield)
                    {
                        if (!inputs.attack)
                        {
                            frame = 0;
                            state = 16;
                            invin = 10;
                            if (inputs.right)
                                speed = new Vector2(1.2f, 0.3f);
                            if (inputs.left)
                                speed = new Vector2(-1.2f, 0.3f);
                            if (inputs.up)
                                speed.y = 1f;
                        }
                    }
                    if (inputs.attack)
                    {
                        if (inputs.right)
                            dir = 1;
                        if (inputs.left)
                            dir = -1;

                        frame = 0;
                        if (inputs.up)
                            state = 14;
                        else if (inputs.down)
                            state = 13;
                        else if (inputs.left || inputs.right)
                            state = 12;
                        else
                            state = 11;
                        if (inputs.shield)
                        {
                            state = 22;
                        }
                    }
                }
            }


            speed.x *= 0.95f;
            if (!(state == 7 || (state == 16 && frame < 6)))
                speed.x *= charData.airSpeed;
        }

        //Falling
        if (state == 7 || (state == 16 && frame < 6))
            speed.y -= 0.1f;
        else
            speed.y += charData.fallSpeed;

        if (speed.y < charData.maxFallSpeed && state != 7)
            speed.y = charData.maxFallSpeed;

        if (!(state == 7 && frame > 9))
            pos += speed;

        CheckCollision();

        if (pos.x < -render.sideBlastZoneSize || pos.x > render.sideBlastZoneSize || pos.y < render.bottomBlastZoneSize || pos.y > render.topBlastZoneSize)
        {
            render.CreateBlastZoneEffect(pos);
            Respawn();
        }

        pos = new Vector2(Mathf.Round(pos.x * 10f), Mathf.Round(pos.y * 10f)) / 10f;
        transform.position = pos;

        hurtbox = FindHurtbox();
        UpdateSprite();
        UpdateTrailsAndEffects();
    }

    private void CheckCollision()
    {
        onGround = false;

        int mult = GameUtils.collisionMult;
        Vector2Int collisionSize = Collision.RoundVector2ToInt(charData.collisionSize, 3);
        for (int i = 0; i<render.groundBoxes.Length; i++) // Loop through all ground boxes in scene and check if they are contacted with the player
        {
            GroundBox groundBox = render.groundBoxes[i];

            Vector2Int roundPos = Collision.RoundVector2ToInt(pos, mult);
            Vector2Int roundSpeed = Collision.RoundVector2ToInt(speed, mult);
            Vector2Int groundPos = Collision.RoundVector2ToInt(groundBox.pos, mult);
            Vector2Int groundSize = Collision.RoundVector2ToInt(groundBox.size, mult);

            if (Collision.IsBoxesCollided(roundPos, collisionSize, groundPos, groundSize))
            {
                if (Collision.CollidedFromTop(roundPos, collisionSize, groundPos, groundSize, roundSpeed))
                {
                    if (!(groundBox.size.y == 0 && (state == 4 && frame > 4)))
                    {
                        if (state != 14)
                            onGround = true;
                        pos.y = (groundPos.y + ((collisionSize.y + groundSize.y) / 2)) / Mathf.Pow(10, mult);
                        if (state == 7)
                            speed.y = speed.y * -0.9f;
                        else
                            speed.y = 0f;
                        if (state > 4 && !(state == 7 || (state > 10 && state < 16))) // STOP AT EDGE
                        {
                            if (roundPos.x > groundPos.x + ((0 - collisionSize.x + groundSize.x) / 2))
                            {
                                pos.x = (groundPos.x + (0 - collisionSize.x + groundSize.x) / 2) / Mathf.Pow(10, mult);
                                speed.x = 0f;
                            }
                            if (roundPos.x < groundPos.x - ((0 - collisionSize.x + groundSize.x) / 2))
                            {
                                pos.x = (groundPos.x - (0 - collisionSize.x + groundSize.x) / 2) / Mathf.Pow(10, mult);
                                speed.x = 0f;
                            }
                        }
                    }
                }

                if (!(groundBox.size.y == 0)) // Ignore if ground box is platform
                {
                    if (!onGround)
                    {
                        if (Collision.CollidedFromBottom(roundPos, collisionSize, groundPos, groundSize, roundSpeed))
                        {
                            pos.y = (groundPos.y + ((collisionSize.y + groundSize.y) / -2)) / Mathf.Pow(10, mult);
                            speed.y = 0f;
                        }
                    }
                    if (Collision.CollidedFromRight(roundPos, collisionSize, groundPos, groundSize, roundSpeed))
                    {
                        pos.x = (groundPos.x + ((collisionSize.x + groundSize.x) / 2)) / Mathf.Pow(10, mult);
                        if (state == 7)
                            speed.x = speed.x * -0.9f;
                        else
                            speed.x = 0f;
                    } 
                    else if(Collision.CollidedFromLeft(roundPos, collisionSize, groundPos, groundSize, roundSpeed))
                    {
                        pos.x = (groundPos.x + ((collisionSize.x + groundSize.x) / -2)) / Mathf.Pow(10, mult);
                        if (state == 7)
                            speed.x = speed.x * -0.9f;
                        else
                            speed.x = 0f;
                    }

                }
            }
            // Ledgegrab check
            if (groundSize.y != 0 && i == 0)
            {
                if ((Mathf.Abs(roundPos.x - groundPos.x) < ((collisionSize.x * 2) + groundSize.x) / 2) && (Mathf.Abs(roundPos.x - groundPos.x) > ((groundSize.x / 2) - collisionSize.x)) && (Mathf.Abs(roundPos.y + (collisionSize.y / 2) - (groundPos.y + (groundSize.y / 2))) < (collisionSize.y/2)))
                {
                    if (!(state == 18 || state == 19 || state == 20 || state == 7 || state == 11 || state == 13 || state == 22) && !onGround && ledgeFrame == 0) // Is there a better way to do this?
                    {
                        invin = 30;
                        speed = new Vector2(0, 0);
                        if (roundPos.x > groundPos.x)
                            dir = -1;
                        else
                            dir = 1;
                        frame = 0;
                        state = 18;
                        pos = new Vector2(groundPos.x + ((collisionSize.x + groundSize.x) / (dir * -2)), groundPos.y + ((groundSize.y - collisionSize.y) / 2)) / 1000f;
                        pos.y += 0.8f;
                        doubleJump = 1;
                    }
                    if (state == 19)
                    {
                        onGround = true;
                        pos = new Vector2(groundPos.x + ((0-collisionSize.x + groundSize.x) / (dir * -2)), groundPos.y + ((collisionSize.y + groundSize.y) / 2)) / 1000f;
                    }
                }
            }
        }
    }

    public void CheckForAttacks()
    {
        for (int i = 0; i < render.hitboxes.Count; i++)
        {
            Hitbox hitbox = render.hitboxes[i];
            if (!hitbox.fresh && !(hitbox.sender == pIndex))
            {
                if (Collision.IsBoxesCollided(Collision.RoundVector2ToInt(pos + hurtbox.offset, GameUtils.collisionMult), Collision.RoundVector2ToInt(hurtbox.size, GameUtils.collisionMult), Collision.RoundVector2ToInt(hitbox.pos, GameUtils.collisionMult), Collision.RoundVector2ToInt(hitbox.size, GameUtils.collisionMult))) // If touching a hitbox
                {
                    if (state == 5)
                    {
                        shield -= Mathf.RoundToInt(hitbox.damage);
                        frame = 0;
                    }
                    else
                    {
                        damage += hitbox.damage;
                        if (superArmor < 1)
                        {
                            state = 7;
                            frame = Mathf.FloorToInt(8f + (hitbox.damage / 3f));
                            dir = render.players[hitbox.sender].dir * -1;

                            speed.y = (0.1f + (damage * 0.012f * Mathf.Sqrt(hitbox.damage))) * hitbox.multiplier.y / charData.weight;
                            speed.x = (dir * -1) * ((0.5f + (damage * 0.015f * Mathf.Sqrt(hitbox.damage))) * hitbox.multiplier.x) / charData.weight;

                            if (hitbox.multiplier.x < 0)
                                dir = render.players[hitbox.sender].dir * 1;

                        }
                        if (hitbox.type == 2)
                        {
                            state = 6;
                            frame = 100;
                            speed = new Vector2(0, 0);
                        }
                    }
                }
            }
        }
    }

    public void GroundMovement()
    {
        frame++;
        if (inputs.right)
        {
            if (state == 1)
            {
                if (frame > 1)
                {
                    speed.x += charData.runSpeed;
                    dir = 1;
                }
            }
            else
            {
                frame = 0;
                state = 1;
                dir = 1;
            }
        }
        else if (inputs.left)
        {
            if (state == 1)
            {
                if (frame > 1)
                {
                    speed.x -= charData.runSpeed;
                    dir = -1;
                }
            }
            else
            {
                frame = 0;
                state = 1;
                dir = -1;
            }
        }
        else
        {
            state = 0;
        }

        if (inputs.up)
        {
            speed.y = 0.6f * charData.jumpHeight;
            if (inputs.left || inputs.right)
            {
                if (Mathf.Abs(1.2f * dir * charData.runSpeed) > Mathf.Abs(speed.x))
                    speed.x = 1.2f * dir * charData.runSpeed;
            }
            frame = 1;
            if (inputs.attack)
            {
                speed.y = 0.5f * charData.jumpHeight;
                state = 14;
                frame = 0;
            }
        }
        else
        {
            if (inputs.down)
            {
                state = 4;
                frame = 0;
            }
            if (inputs.shield)
            {
                state = 5;
                frame = 1;
            }
            if (inputs.attack)
            {
                if (inputs.left || inputs.right)
                {
                    state = 9;
                    frame = 0;
                }
                else
                {
                    state = 8;
                    frame = 1;
                }
                if (inputs.down)
                {
                    state = 10;
                    frame = 0;
                }
                if (inputs.shield)
                {
                    state = 22;
                    frame = 0;
                }
            }
        }
    }

    private void Respawn()
    {
        damage = 0;
        if (stocks != -1)
            stocks--;
        doubleJump = 1;
        dir = 1;
        frame = 0;
        state = 2;
        invin = 100;
        charge = 0;
        superArmor = 0;
        onGround = false;
        speed = new Vector2(0f, -10f);
        pos = new Vector2(0f, 38.4f);
        if (render.playerStates.Contains(23))
            pos.x = 9.6f;
        if (stocks == 0) 
            state = 24;
        else
        {
            state = 23;
            render.CreateProjectile(0, 0, "spawn", pIndex);
        }
    }

    private PlayerHurtbox FindHurtbox()
    {
        for (int i = 0; i < charData.hurtboxes.Length; i++)
        {
            if (charData.hurtboxes[i].state == state)
                return charData.hurtboxes[i].FixHurtboxDirection(dir);
        }
        return charData.hurtboxes[0].FixHurtboxDirection(dir);
    }

    public void UpdateSprite()
    {
        if (state == 24)
        {
            spriteRenderer.color = Color.clear;
            return;
        }
        spriteRenderer.material.SetFloat(GameUtils.brightnessName, 0);
        spriteRenderer.color = Color.white;
        if (state == 15 && frame % 2 == 0)
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);

        if ((state == 16 || state == 17) && invin > 0)
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        if (invin > 0 && render.gameFrame % 2 == 0)
            spriteRenderer.material.SetFloat(GameUtils.brightnessName, 20);
        if (charData.charName == "Medium Mike")
        {
            if (frame < 80 && state == 10)
            {
                int mult = frame % 2 * (frame + 10);
                spriteRenderer.material.SetFloat(GameUtils.brightnessName, mult);
            }
        }
        spriteRenderer.flipX = dir == -1;
        SetSprite(DisplayPlayer.SetSprite(charData.name, state, frame, charge));

    }

    public void SetSprite(int index) // Set sprite to selected index of the list
    {
        if (charData.sprites.Length - 1 < index || index < 0) // If index is out of range
            return;
        spriteRenderer.sprite = charData.sprites[index];
    }

    public void LoadStateData(PlayerSaveData saveData)
    {
        charIndex = saveData.charIndex;
        pIndex = saveData.pIndex;
        pos = saveData.pos;
        speed = saveData.speed;
        onGround = saveData.onGround;
        doubleJump = saveData.doubleJump;

        state = saveData.state;
        frame = saveData.frame;
        dir = saveData.dir;
        stocks = saveData.stocks;
        shield = saveData.shield;
        invin = saveData.invin;
        ledgeFrame = saveData.ledgeFrame;
        damage = saveData.damage;
        charge = saveData.charge;
        superArmor = saveData.superArmor;
        inputs.SetToPlayerInput(saveData.playerInput);
    }

    public void UpdateTrailsAndEffects()
    {
        if (charData.charName == "Road Combatant")
        {
            myTrails[0].gameObject.transform.localPosition = new Vector3(0.75f * dir, 3.7f);
            if (state == 14 && frame < 9)
                myTrails[0].gameObject.SetActive(true);
            else
                myTrails[0].gameObject.SetActive(false);
            if (state == 22) myEffects[0].Play();
        }
    }
}