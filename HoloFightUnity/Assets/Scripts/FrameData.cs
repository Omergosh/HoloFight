using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitboxData
{
    // Contains data about a single hitbox: position (assuming player is facing right), size, damage, hitstun, etc.
    public Rect hitboxRect;
    public int damage;
    public int hitstunInflicted;
    public int shieldDamage;
    public bool clashes;
    public bool airUnblockable;
    public bool groundUnblockable;
}

public struct FrameData
{
    // Contains data about a single frame:
    // all hitboxes active on this frame, changes to player's velocity/position (assuming player is facing right),
    // invincibility/armor, etc.
    // (also info about possible cancels/transitions into other animations/states)
    public HitboxData[] hitboxes;
    public Vector2 changeToPlayerPosition;
    public Vector2 changeToPlayerVelocity;
    public bool canCancelIntoJump;
    public bool canCancelIntoAttackA;
    public bool canCancelIntoAttackB;
    public bool canCancelIntoAttackC;
    public bool canCancelIntoDefend;
}

public struct AnimationStateData
{
    // Contains frames and info about exiting/transitions to other states/animations
    public string animationName;
    public string animationToExitIntoAfterLastFrame; // If set to current animation name, that means this animation loops.
    public FrameData[] frames;
    public int currentFrame;
}