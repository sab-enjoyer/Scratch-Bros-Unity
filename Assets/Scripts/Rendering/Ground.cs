using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private Render render;
    private SpriteRenderer mySpriteRenderer;
    private List<GameObject> groundBoxObjects = new();
    public Gradient ultimatumGlow;
    public Material defaultMaterial;
    public MapData mapData;
    private int timer = 0;
    public float glowDelay;

    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        render = GameObject.Find("Render").GetComponent<Render>();

        mapData = GameManager.instance.maps[render.mapId];
        mySpriteRenderer.material = mapData.material;
        mySpriteRenderer.sprite = mapData.sprite;
        transform.position = mapData.offset;

        for (int i = 0; i < mapData.groundBoxes.Length; i++)
        {
            CreateGroundBoxObject(i);
        }
    }

    private void CreateGroundBoxObject(int index)
    {
        GameObject groundBoxObject = new GameObject("Ground Box " + (groundBoxObjects.Count + 1));
        groundBoxObjects.Add(groundBoxObject);
        groundBoxObject.transform.SetParent(transform);

        LineRenderer lineRenderer = groundBoxObject.AddComponent<LineRenderer>();

        lineRenderer.loop = true;
        lineRenderer.material = render.lineMaterial;
        lineRenderer.startWidth = render.lineThickness;
        lineRenderer.endWidth = render.lineThickness;
        lineRenderer.colorGradient = render.groundBoxGradient;
        lineRenderer.positionCount = 4;
        lineRenderer.sortingLayerName = "Boxes";

        lineRenderer.SetPositions(GameUtils.DrawBox(render.groundBoxes[index].pos, render.groundBoxes[index].size).ToArray());
    }

    public void DrawGroundBoxes()
    {
        if (groundBoxObjects.Count < render.groundBoxes.Length)
        {
            for (int i = 0; i < render.groundBoxes.Length - groundBoxObjects.Count; i++)
            {
                CreateGroundBoxObject(i);
            }
        }
        if (groundBoxObjects.Count > render.groundBoxes.Length)
        {
            for (int i = 0; i < groundBoxObjects.Count - render.groundBoxes.Length; i++)
            {
                Destroy(groundBoxObjects[groundBoxObjects.Count - 1]);
                groundBoxObjects.RemoveAt(groundBoxObjects.Count - 1);
            }
        }

        for (int i = 0; i < render.groundBoxes.Length; i++)
        {
            LineRenderer lineRenderer = groundBoxObjects[i].GetComponent<LineRenderer>();

            if (!render.showBoxes)
                lineRenderer.enabled = false;
            else
            {
                lineRenderer.enabled = true;
                lineRenderer.startWidth = render.lineThickness;
                lineRenderer.endWidth = render.lineThickness;
                lineRenderer.colorGradient = render.groundBoxGradient;

                lineRenderer.SetPositions(GameUtils.DrawBox(render.groundBoxes[i].pos, render.groundBoxes[i].size).ToArray());

            }

        }
    }

    private void Update()
    {
        timer++;

        if (mapData.name == "Ultimatum")
            mySpriteRenderer.material.SetColor("_Color", ultimatumGlow.Evaluate(Mathf.Repeat(timer/glowDelay, 1f)));

        transform.position = mapData.offset;

        mySpriteRenderer.enabled = !render.showBoxes;
    }
}
