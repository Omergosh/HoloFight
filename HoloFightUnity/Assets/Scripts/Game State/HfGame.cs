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
    public const float WALL_JUMP_VELOCITY = 24f;
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