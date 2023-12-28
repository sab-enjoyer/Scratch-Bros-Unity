using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public static class Collision
{
    public static bool IsBoxesCollided(Vector2Int pos1, Vector2Int size1, Vector2Int pos2, Vector2Int size2)
    {
        if (Mathf.Abs(pos1.x - pos2.x) < (size1.x + size2.x) / 2 && Mathf.Abs(pos1.y - pos2.y) < (size1.y + size2.y) / 2)
            return true;
        else
            return false;
            
    }

    public static bool CollidedFromTop(Vector2Int pos1, Vector2Int size1, Vector2Int pos2, Vector2Int size2, Vector2Int speed)
    {
        if (!((pos2.y + ((size1.y + size2.y) / 2)) > (pos1.y - speed.y)))
        {
            return true;
        }
        else
            return false;
    }

    public static bool CollidedFromBottom(Vector2Int pos1, Vector2Int size1, Vector2Int pos2, Vector2Int size2, Vector2Int speed)
    {
        if (!((pos2.y + ((size1.y + size2.y) / -2)) < (pos1.y - speed.y)))
        {
            return true;
        }
        else
            return false;
    }

    public static bool CollidedFromRight(Vector2Int pos1, Vector2Int size1, Vector2Int pos2, Vector2Int size2, Vector2Int speed)
    {
        if (!((pos2.x + ((size1.x + size2.x) / 2)) > (pos1.x - speed.x)))
        {
            return true;
        }
        else
            return false;
    }

    public static bool CollidedFromLeft(Vector2Int pos1, Vector2Int size1, Vector2Int pos2, Vector2Int size2, Vector2Int speed)
    {
        if (!((pos2.x + ((size1.x + size2.x) / -2)) < (pos1.x - speed.x)))
        {
            return true;
        }
        else
            return false;
    }

    public static Vector2Int RoundVector2ToInt(Vector2 vector2, int decimalPlaces = 2)
    {
        float multiplier = 1;
        if (decimalPlaces == 3)
            multiplier = 1000f;
        else
        {
            for (int i = 0; i < decimalPlaces; i++)
            {
                multiplier *= 10f;
            }
        }
        return Vector2Int.RoundToInt(vector2 * multiplier);
    }

    public static float RoundFloat(float f, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return Mathf.Round(f * multiplier) / multiplier;
    }
}
