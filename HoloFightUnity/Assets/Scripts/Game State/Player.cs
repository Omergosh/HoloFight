using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static HFConstants;

[Serializable]
public class Player
{
    // State variables - likely to change often
    public Vector2 position;
    public Vector2 velocity;
    public bool isOnGround = true;
    public float bounceOffEnergy = 0;
    public int health = 100;
    public int healthMax = 100;
    public int hitstun = 0;
    public int currentAttackId = 0;
    public bool facingRight = true;

    // idk how often this'll change so it goes into its own category
    public Vector2 hurtboxSize = new Vector2(70f, 140f);

    // Variables that change less often
    public bool bouncy = false;

    // Data unlikely to change mid-match
    public int playerId;
    public int teamId; // If one-on-one/FFA, equal to playerId
    public Vector2 collisionBoxSize;
    public string characterName;
    public string characterFullName;
    public float jumpPower = 25f;
    public float maxSpeed = 26.0f;
    public float acceleration = 2.0f;

    // Frame/animation data
    public Dictionary<string, AnimationStateData> animationsAllData = new Dictionary<string, AnimationStateData>();
    public AnimationStateData currentAnimationState;

    public PlayerState playerState = PlayerState.IDLE;

    #region Game loop + logic methods

    // Overload this method to implement character-specific logic!
    public virtual void ProcessAnimationStateLogic(InputData inputData)
    {
        // If a hitstun/blockstun state, decrement the appropriate value, and exit to another state where applicable

        // Based on current state and inputs, either:
        // - stay on this frame,
        // - advance current animation by one frame
        // - transition into another animation

        Debug.Log("animation state logic goes here");
    }

    public void ProcessAllMovement(InputData inputData)
    {
        Vector2 newPosition = new Vector2(this.position.x, this.position.y);

        // If current animation frame prescribes changes to position/velocity, apply them here

        // If current animationstate allows for free horizontal movement, jumping, etc.
        // then handle that here
        newPosition = ProcessMovementBeforeCollisions(inputData);

        // Apply forces such as gravity and friction
        if (newPosition.y <= HfGame.bounds.yMin && this.velocity.y <= 0f)
        {
            this.isOnGround = true;
            if (this.bouncy && this.velocity.y <= -5f)
            {
                // Bounce off ground instead of landing if landing velocity is too high.
                this.velocity.y = Mathf.Abs(this.velocity.y) * 0.6f;
            }
            else
            {
                if(this.velocity.y < 0f)
                {
                    TriggerLand();
                }
                this.velocity.y = 0f;
            }
        }
        else
        {
            // Player is airborne.
            this.isOnGround = false;
            // Apply gravity!
            this.velocity.y -= GRAVITY;
            // If player does not have any upwards momentum (falling or at 0 vertical velocity),
            // allow them to input 'DOWN' to fastfall.
            if (this.velocity.y <= 0 && inputData.GetInputDown(INPUT_DOWN))
            {
                // For now, 'fastfalling' causes gravity to be applied twice as quickly.
                this.velocity.y -= GRAVITY;
            }
        }

        // If player is on ground. (apply friction, check if player wants to jump)
        if (this.isOnGround && this.velocity.y == 0f)
        {
            //if (IsInActionableState)
            //{
            //    if (inputData.GetInputDown(INPUT_LEFT) && !inputData.GetInputDown(INPUT_RIGHT))
            //    {
            //        facingRight = false;
            //    }
            //    else if (!inputData.GetInputDown(INPUT_LEFT) && inputData.GetInputDown(INPUT_RIGHT))
            //    {
            //        facingRight = true;
            //    }
            //}

            // Player is on ground. If player is not moving along the ground
            if (!(inputData.GetInputDown(INPUT_LEFT)
                ^ inputData.GetInputDown(INPUT_RIGHT)))
            {
                // Apply friction!
                if (Mathf.Abs(this.velocity.x) < HFConstants.MINIMUM_NONZERO_FLOAT_THRESHOLD)
                {
                    this.velocity.x = 0;
                }
                else
                {
                    this.velocity.x *= FRICTION_MULTIPLIER;
                }
            }
            // Player can jump!
            if (inputData.GetInputDown(INPUT_UP) && (IsInActionableState || currentAnimationState.CurrentFrame.canCancelIntoJump))
            {
                this.velocity.y = this.jumpPower;
                TriggerJump();
            }
        }
        // Bump into / bounce off of horizontal walls
        if (newPosition.x <= HfGame.bounds.xMin && this.velocity.x <= 0f)
        {
            this.ProcessWallHugOrBounce(
               inputData.GetInputDown(INPUT_LEFT),
               inputData.GetInputDown(INPUT_UP),
                false
                );
        }
        if (newPosition.x >= HfGame.bounds.xMax && this.velocity.x >= 0f)
        {
            this.ProcessWallHugOrBounce(
                inputData.GetInputDown(INPUT_RIGHT),
                inputData.GetInputDown(INPUT_UP),
                true
                );
        }

        // Enforce screen boundaries 
        newPosition.x = Mathf.Clamp(newPosition.x, HfGame.bounds.xMin, HfGame.bounds.xMax);
        newPosition.y = Mathf.Clamp(newPosition.y, HfGame.bounds.yMin, HfGame.bounds.yMax);
        // (could be moved before the gravity/friction changes, which only affect velocity, not position)

        // After processing movement logic, update final position of player
        this.position = newPosition;

        if (IsInActionableState)
        {
            if (inputData.GetInputDown(INPUT_LEFT) && !inputData.GetInputDown(INPUT_RIGHT))
            {
                if ((playerState != PlayerState.JUMPING && playerState != PlayerState.FALLING) || inputData.GetInputJustPressed(INPUT_LEFT))
                {
                    facingRight = false;
                }
            }
            else if (!inputData.GetInputDown(INPUT_LEFT) && inputData.GetInputDown(INPUT_RIGHT))
            {
                if ((playerState != PlayerState.JUMPING && playerState != PlayerState.FALLING) || inputData.GetInputJustPressed(INPUT_RIGHT))
                {
                    facingRight = true;
                }
            }
        }
    }

