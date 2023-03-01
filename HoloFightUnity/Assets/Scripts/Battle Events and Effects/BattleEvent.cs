using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BattleEvent
{
    public int frame;
    public string name;

    public static string GenerateName(int frame, IBattleEventInteraction battleEventInteraction)
    {
        string newName = "";
        newName += frame.ToString();
        newName += nameof(battleEventInteraction);
        return newName;
    }
}
