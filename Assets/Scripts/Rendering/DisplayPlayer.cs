using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DisplayPlayer
{
    public static int SetSprite(string charName, int state, int frame, int charge)
    {
        // Return index of sprite based on frame and state of player
        // formatting changed to reduce redundancy

        if (charName == "Road Combatant")
        {
            if (state == 0 || state == 23 || state == 21)
            {
                if (Mathf.RoundToInt(frame / 8) % 2 == 1) return 4;
                else return 5;
            }
            else if (state == 1)
            {
                if ((Mathf.RoundToInt((frame + 2f) / 3) % 2) == 0) return 50;
                else if ((Mathf.RoundToInt((frame + 2f) / 3) % 4) == 1) return 48;
                else return 49;
            }
            else if (state == 4) return 0;
            else if (state == 2 || state == 15 || state == 20) return 6;
            else if (state == 3)
            {
                if (frame < 3) return 19;
                else if (frame < 6) return 20;
                else if (frame < 9) return 21;
                else return 6;
            }
            else if (state == 5) return 47;
            else if (state == 6)
            {
                if (Mathf.RoundToInt(frame / 15) % 2 == 1) return 2;
                else return 3;
            }
            else if (state == 7) return 7;
            else if (state == 8)
            {
                if (frame < 6) return 8;
                else if (frame < 14) return 9;
                else return 4;
            }
            else if (state == 9)
            {
                if (frame < 14 && 2 < frame) return (Mathf.RoundToInt(frame / 3) % 4) + 10;
                else return 4;
            }
            else if (state == 10)
            {
                if (frame < 2 || 6 < frame) return 23;
                else return 22;
            }
            else if (state == 11)
            {
                if (frame < 8) return 24;
                else return 6;
            }
            else if (state == 12)
            {
                if (frame < 14) return (Mathf.RoundToInt(frame / 2) % 4) + 10;
                else return 6;
            }
            else if (state == 13)
            {
                if (frame < 4) return 28;
                else return 29;
            }
            else if (state == 14)
            {
                if (frame < 8) return 14;
                else return 25;
            }
            else if (state == 16) return 15;
            else if (state == 17) return 16;
            else if (state == 18) return 17;
            else if (state == 19) return 18;
            else if (state == 22)
            {
                if (frame < 10) return 30;
                else if (frame < 20) return 31;
                else if (frame < 40) return 32;
                else if (frame < 50) return 33;
            }
            else if (state == 25)
            {
                if (frame < 2) return 36;
                else if (frame < 4) return 34;
                else if (frame < 6) return 35;
                else if (frame < 7) return 37;
                else if (frame < 9) return 38;
                else if (frame < 10) return 39;
                else if (frame < 12) return 40;
                else if (frame < 13) return 41;
                else if (frame < 15) return 42;
                else if (frame < 20) return 43;
                else if (frame < 23) return 44;
                else if (frame < 24) return 45;
                else return 36;
            }
            else return 4;
        }
        else if(charName == "Medium Mike")
        {
            if (state == 0 || state == 23 || state == 21)
            {
                if (Mathf.RoundToInt(frame / 5) % 2 == 1) return 23;
                else return 24;
            }
            else if (state == 1)
            {
                if (frame > 1)
                {
                    if (Mathf.RoundToInt(frame / 3) % 2 == 1) return 45;
                    else return 46;
                }
                else return 24;
            }
            else if (state == 2 || state == 15 || state == 20) return 43;
            else if (state == 3)
            {
                if (frame < 3) return 29;
                else if (frame < 6) return 30;
                else if (frame < 9) return 31;
                else return 43;
            }
            else if (state == 4) return 1;
            else if (state == 5) return 47;
            else if (state == 6)
            {
                if (Mathf.RoundToInt(frame / 15) % 2 == 1) return 3;
                else return 4;
            }
            else if (state == 7) return 22;
            else if (state == 8) return (Mathf.RoundToInt(frame / 3) % 4) + 25;
            else if (state == 9)
            {
                if (frame < 3) return 12;
                if (frame < 5) return 13;
                if (frame < 7) return 14;
                if (frame < 8) return 15;
                if (frame < 10) return 16;
                if (frame < 11) return 17;
                if (frame < 12) return 18;
                if (frame < 14) return 19;
                else return 20;
            }
            else if (state == 10)
            {
                if (frame < 30) return Mathf.FloorToInt(frame / 10) + 5;
                else if (frame < 80) return 7;
                else if (frame < 90) return Mathf.FloorToInt((frame - 80) / 3) + 8;
                else return 23;
            }
            else if (state == 11) return 44;
            else if (state == 12)
            {
                if (frame < 15) return Mathf.RoundToInt(frame / 3) + 48;
                else return 43;
            }
            else if (state == 13) return 2;
            else if (state == 14) return Mathf.RoundToInt(frame / 2) + 71;
            else if (state == 16) return 0;
            else if (state == 17) return 53;
            else if (state == 18) return 42;
            else if (state == 19) return 21;
            else if (state == 22)
            {
                if (charge < 200) return frame < 10 ? 41 : 23;
                else
                {
                    if (frame < 17) return Mathf.FloorToInt(frame / 2) + 32;
                    else return 40;
                }
            }
            else if (state == 25) return Mathf.FloorToInt(frame / 6) + 54;
        }
        return 4; // DEFAULT
    }
}