    public Vector2 ProcessMovementBeforeCollisions(InputData inputData)
    {
        // If current animation frame prescribes changes to position/velocity, apply them here

        // If current animationstate allows for free horizontal movement, jumping, etc.
        // then handle that here// Parse inputs
        if (!IsInHitstun)
        {
            this.ProcessHorizontalMovement(
                inputData.GetInputDown(INPUT_LEFT),
                inputData.GetInputDown(INPUT_RIGHT)
                );
        }

        Vector2 newPosition = new Vector2(this.position.x, this.position.y);

        // Apply change in position from velocity
        newPosition += this.velocity;

        // Return new position after initial movement processing
        return newPosition;
    }

    public void ProcessHorizontalMovement(bool leftButtonDown, bool rightButtonDown)
    {
        //if (IsInActionableState)
        //{
            if (leftButtonDown)
            {
                //this.velocity.x--;
                this.velocity.x = Mathf.Max(
                    this.velocity.x - this.acceleration,
                    this.maxSpeed * -1
                    );
            }
            if (rightButtonDown)
            {
                //this.velocity.x++;
                this.velocity.x = Mathf.Min(
                    this.velocity.x + this.acceleration,
                    this.maxSpeed
                    );
            }
        //}
    }

    public void ProcessWallHugOrBounce(bool movingIntoWallButtonDown, bool jumpButtonDown, bool wallIsOnRight)
    {
        if (wallIsOnRight)
        {
            // Player is colliding with a wall to their right
            if (!movingIntoWallButtonDown)
            {
                if (this.velocity.x > HFConstants.BOUNCE_OFF_THRESHOLD)
                {
                    // Bounce off wall instead of stopping if velocity is high enough.
                    this.velocity.x = Mathf.Abs(this.velocity.x) * -HFConstants.BOUNCE_OFF_MULTIPLIER;
                }
                else
                {
                    this.velocity.x = 0f;

                    // If player was going to stick to wall, check for wall jump
                    if (jumpButtonDown)
                    {
                        // Wall jump (jump off of wall)
                        if (!this.isOnGround)
                        {
                            this.velocity.x = -WALL_JUMP_VELOCITY;
                            this.velocity.y = WALL_JUMP_VELOCITY;
                            facingRight = false;
                            TriggerJump();
                        }
                        this.bounceOffEnergy = 0f;
                    }
                }
            }
            else
            {
                // Holding button to move into + against collided wall
                facingRight = true;
                
                if (jumpButtonDown)
                {
                    // Wall jump (jump off of wall)
                    if (!this.isOnGround)
                    {
                        this.velocity.x = -WALL_JUMP_VELOCITY;
                        this.velocity.y = WALL_JUMP_VELOCITY;
                        facingRight = false;
                        TriggerJump();
                    }
                    this.bounceOffEnergy = 0f;
                }
                else
                {
                    if (this.bounceOffEnergy > HFConstants.MINIMUM_NONZERO_FLOAT_THRESHOLD)
                    {
                        this.bounceOffEnergy *= HFConstants.BOUNCE_MOMENTUM_DECAY_RATE;
                    }
                    else
                    {
                        if (this.velocity.x > HFConstants.BOUNCE_OFF_THRESHOLD)
                        {
                            this.bounceOffEnergy = Math.Abs(this.velocity.x);
                        }
                        else
                        {
                            this.bounceOffEnergy = 0f;
                        }
                    }
                    this.velocity.x = 0f;
                }
            }
        }
        else
        {
            // Player is colliding with a wall to their left
            if (!movingIntoWallButtonDown)
            {
                if (this.velocity.x < -HFConstants.BOUNCE_OFF_THRESHOLD)
                {
                    // Bounce off wall instead of stopping if velocity is high enough.
                    this.velocity.x = Mathf.Abs(this.velocity.x) * HFConstants.BOUNCE_OFF_MULTIPLIER;
                }
                else
                {
                    this.velocity.x = 0f;

                    // If player was going to stick to wall, check for wall jump
                    if (jumpButtonDown)
                    {
                        // Wall jump (jump off of wall)
                        if (!this.isOnGround)
                        {
                            this.velocity.x = WALL_JUMP_VELOCITY;
                            this.velocity.y = WALL_JUMP_VELOCITY;
                            facingRight = true;
                            TriggerJump();
                        }
                        this.bounceOffEnergy = 0f;
                    }
                }
            }
            else
            {
                // Holding button to move into + against collided wall
                facingRight = false;
                
                if (jumpButtonDown)
                {
                    // Wall jump (jump off of wall)
                    if (!this.isOnGround)
                    {
                        this.velocity.x = WALL_JUMP_VELOCITY;
                        this.velocity.y = WALL_JUMP_VELOCITY;
                        facingRight = true;
                        TriggerJump();
                    }
                    this.bounceOffEnergy = 0f;
                }
                else
                {
                    if (this.bounceOffEnergy > HFConstants.MINIMUM_NONZERO_FLOAT_THRESHOLD)
                    {
                        this.bounceOffEnergy *= HFConstants.BOUNCE_MOMENTUM_DECAY_RATE;
                    }
                    else
                    {
                        if (this.velocity.x < -HFConstants.BOUNCE_OFF_THRESHOLD)
                        {
                            this.bounceOffEnergy = Math.Abs(this.velocity.x);
                        }
                        else
                        {
                            this.bounceOffEnergy = 0f;
                        }
                    }
                    this.velocity.x = 0f;
                }
            }
        }
    }

