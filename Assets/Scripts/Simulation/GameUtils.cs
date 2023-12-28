using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtils
{
    public const int collisionMult = 3;
    public const string brightnessName = "_brightness";
    public static List<Vector3> DrawBox(Vector2 pos, Vector2 size)
    {
        List<Vector3> vertices = new()
        {
            new Vector3(pos.x + (size.x * -0.5f), pos.y + (size.y * 0.5f)),
            new Vector3(pos.x + (size.x * 0.5f), pos.y + (size.y * 0.5f)),
            new Vector3(pos.x + (size.x * 0.5f), pos.y + (size.y * -0.5f)),
            new Vector3(pos.x + (size.x * -0.5f), pos.y + (size.y * -0.5f))
        };

        return vertices;
    }

    public static int GetWinner(int stocks1, int damage1, int stocks2, int damage2)
    {
        if (stocks1 > stocks2)
            return 0;
        else if (stocks2 > stocks1)
            return 1;
        else if (damage2 > damage1)
            return 0;
        else if (damage1 > damage2)
            return 1;
        else return 0;
    }

    public static Vector2 SpawnPosition(int p, int mapId)
    {
        if (mapId == 0)
            return new Vector2((p % 2 * 19.2f) - 9.6f, 3.2f);
        else
            return new Vector2((p % 2 * 19.2f) - 9.6f, 0);
    }

    public static Dictionary<string, byte> projNames = new()
    {
        {"fireball", 1 },
        {"spawn", 2 },
    };

    public static Dictionary<int, string> stateNames = new()
    {
        {0, "Idle" },
        {1, "Walking" },
        {2, "Airborne" },
        {3, "Double Jump" },
        {4, "Crouch" },
        {5, "Shield" },
        {6, "Shield Break" },
        {7, "Hitstun" },
        {8, "Neutral Ground Attack" },
        {9, "Side Ground Attack" },
        {10, "Down Ground Attack" },
        {11, "Neutral Air Attack" },
        {12, "Side Air Attack" },
        {13, "Down Air Attack" },
        {14, "Up Air Attack/Recovery" },
        {15, "Inactionable Airborne" },
        {16, "Airdodge" },
        {17, "Shield Dodge" },
        {18, "Ledge Hold" },
        {19, "Ledge Getup" },
        {20, "Ledge Drop" },
        {21, "Begin" },
        {22, "Special" },
        {23, "Spawn Platform" },
        {24, "Dead" },
        {25, "Taunt" },

    };
}
