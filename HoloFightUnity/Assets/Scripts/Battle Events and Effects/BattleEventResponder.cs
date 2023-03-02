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
                    //Debug.Log("Battle Event found!");
                    Debug.Log("Hitstop: " + (gameState.hitstopFramesRemaining).ToString());
                    
                    if (HfGame.eventTracker.battleEventsByFrame[gameState.frameNumber].Exists(b => b.GetType().Name == response.eventType))
                    {
                        //Debug.Log("Battle Event invoked!");
                        response.action.Invoke();
                        ProcessBattleEventResponse(response.eventType, HfGame.eventTracker.battleEventsByFrame[gameState.frameNumber][0]);
                        // ^ works so long as there's only one event on the tracker for that frame
                    }
                }
            }
        }
    }

    public void ProcessBattleEventResponse(string eventType, IBattleEventInteraction eventInfo)
    {
        switch (eventType)
        {
            case "BattleEventAttackHit":
                //Debug.Log("Hit VFX should play now");
                BattleEventAttackHit hitEventInfo = (BattleEventAttackHit)eventInfo;
                //PlayVFX(hitEventInfo.GetContactPoint());
                PlayVFX(hitEventInfo.GetIntersectRect().center);
                //PlayVFX(hitEventInfo.hitboxRect.center);
                //PlayVFX(hitEventInfo.hurtboxRect.center);
                //PlayVFX(new Vector2());
                break;

            default:
                Debug.Log("BattleEventResponder: Invalid battle event type");
                break;
        }
    }

    public void PlaySound(string soundTag)
    {
        GameManager.instance.audioManager.PlaySFX(soundTag);
    }
    public void PlayVFX(Vector2 hitPosition)
    {
        BattleVFXManager.PlayHitEffectAnim(hitPosition);
    }
}
