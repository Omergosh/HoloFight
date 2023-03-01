using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleEventInteraction
{
    
}

public struct BattleEventAttackHit : IBattleEventInteraction
{
    //RectInt
    public Rect hitboxRect;
    public Rect hurtboxRect;

    public Vector2 GetContactPoint()
    {
        return Vector2.Lerp(hitboxRect.center, hurtboxRect.center, 0.5f);
        //Rect intersection = hitboxRect;
        //intersection = intersection.Intersect(hurtboxRect);
    }

    public Rect GetIntersectRect()
    {
        Rect intersection = new Rect();
        intersection.xMin = Mathf.Max(hitboxRect.xMin, hurtboxRect.xMin);
        intersection.yMin = Mathf.Max(hitboxRect.yMin, hurtboxRect.yMin);
        intersection.xMax = Mathf.Min(hitboxRect.xMax, hurtboxRect.xMax);
        intersection.yMax = Mathf.Min(hitboxRect.yMax, hurtboxRect.yMax);
        return intersection;
    }
}
public struct BattleEventAnimationEffect : IBattleEventInteraction {}
public struct BattleEventJump : IBattleEventInteraction {}
public struct BattleEventWallJump : IBattleEventInteraction {}
public struct BattleEventTouchWall : IBattleEventInteraction {}
public struct BattleEventCharacterDeath : IBattleEventInteraction {}
