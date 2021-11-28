using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static HFConstants;

public static class HFConstants
{
    public const float FRAME_RATE_SPEED_SCALE_MULTIPLIER = 1f / 60f;
    public const int MAX_PLAYERS = 2;


    public const int INPUT_BUFFER_WINDOW = 5;
    public const int INPUT_PREVIOUS_STORED_MAX = 30;

    public const int INPUT_LEFT = (1 << 0);
    public const int INPUT_RIGHT = (1 << 1);
    public const int INPUT_UP = (1 << 2);
    public const int INPUT_DOWN = (1 << 3);
    public const int INPUT_ATTACK_A = (1 << 4);
    public const int INPUT_ATTACK_B = (1 << 5);
    public const int INPUT_ATTACK_C = (1 << 6);
    public const int INPUT_DEFEND_D = (1 << 7);

    public const int FLOOR_Y = 600;
    public const float GRAVITY = 1.2f;
    public const float FRICTION_MULTIPLIER = 0.9f;
    public const float BOUNCE_OFF_THRESHOLD = 5f;
    public const float BOUNCE_OFF_MULTIPLIER = 0.7f;
    public const float BOUNCE_OFF_POPUP_MULTIPLIER = 0.8f;
    public const float BOUNCE_MOMENTUM_DECAY_RATE = 0.99f;
    public const float MINIMUM_NONZERO_FLOAT_THRESHOLD = 0.4f;
}

public enum PlayerState
{
    IDLE,
    RUNNING,
    JUMPSQUAT,
    JUMPING,
    FALLING,
    ATTACKING,
    BLOCKING,
    GUARD_BROKEN,
    HITSTUN_GROUNDED,
    HITSTUN_AIRBORNE,
    BOUNCING_OFF_GROUND,
    BOUNCING_OFF_WALL,
    CRUMPLED_ON_AIR,
    GETTING_UP_FROM_FLOOR,
    WALLSPLAT
}

[Serializable]
public struct InputData
{
    private LinkedList<long> inputsCurrentAndPrevious;

    public void Init()
    {
        inputsCurrentAndPrevious = new LinkedList<long>();
    }

    // Handling of entire sets of inputs
    public void AddCurrentInputs(long newInputs)
    {
        inputsCurrentAndPrevious.AddLast(newInputs);
        if (inputsCurrentAndPrevious.Count > HFConstants.INPUT_PREVIOUS_STORED_MAX)
        {
            inputsCurrentAndPrevious.RemoveFirst();
        }
    }

    public long CurrentInputs => inputsCurrentAndPrevious.Last.Value;

    // Public (not static) methods for individual input queries
    public bool GetInputDown(int inputConstant)
    {
        if((CurrentInputs & inputConstant) != 0)
        {
            return true;
        }
        return false;
    }
    public bool GetInputJustPressed(int inputConstant)
    {
        if((CurrentInputs & inputConstant) != 0)
        {
            if ((inputsCurrentAndPrevious.Last.Previous.Value & inputConstant) == 0)
            {
                return true;
            }
        }
        return false;
    }
    public bool GetInputHeldDown(int inputConstant, int durationInFrames)
    {
        // Validate duration parameter
        if(durationInFrames < 1)
        {
            return false;
        }

        LinkedListNode<long> currentInputsNode = inputsCurrentAndPrevious.Last;
        for(int i = 0; i < durationInFrames; i++)
        {
            if((currentInputsNode.Value & inputConstant) == 0)
            {
                return false;
            }
            currentInputsNode = currentInputsNode.Previous;
        }
        return true;
    }

    public bool GetInputJustReleased(int inputConstant)
    {
        if ((CurrentInputs & inputConstant) == 0)
        {
            if ((inputsCurrentAndPrevious.Last.Previous.Value & inputConstant) != 0)
            {
                return true;
            }
        }
        return false;
    }
}