    //public void ProcessThisPlayerLandedTheirAttack(AttackInteractionData attackInteractionData)
    //{
    //    // this function exists to allow for logic like
    //    // "spawn entity/projectile on hit"
    //    // or "change animation on hit"
    //}

    #endregion

    #region Game logic event calls for triggering animation state changes, etc. Virtual to allow overrides.

    protected virtual void TriggerJump()
    {

    }
    protected virtual void TriggerLand()
    {

    }

    #endregion

    #region Game logic helper/state functions
    public Rect GetHurtbox()
    {
        Rect hurtboxToReturn = new Rect();

        hurtboxToReturn.width = hurtboxSize.x;
        hurtboxToReturn.height = hurtboxSize.y;
        hurtboxToReturn.x = position.x - (hurtboxSize.x / 2f);
        hurtboxToReturn.y = position.y + (hurtboxSize.y);

        return hurtboxToReturn;
    }

    public Rect GetHurtboxRelative()
    {
        Rect hurtboxToReturn = new Rect();

        hurtboxToReturn.width = hurtboxSize.x;
        hurtboxToReturn.height = hurtboxSize.y;
        hurtboxToReturn.x = 0f - (hurtboxSize.x / 2f);
        hurtboxToReturn.y = hurtboxSize.y;
        //hurtboxToReturn.x = 0f;
        //hurtboxToReturn.y = (hurtboxSize.y / 2f);

        return hurtboxToReturn;
    }

