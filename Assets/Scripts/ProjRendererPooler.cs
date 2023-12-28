using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// creates a pool of projectiles instead of creating a new one every time for optimization
public class ProjRendererPooler : MonoBehaviour
{
    public GameObject projRenderer;
    private int poolSize;
    private Render render;

    public Queue<GameObject> projPool = new();

    private void Start()
    {
        render = GameObject.Find("Render").GetComponent<Render>();
        poolSize = render.maxProjectiles;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projRenderer, transform);
            obj.SetActive(false);
            projPool.Enqueue(obj);
        }
    }

    public GameObject SpawnFromPool()
    {
        GameObject objectToSpawn = projPool.Dequeue();
        objectToSpawn.SetActive(true);
        projPool.Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