[Serializable]
public class Player
{
    // State variables - likely to change often
    public Vector2 position;
    public Vector2 velocity;
    public bool isOnGround = true;
    public float bounceOffEnergy = 0;
    public int health = 100;
    public int hitstun = 0;
    public int currentAttackId = 0;
    public bool facingRight = true;

    // Variables that change less often
    public bool bouncy = false;

    // Data unlikely to change mid-match
    public int playerId;
    public int teamId; // If one-on-one/FFA, equal to playerId
    public Vector2 collisionBoxSize;
    public string characterName;
    public float jumpPower = 25f;
    public float maxSpeed = 26.0f;
    public float acceleration = 2.0f;

    // Frame/animation data
    public Dictionary<string, AnimationStateData> animationsAllData;
    public AnimationStateData currentAnimationState;

    private PlayerState playerState = PlayerState.IDLE;

    #region Game loop + logic methods

    // Overload this method to implement character-specific logic!
    public void ProcessAnimationStateLogic(InputData inputData)
    {
        // If a hitstun/blockstun state, decrement the appropriate value, and exit to another state where applicable

        // Based on current state and inputs, either:
        // - stay on this frame,
        // - advance current animation by one frame
        // - transition into another animation
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
                this.velocity.y = 0f;
            }
        }
        else
        {
            this.isOnGround = false;
            this.velocity.y -= GRAVITY;
        }

        // If player is on ground. (apply friction, check if player wants to jump)
        if (newPosition.y <= HfGame.bounds.yMin && this.velocity.y == 0f)
        {
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
            if (inputData.GetInputDown(INPUT_UP))
            {
                this.velocity.y = this.jumpPower;
            }
        }
        // Bump into / bounce off of horizontal walls
        if (newPosition.x <= HfGame.bounds.xMin && this.velocity.x <= 0f)
        {
            this.ProcessWallHugOrBounce(
               inputData.GetInputDown(INPUT_LEFT),
                false
                );
        }
        if (newPosition.x >= HfGame.bounds.xMax && this.velocity.x >= 0f)
        {
            this.ProcessWallHugOrBounce(
                inputData.GetInputDown(INPUT_RIGHT),
                true
                );
        }

        // Enforce screen boundaries 
        newPosition.x = Mathf.Clamp(newPosition.x, HfGame.bounds.xMin, HfGame.bounds.xMax);
        newPosition.y = Mathf.Clamp(newPosition.y, HfGame.bounds.yMin, HfGame.bounds.yMax);
        // (could be moved before the gravity/friction changes, which only affect velocity, not position)

        // After processing movement logic, update final position of player
        this.position = newPosition;
    }

    public Vector2 ProcessMovementBeforeCollisions(InputData inputData)
    {
        // If current animation frame prescribes changes to position/velocity, apply them here

        // If current animationstate allows for free horizontal movement, jumping, etc.
        // then handle that here// Parse inputs
        this.ProcessHorizontalMovement(
            inputData.GetInputDown(INPUT_LEFT),
            inputData.GetInputDown(INPUT_RIGHT)
            );

        Vector2 newPosition = new Vector2(this.position.x, this.position.y);

        // Apply change in position from velocity
        newPosition += this.velocity;

        // Return new position after initial movement processing
        return newPosition;
    }

    public void ProcessHorizontalMovement(bool leftButtonDown, bool rightButtonDown)
    {
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
    }

    public void ProcessWallHugOrBounce(bool movingIntoWallButtonDown, bool wallIsOnRight)
    {
        if (wallIsOnRight)
        {
            // Player is colliding with a wall to their right
            if (!movingIntoWallButtonDown)
            {
                // If stored momentum is present, use that instead of bouncing off normally
                if (this.bounceOffEnergy > HFConstants.MINIMUM_NONZERO_FLOAT_THRESHOLD)
                {
                    this.velocity.x = -1f * Mathf.Abs(this.bounceOffEnergy) * HFConstants.BOUNCE_OFF_MULTIPLIER;
                    if (!this.isOnGround)
                    {
                        this.velocity.y = Mathf.Abs(this.bounceOffEnergy) * HFConstants.BOUNCE_OFF_POPUP_MULTIPLIER;
                    }
                    this.bounceOffEnergy = 0f;
                }
                else if (this.velocity.x > HFConstants.BOUNCE_OFF_THRESHOLD)
                {
                    // Bounce off wall instead of stopping if velocity is high enough.
                    this.velocity.x = Mathf.Abs(this.velocity.x) * -HFConstants.BOUNCE_OFF_MULTIPLIER;
                }
                else
                {
                    this.velocity.x = 0f;
                }
            }
            else
            {
                // Holding button to move into + against collided wall
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
        else
        {
            // Player is colliding with a wall to their left
            if (!movingIntoWallButtonDown)
            {
                // If stored momentum is present, use that instead of bouncing off normally
                if (this.bounceOffEnergy > HFConstants.MINIMUM_NONZERO_FLOAT_THRESHOLD)
                {
                    this.velocity.x = Mathf.Abs(this.bounceOffEnergy) * HFConstants.BOUNCE_OFF_MULTIPLIER;
                    if (!this.isOnGround)
                    {
                        this.velocity.y = Mathf.Abs(this.bounceOffEnergy) * HFConstants.BOUNCE_OFF_POPUP_MULTIPLIER;
                    }
                    this.bounceOffEnergy = 0f;
                }
                else if (this.velocity.x < -HFConstants.BOUNCE_OFF_THRESHOLD)
                {
                    // Bounce off wall instead of stopping if velocity is high enough.
                    this.velocity.x = Mathf.Abs(this.velocity.x) * HFConstants.BOUNCE_OFF_MULTIPLIER;
                }
                else
                {
                    this.velocity.x = 0f;
                }
            }
            else
            {
                // Holding button to move into + against collided wall
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
    
    //public void ProcessThisPlayerLandedTheirAttack(AttackInteractionData attackInteractionData)
    //{
    //    // this function exists to allow for logic like
    //    // "spawn entity/projectile on hit"
    //    // or "change animation on hit"
    //}
    
    #endregion

    #region Game logic helper/state functions
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

[Serializable]
public struct HfGame
{
    public int frameNumber;
    public int checksum => GetHashCode();

    public InputData[] inputData;
    
    public Player[] players;

    public static Rect bounds = new Rect(0, 0, 1280, 720);

    public HfGame(int numberOfPlayers)
    {
        frameNumber = 0;
        players = new Player[numberOfPlayers];
        inputData = new InputData[numberOfPlayers];
        for (int p = 0; p < players.Length; p++)
        {
            inputData[p].Init();
        }
        Init();
        Debug.Log("Game initialized");
    }

    void Init()
    {
        for (int p = 0; p < players.Length; p++)
        {
            players[p] = new Player();
            players[p].playerId = p;
            players[p].teamId = p;
        }
        players[0].position = new Vector2(bounds.width * 0.5f / players.Length, 0f);
        players[1].position = new Vector2(bounds.width * (1 - (0.5f / players.Length)), 0f);
        players[0].velocity = new Vector2(200f, 750f) * FRAME_RATE_SPEED_SCALE_MULTIPLIER;
        players[1].velocity = new Vector2(-500f, 1500f) * FRAME_RATE_SPEED_SCALE_MULTIPLIER;
        players[0].characterName = "Ina";
        players[1].characterName = "Amelia";

        players[1].bouncy = true;
    }

    public void AdvanceFrame(long[] inputs)
    {
        // *---------------------------------------------------------------------------------------*
        // 1. Increment frame number
        // 2. Input processing
        //         Execute any relevant input processing that extends beyond the current frame's inputs
        //         (e.g. double taps, holding buttons, buffering inputs...)
        // 3. Progress character animations/states.
        //         Increment/decrement state variables such as hitstun, blockstun, shield health, etc.
        // 4. Process character movement
        // 5. Process projectile/object movement and creation
        // 6. Check for hits/blocks
        // 7. Finally, trigger a call to update visuals
        // *---------------------------------------------------------------------------------------*
        
        // 1. Increment frame number
        frameNumber++;
        Debug.Log($"Frame: {frameNumber}");

        // 2. Input processing
        for (int p = 0; p < players.Length; p++)
        {
            inputData[p].AddCurrentInputs(inputs[p]);
        }

        // 3. Process character animation states
        for (int p = 0; p < players.Length; p++)
        {
            //players[p].ProcessAnimationStateLogic(inputDatasets[p]);
        }

        // 4. Process character movement
        for (int p = 0; p < players.Length; p++)
        {
            players[p].ProcessAllMovement(inputData[p]);
            //Vector2 newPosition = new Vector2(players[p].position.x, players[p].position.y);

            // Apply each player's movement logic (dependent on their animationstate / player state)
            //newPosition = players[p].ProcessMovementBeforeCollisions(inputData[p]);

            // Process collisions between all players (and between players and objects/obstacles/walls)
            // Add check either in here or player function for valid wallhugging/walljumping state

            // Apply forces such as gravity/friction to velocities (but do not change positions here)

            // Final positions/velocities determined and assigned

            // Debug info
            Debug.Log($"Player {p + 1} velocity: {players[p].velocity.x}, {players[p].velocity.y}");
        }

        // 5. Projectile logic (DON'T IMPLEMENT YET)
        //      a. projectile movement
        //      b. projectile creation
        // (processed after movement so that new ones stay in their initial positions on the frame they're created)

        // Check for hits/blocks
        // 1. check for hitbox/hurtbox overlaps all at once
        // 2. THEN apply interaction logic all at once, to allow for things such as attacks trading (simultaneous hits), etc.
        // on contact: (send players into hitstun states, apply hitstun/blockstun, etc.)

        // Debug info
        Debug.Log($"Checksum: {checksum}");
    }

    public static bool ParseOnePlayerInput(long inputs, int playerID, int inputConstant)
    {
        return ((inputs & inputConstant) != 0);
    }

    public long ReadInputs(int playerID)
    {
        long input = 0;

        if (playerID == 0)
        {
            if (Input.GetKey(KeyCode.A))
            {
                input |= INPUT_LEFT;
            }
            if (Input.GetKey(KeyCode.D))
            {
                input |= INPUT_RIGHT;
            }
            if (Input.GetKey(KeyCode.W))
            {
                input |= INPUT_UP;
            }
            if (Input.GetKey(KeyCode.S))
            {
                input |= INPUT_DOWN;
            }
            if (Input.GetKey(KeyCode.Z))
            {
                input |= INPUT_ATTACK_A;
            }
            if (Input.GetKey(KeyCode.X))
            {
                input |= INPUT_ATTACK_B;
            }
            if (Input.GetKey(KeyCode.C))
            {
                input |= INPUT_ATTACK_C;
            }
            if (Input.GetKey(KeyCode.V))
            {
                input |= INPUT_DEFEND_D;
            }
        } else if(playerID == 1)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                input |= INPUT_LEFT;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                input |= INPUT_RIGHT;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                input |= INPUT_UP;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                input |= INPUT_DOWN;
            }
            if (Input.GetKey(KeyCode.U))
            {
                input |= INPUT_ATTACK_A;
            }
            if (Input.GetKey(KeyCode.I))
            {
                input |= INPUT_ATTACK_B;
            }
            if (Input.GetKey(KeyCode.O))
            {
                input |= INPUT_ATTACK_C;
            }
            if (Input.GetKey(KeyCode.P))
            {
                input |= INPUT_DEFEND_D;
            }
        }

        return input;
    }

    public void Serialize()
    {

    }

    public void Deserialize()
    {

    }

    public override int GetHashCode()
    {
        int hashCode = -1214587014;
        hashCode = hashCode * -1521134295 + frameNumber.GetHashCode();
        foreach (var player in players)
        {
            hashCode = hashCode * -1521134295 + player.GetHashCode();
        }
        return hashCode;
    }
}