    public HitboxData[] GetHitboxes()
    {
        HitboxData[] hitboxesToReturn = new HitboxData[currentAnimationState.CurrentFrame.hitboxes.Count];

        float facingRightMultiplier = facingRight ? 1f : -1f;
        Vector2 playerPositionOffset = new Vector2(
                position.x,
                position.y
            );

        for (int i = 0; i < hitboxesToReturn.Length; i++)
        {
            hitboxesToReturn[i] = currentAnimationState.CurrentFrame.hitboxes[i];

            // If player is facing left, effective active hitbox data needs to be adjusted to account for this when retrieved.
            if (!facingRight)
            {
                // Flip the hitbox's inherent x offset to the left
                hitboxesToReturn[i].hitboxRect.x -= 2 * (hitboxesToReturn[i].hitboxRect.x);
                // Account for the 'anchor' of Rects being in the top-left corner, while OUR frame data anchors on bottom-left.
                // (and then flipping part of that around for facing left - because our flipped frame data anchors bottom-right.)
                // X
                hitboxesToReturn[i].hitboxRect.x -= (hitboxesToReturn[i].hitboxRect.width);
            }

            // Account for the 'anchor' of Rects being in the top-left corner, while OUR frame data anchors on bottom-left.
            // Y
            hitboxesToReturn[i].hitboxRect.y += hitboxesToReturn[i].hitboxRect.height;

            hitboxesToReturn[i].hitboxRect.position += playerPositionOffset;

            //Debug.Log(facingRightMultiplier);
            //Debug.Log(player.facingRight);
            //Debug.Log(hitboxesToReturn[i]);
        }

        //Debug.Log(hitboxesToReturn);
        return hitboxesToReturn;
    }

    protected void ChangeAnimationState(string newAnimationState)
    {
        currentAnimationState = animationsAllData[newAnimationState];
        currentAnimationState.currentFrameNumber = 0;

        switch (newAnimationState)
        {
            case "idle":
                playerState = PlayerState.IDLE;
                break;
            case "run":
                playerState = PlayerState.RUNNING;
                break;
            case "jump":
                playerState = PlayerState.JUMPING;
                break;
            case "fall":
                playerState = PlayerState.FALLING;
                break;
            case "attackA1":
            case "attackB1":
            case "attackC1":
                playerState = PlayerState.ATTACKING;
                break;
            case "hitstun":
                playerState = (isOnGround) ? PlayerState.HITSTUN_GROUNDED : PlayerState.HITSTUN_AIRBORNE;
                break;
        }
    }

    public void InflictDamageAndHitstunAndKnockback(HitboxData hitboxData, bool isAttackerFacingRight)
    {
        ChangeAnimationState("hitstun");

        // [ Damage, hitstun, knockback. ] //
        
        // Damage
        health -= hitboxData.damage;

        // Hitstun
        if (hitstun < hitboxData.hitstunInflicted)
        {
            hitstun = hitboxData.hitstunInflicted;
        }

        //Knockback
        if (hitboxData.knockbackReplacesInsteadOfAdds)
        {
            velocity = hitboxData.knockbackInflicted;
            if (!isAttackerFacingRight)
            {
                velocity.x *= -1;
            }
        }
        else
        {
            Vector2 addToVelocity = hitboxData.knockbackInflicted;
            if (!isAttackerFacingRight)
            {
                addToVelocity *= -1;
            }
            velocity += addToVelocity;
        }
    }

