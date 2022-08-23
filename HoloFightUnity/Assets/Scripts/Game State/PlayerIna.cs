using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HFConstants;

public class PlayerIna : Player
{
    public PlayerIna()
    {
        // Assign Ina-specific character data!
        characterName = "Ina";
        characterFullName = "Ninomae Ina'nis";
    }

    public override void ProcessAnimationStateLogic(InputData inputData)
    {
        // If a hitstun/blockstun state, decrement the appropriate value, and exit to another state where applicable

        // Based on current state and inputs, either:
        // - stay on this frame,
        // - advance current animation by one frame
        // - transition into another animation
        
        switch (playerState)
        {
            case PlayerState.IDLE:
                //Debug.Log("idle");
                if (inputData.GetInputDown(INPUT_ATTACK_B))
                {
                    ChangeAnimationState("attackB1");
                }
                else if (inputData.GetInputDown(INPUT_ATTACK_A))
                {
                    ChangeAnimationState("attackA1");
                }
                else if ((inputData.GetInputDown(INPUT_LEFT) && !inputData.GetInputDown(INPUT_RIGHT)) ||
                    (!inputData.GetInputDown(INPUT_LEFT) && inputData.GetInputDown(INPUT_RIGHT)))
                {
                    ChangeAnimationState("run");
                }
                else
                {
                    currentAnimationState.AdvanceFrame();
                }
                break;

            case PlayerState.RUNNING:
                //Debug.Log("runn");
                if (inputData.GetInputDown(INPUT_ATTACK_B))
                {
                    ChangeAnimationState("attackB1");
                }
                else if (inputData.GetInputDown(INPUT_ATTACK_A))
                {
                    ChangeAnimationState("attackA1");
                }
                else if (!inputData.GetInputDown(INPUT_LEFT) && !inputData.GetInputDown(INPUT_RIGHT))
                {
                    ChangeAnimationState("idle");
                }
                else
                {
                    currentAnimationState.AdvanceFrame();
                }
                break;

            case PlayerState.JUMPING:
                if (inputData.GetInputDown(INPUT_ATTACK_A))
                {
                    ChangeAnimationState("airAttackA1");
                }
                else if (velocity.y < 0f)
                {
                    ChangeAnimationState("fall");
                }
                else if (currentAnimationState.currentFrameNumber < 4)
                {
                    currentAnimationState.AdvanceFrame();
                }
                break;

            case PlayerState.FALLING:
                if (inputData.GetInputDown(INPUT_ATTACK_A))
                {
                    ChangeAnimationState("airAttackA1");
                }
                else
                {
                    currentAnimationState.AdvanceFrame();
                }
                break;

            case PlayerState.ATTACKING:
                //Debug.Log("she attacc");
                //Debug.Log("Frame of attack animation before advance (starting at 0): " + currentAnimationState.currentFrameNumber.ToString());
                if (currentAnimationState.currentFrameNumber >= currentAnimationState.frames.Count - 1)
                {
                    ReturnToNeutralState(inputData);
                }
                currentAnimationState.AdvanceFrame();
                //Debug.Log("Frame of attack animation after advance (starting at 0): " + currentAnimationState.currentFrameNumber.ToString());
                break;

            case PlayerState.HITSTUN_AIRBORNE:
            case PlayerState.HITSTUN_GROUNDED:
                //bouncy = true;
                hitstun--;
                if(hitstun <= 0)
                {
                    //bouncy = false;
                    ReturnToNeutralState(inputData);
                }
                break;

            default:
                Debug.Log("unknown/undefined player state");
                break;
        }
    }

    private void ReturnToNeutralState(InputData inputData)
    {
        if (isOnGround)
        {
            if ((inputData.GetInputDown(INPUT_LEFT) && !inputData.GetInputDown(INPUT_RIGHT)) ||
                    (!inputData.GetInputDown(INPUT_LEFT) && inputData.GetInputDown(INPUT_RIGHT)))
            {
                ChangeAnimationState("run");
            }
            else
            {
                ChangeAnimationState("idle");
            }
        }
        else
        {
            ChangeAnimationState("fall");
        }
    }

    protected override void TriggerJump()
    {
        //base.TriggerJump();
        ChangeAnimationState("jump");
    }

    protected override void TriggerLand()
    {
        //base.TriggerLand();
        if (!IsInHitstun)
        {
            ChangeAnimationState("idle");
        }
        //else
        //{
        //    ChangeAnimationState("hitstun");
        //}
    }
}
