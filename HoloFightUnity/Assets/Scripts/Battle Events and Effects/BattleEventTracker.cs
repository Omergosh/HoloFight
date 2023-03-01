using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BattleEventTracker
{
    [SerializeField]
    public Dictionary<int, List<IBattleEventInteraction>> battleEventsByFrame;
    const int timeoutWindow = 600;

    [SerializeField]
    List<IBattleEventInteraction> debugList;

    public BattleEventTracker(int timeout = 600)
    {
        battleEventsByFrame = new Dictionary<int, List<IBattleEventInteraction>>();
        debugList = new List<IBattleEventInteraction>();
    }

    public void AddEvent(int currentFrame, IBattleEventInteraction newEvent)
    {
        // 1. Check if any event data exists for given frame
        // 2. If not, create a new Key Value pair for that frame
        // 3. If so, add a new item to the value List for that frame

        if (!battleEventsByFrame.ContainsKey(currentFrame))
        {
            List<IBattleEventInteraction> newList = new List<IBattleEventInteraction>();
            newList.Add(newEvent);
            battleEventsByFrame.Add(currentFrame, newList);
            debugList = newList;
        }
        else
        {
            battleEventsByFrame[currentFrame].Add(newEvent);
        }

        // Debug
        Debug.Log("Added Battle Event to Tracker!");
        //Debug.Log(battleEventsByFrame.ToString());
        //Debug.Log(battleEventsByFrame);
        //Debug.Log(battleEventsByFrame.Keys);
        //Debug.Log(battleEventsByFrame.Values.ToString());
        Debug.Log(battleEventsByFrame.Count);
        //Debug.Log(battleEventsByFrame[currentFrame]);
        //Debug.Log(battleEventsByFrame[currentFrame].Count);
        //Debug.Log(battleEventsByFrame[currentFrame][0].GetType());
        //Debug.Log(battleEventsByFrame[currentFrame].ToArray());
    }

    public void PurgeTimedOutEvents(int currentFrame)
    {
        foreach(int eventFrame in battleEventsByFrame.Keys)
        {
            if(eventFrame + timeoutWindow <= currentFrame)
            {
                battleEventsByFrame.Remove(eventFrame);
            }
        }
    }
}
