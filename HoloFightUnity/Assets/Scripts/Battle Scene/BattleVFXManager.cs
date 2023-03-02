using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleVFXManager : MonoBehaviour
{
    public BattleVFXAnimEffect[] hitEffectPool = new BattleVFXAnimEffect[8];
    public int currentHitEffectIndex = 0;

    public static BattleVFXManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        //else
        //{
        //    Destroy(this);
        //}
    }

    public int IncrementHitEffectIndex()
    {
        currentHitEffectIndex++;
        if (currentHitEffectIndex >= hitEffectPool.Length) { currentHitEffectIndex = 0; }
        return currentHitEffectIndex;
    }

    public static void PlayHitEffectAnim(Vector2 centerPoint)
    {
        instance.hitEffectPool[instance.currentHitEffectIndex].gameObject.SetActive(true);
        instance.hitEffectPool[instance.currentHitEffectIndex].transform.position =
            new Vector3(
                (centerPoint.x - (HfGame.bounds.width / 2)) / BattleManagerScript.pixelsInWorldUnit,
                ((centerPoint.y - (HfGame.bounds.height / 2)) / BattleManagerScript.pixelsInWorldUnit) - 1.4f,
                instance.hitEffectPool[instance.currentHitEffectIndex].transform.position.z / BattleManagerScript.pixelsInWorldUnit
                );
        // TODO: Investigate why the 'y' transform position has to have 1.4 Unity units (140 gamestate pixels) subtracted from it.
        //      I suspect it has something to do with the player's hurtbox height being 140 pixels.
        //      Perhaps because the hitbox/hurtbox positions being retrieved are relative to their parent player object positions?
        //      Either way, it's unintuitive and the discrepancy should probably be resolved elsewhere, instead of worked around here.

        instance.hitEffectPool[instance.currentHitEffectIndex].animation.Play();
        instance.IncrementHitEffectIndex();
    }
}
