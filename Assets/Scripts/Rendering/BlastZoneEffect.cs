using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastZoneEffect : MonoBehaviour
{
    public Vector3 deathPos;
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int timer;

    void Start()
    {
        if (deathPos == null)
        {
            Destroy(gameObject);
            return;
        }
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        Vector3 dir = new Vector3(0f, 0f, 90f) + (Vector3.zero - deathPos);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        transform.Rotate(new Vector3(0f, 0f, 90f));
        timer = 2;
    }

    private void Update()
    {
        spriteRenderer.sprite = sprites[Mathf.FloorToInt(timer / 2)];

        timer++;
        if (Mathf.FloorToInt(timer / 2) > sprites.Length-1)
            Destroy(gameObject);
    }
}
