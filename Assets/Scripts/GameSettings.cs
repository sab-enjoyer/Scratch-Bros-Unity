using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public int stocks;
    public int time;
    public int cpuLevel;
    public int cpuChar;

    public GameSettings()
    {
        stocks = 3;
        time = 0;
        cpuLevel = 3;
        cpuChar = -1;
    }
}