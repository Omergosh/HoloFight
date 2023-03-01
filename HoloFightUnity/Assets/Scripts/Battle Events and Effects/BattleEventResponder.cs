using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleEventResponder : SerializedMonoBehaviour
{
    [SerializeField]
    List<BattleEventResponse> activeResponses = new List<BattleEventResponse>();

    // Start is called before the first frame update
    void Start()
    {
        //activeResponses = new List<BattleEventResponse>();
        //activeResponses.Add(new BattleEventResponse(typeof(BattleEventAttackHit)));
    }

    public void CheckEvents(HfGame gameState)
    {
        if (gameState.hitstopFramesRemaining == HFConstants.HITSTOP_FRAMES_UNIVERSAL)
        {
            foreach (BattleEventResponse response in activeResponses)
            {
                if (HfGame.eventTracker.battleEventsByFrame.ContainsKey(gameState.frameNumber))
                {
                    Debug.Log("Battle Event found!");
                    Debug.Log("Hitstop: " + (gameState.hitstopFramesRemaining).ToString());
                    
                    if (HfGame.eventTracker.battleEventsByFrame[gameState.frameNumber].Exists(b => b.GetType().Name == response.eventType))
                    {
                        Debug.Log("Battle Event invoked!");
                        response.action.Invoke();
                    }
                }
            }
        }
    }

    public void PlaySound(string soundTag)
    {
        GameManager.instance.audioManager.PlaySFX(soundTag);
    }
}
