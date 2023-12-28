using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileSpriteGroup{
    public List<Sprite> sprites;
    public Material spriteMaterial;
}

public class ProjectileRenderer : MonoBehaviour
{
    public Projectile myProjectile;
    public Gradient fade;
    private SpriteRenderer spriteRenderer;
    public List<ProjectileSpriteGroup> spriteGroups = new();
    List<Sprite> mySprites = new();

    void Start()
    {
        if (myProjectile == null)
            Destroy(gameObject);
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Display();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Display();
    }

    private void Display()
    {
        if (myProjectile.type == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        transform.position = myProjectile.pos;
        spriteRenderer.flipX = myProjectile.dir == -1;
        spriteRenderer.material = spriteGroups[myProjectile.type - 1].spriteMaterial;
        spriteRenderer.sortingLayerName = "Default";

        int frame = myProjectile.frame;

        if (myProjectile.type <= spriteGroups.Count)
            mySprites = spriteGroups[myProjectile.type - 1].sprites;
        else
            Debug.LogErrorFormat("missing sprite group");

        if (myProjectile.type == 1)
        {
            spriteRenderer.color = Color.white;
            spriteRenderer.material.SetFloat("_Glow", (Mathf.FloorToInt(frame) / 3 % 2 * 0.5f)+ 1);
            if (frame < 4)
                spriteRenderer.sprite = mySprites[0];
            else
                spriteRenderer.sprite = mySprites[(Mathf.FloorToInt(frame) / 2 % 4) + 1];

            if (frame > 34)
            {
                spriteRenderer.color = fade.Evaluate((frame - 35f) / 5f);
                spriteRenderer.material.SetFloat("_Glow", 0.25f);
            }
        } 
        else if (myProjectile.type == 2)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.48f);
            spriteRenderer.sortingLayerName = "Default Effects";
            spriteRenderer.color = Color.white;
            spriteRenderer.sprite = mySprites[0];
        }
    }
}
