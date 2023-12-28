using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDisplayer : MonoBehaviour
{
    public int playerIndex;
    private Player player;
    private RectTransform rectTransform;

    [SerializeField]
    private Image characterImage, stocksImage, bgImage;

    [SerializeField]
    private DamageCounter damageCounter;

    [SerializeField]
    private Sprite[] stockSprites;

    private Render render;
    // Start is called before the first frame update
    void Start()
    {
        render = GameObject.Find("Render").GetComponent<Render>();
        player = render.players[playerIndex];
        UpdateDisplay();

        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(200f + (400f * playerIndex), rectTransform.anchoredPosition.y);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (render.playerStates.Contains(24))
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        damageCounter.damage = player.damage;
        damageCounter.showDamage = player.state == 24;
        if (player.stocks > 0)
        {
            stocksImage.gameObject.SetActive(true);
            stocksImage.sprite = stockSprites[player.stocks - 1];
            stocksImage.color = getPlayerColor();
        }
        else
            stocksImage.gameObject.SetActive(false);

        characterImage.sprite = GameManager.instance.characters[player.charIndex].icon;
        Color color = getPlayerColor();
        bgImage.color = new Color(color.r, color.g, color.b, bgImage.color.a);

    }

    private Color getPlayerColor()
    {
        if (player.playerType == GameManager.PlayerTypes.Bot || player.playerType == GameManager.PlayerTypes.Dummy)
            return GameManager.instance.playerColors[0];
        else
            return GameManager.instance.playerColors[player.pIndex + 1];
    }
}
