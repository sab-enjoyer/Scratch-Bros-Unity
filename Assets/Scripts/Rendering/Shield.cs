using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer highlightSprite, baseSprite;

    public Player player;

    [SerializeField]
    private Sprite[] shieldBases, shieldHighlights;

    private void Start()
    {
        if (player != null)
        {
            transform.SetParent(GameObject.Find("Player " + player.pIndex).transform);
            transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        else
            Destroy(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if (player.state == 5)
        {
            int shieldSize = 18 - Mathf.CeilToInt(player.shield / (1.5f * player.charData.shieldSize));
            if (shieldSize < shieldHighlights.Length)
            {
                highlightSprite.sprite = shieldHighlights[shieldSize];
                baseSprite.sprite = shieldBases[shieldSize];

                Color baseColor = GameManager.instance.playerColors[player.pIndex + 1];
                if (player.frame == 1)
                    baseColor = new Color(baseColor.r + 0.4f, baseColor.g + 0.4f, baseColor.b + 0.4f, 0.9f);
                else
                    baseColor.a = 0.5f;

                highlightSprite.color = GameManager.instance.playerColors[player.pIndex + 1];
                highlightSprite.color = new Color(highlightSprite.color.a + 55, highlightSprite.color.g + 55, highlightSprite.color.b + 55);
                baseSprite.color = baseColor;
            }

        }
        else
        {
            baseSprite.sprite = null;
            highlightSprite.sprite = null;
        }
    }
}
