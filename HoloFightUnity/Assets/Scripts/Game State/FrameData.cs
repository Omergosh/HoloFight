using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HitboxData
{
    // Contains data about a single hitbox: position (assuming player is facing right), size, damage, hitstun, etc.
    public Rect hitboxRect;

    public int damage;
    public int hitstunInflicted;
    public Vector2 knockbackInflicted;
    public bool knockbackReplacesInsteadOfAdds;
    public int shieldDamage;

    public bool clashes;
    public bool airUnblockable;
    public bool groundUnblockable;
}

[Serializable]
public struct FrameData
{
    // Contains data about a single frame:
    // all hitboxes active on this frame, changes to player's velocity/position (assuming player is facing right),
    // invincibility/armor, etc.
    // (also info about possible cancels/transitions into other animations/states)
    public List<HitboxData> hitboxes;
    public Vector2 changeToPlayerPosition;
    public Vector2 changeToPlayerVelocity;
    public bool canCancelIntoJump;
    public bool canCancelIntoAttackA;
    public bool canCancelIntoAttackB;
    public bool canCancelIntoAttackC;
    public bool canCancelIntoDefend;
}

[Serializable]
public struct AnimationStateData
{
    // Contains frames and info about exiting/transitions to other states/animations
    public string animationName;
    public string animationToExitIntoAfterLastFrame;
        // ^ If set to current animation name, that means this animation loops.
        // ^ If empty string, animation lingers on last frame until game logic triggers animation state change
    public List<FrameData> frames;
    public int currentFrameNumber;

    public FrameData CurrentFrame => frames[currentFrameNumber];

    public void AdvanceFrame()
    {
        currentFrameNumber++;
        if (currentFrameNumber >= frames.Count)
        {
            currentFrameNumber = 0;
        }
    }

    public void RewindFrame()
    {
        currentFrameNumber--;
        if (currentFrameNumber < 0)
        {
            currentFrameNumber = frames.Count - 1;
        }
    }

    public void GoToFrame(int newFrame)
    {
        if (newFrame < 0 || newFrame >= frames.Count)
        {
            Debug.Log("That frame does not exist. Invalid index / frame number.");
        }
        else
        {
            currentFrameNumber = newFrame;
        }
    }
}