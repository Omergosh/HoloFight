using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BattleEventResponse// : MonoBehaviour
{
    [SerializeField]
    //public Type eventToRespondTo;
    public string eventType;
    public UnityEvent action;

    //public BattleEventResponse(Type newEventType, UnityEvent newAction)
    //{
    //    eventToRespondTo = newEventType;
    //    action = newAction;
    //}

    //public BattleEventResponse(Type newEventType)
    //{
    //    eventToRespondTo = newEventType;
    //}
}