    //public bool InflictShieldDamage(HitboxData hitboxData)
    //{
    //    // If shield was broken, return true.

    //    // Else, return false.
    //    return false;
    //}
    
    public bool IsInHitstun
    {
        get
        {
            List<PlayerState> playerStatesToCheckAgainst = new List<PlayerState>{
                PlayerState.HITSTUN_AIRBORNE,
                PlayerState.HITSTUN_GROUNDED
            };
            if (playerStatesToCheckAgainst.Contains(this.playerState)) { return true; }
            return false;
        }
    }

    public bool IsRecoveringPostHitstun
    {
        get
        {
            List<PlayerState> playerStatesToCheckAgainst = new List<PlayerState>{
                PlayerState.GETTING_UP_FROM_FLOOR
            };
            if (playerStatesToCheckAgainst.Contains(this.playerState)) { return true; }
            return false;
        }
    }

    public bool IsInBlockingState
    {
        get
        {
            List<PlayerState> playerStatesToCheckAgainst = new List<PlayerState>{
                PlayerState.BLOCKING
            };
            if (playerStatesToCheckAgainst.Contains(this.playerState)) { return true; }
            return false;
        }
    }

    public bool IsInAttackingState
    {
        get
        {
            List<PlayerState> playerStatesToCheckAgainst = new List<PlayerState>{
                PlayerState.ATTACKING
            };
            if (playerStatesToCheckAgainst.Contains(this.playerState)) { return true; }
            return false;
        }
    }

    public bool IsInActionableState
    {
        get
        {
            List<PlayerState> playerStatesToCheckAgainst = new List<PlayerState>{
                PlayerState.FALLING,
                PlayerState.IDLE,
                PlayerState.JUMPING,
                PlayerState.JUMPSQUAT,
                PlayerState.RUNNING
            };
            if (playerStatesToCheckAgainst.Contains(this.playerState)) { return true; }
            return false;
        }
    }
    #endregion

    #region Serialization methods
    public void Serialize(BinaryWriter bw)
    {
        bw.Write(position.x);
        bw.Write(position.y);
        bw.Write(velocity.x);
        bw.Write(velocity.y);
        bw.Write(collisionBoxSize.x);
        bw.Write(collisionBoxSize.y);
        bw.Write(health);
        bw.Write(hitstun);
        bw.Write(bouncy);
        bw.Write(characterName);
        bw.Write(jumpPower);
    }

    public void Deserialize(BinaryReader br)
    {
        position.x = br.ReadSingle();
        position.y = br.ReadSingle();
        velocity.x = br.ReadSingle();
        velocity.y = br.ReadSingle();
        collisionBoxSize.x = br.ReadSingle();
        collisionBoxSize.y = br.ReadSingle();
        health = br.ReadInt32();
        hitstun = br.ReadInt32();
        bouncy = br.ReadBoolean();
        characterName = br.ReadString();
        jumpPower = br.ReadSingle();
    }

    public override int GetHashCode()
    {
        int hashCode = 1858597544;
        hashCode = hashCode * -1521134295 + position.GetHashCode();
        hashCode = hashCode * -1521134295 + velocity.GetHashCode();
        hashCode = hashCode * -1521134295 + collisionBoxSize.GetHashCode();
        hashCode = hashCode * -1521134295 + health.GetHashCode();
        hashCode = hashCode * -1521134295 + hitstun.GetHashCode();
        hashCode = hashCode * -1521134295 + bouncy.GetHashCode();
        hashCode = hashCode * -1521134295 + characterName.GetHashCode();
        hashCode = hashCode * -1521134295 + jumpPower.GetHashCode();
        return hashCode;
    }
    #endregion
}