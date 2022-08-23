using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimationSet", menuName = "ScriptableObjects/CharacterAnimationSetScriptableObject", order = 1)]
public class CharacterAnimationSetScriptableObject : ScriptableObject
{
    public AnimationClip idle;
    public AnimationClip run;
    public AnimationClip jump;
    public AnimationClip fall;

    public AnimationClip hitstun;
    public AnimationClip attackA1;
    public AnimationClip attackB1;
    public AnimationClip attackC1;
    public AnimationClip airAttackA1;
    public AnimationClip airAttackB1;
    public AnimationClip airAttackC1;
}
