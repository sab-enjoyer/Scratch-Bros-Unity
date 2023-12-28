using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxPooler : MonoBehaviour
{
    public GameObject boxRenderer;
    public int poolSize;

    public Queue<GameObject> hitboxPool = new();

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(boxRenderer, transform);
            obj.SetActive(false);
            hitboxPool.Enqueue(obj);
        }
    }

    public GameObject SpawnFromPool()
    {
        GameObject objectToSpawn = hitboxPool.Dequeue();
        objectToSpawn.SetActive(true);
        hitboxPool.Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
