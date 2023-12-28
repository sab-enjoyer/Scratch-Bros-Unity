using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParallaxData
{
    public bool visible;
    public Sprite sprite;
    public Material material;
    public Vector3 offset;
    public Vector3 offsetOnStageSelect;
}

public class Parallax : MonoBehaviour
{
    public int layer;
    public Camera mainCamera;
    [SerializeField]
    private Render render;
    public ParallaxData myData;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        myData = GameManager.instance.maps[render.mapId].parallax1;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = layer;

        if (layer == 1)
            myData = GameManager.instance.maps[render.mapId].parallax1;
        else if (layer == 2)
            myData = GameManager.instance.maps[render.mapId].parallax2;
        else if (layer == 3)
            myData = GameManager.instance.maps[render.mapId].parallax3;
        else if (layer == 0)
            myData = GameManager.instance.maps[render.mapId].foreground;

        spriteRenderer.sprite = myData.sprite;
        spriteRenderer.material = myData.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (!myData.visible)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            spriteRenderer.enabled = !render.showBoxes;
            if (layer == 3)
                transform.position = (mainCamera.transform.position + myData.offset) * 0.0625f;
            else if (layer == 2)
                transform.position = (mainCamera.transform.position + myData.offset) * 0.125f;
            else if (layer == 1)
                transform.position = (mainCamera.transform.position + myData.offset) * 0.25f;
            else
                transform.position = myData.offset;
        }
    }
}
