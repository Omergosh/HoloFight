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

    public const int HITSTOP_FRAMES_UNIVERSAL = 2;
    public const int ALREADY_HIT_ATTACKID_MAX_BUFFER = 10;

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

public enum CurrentBattleProgress
{
    // Before the first round starts. Character intros and such roll out, BGM starts, assets load in (if not already) loaded, etc.
    WAITING_FOR_FIRST_ROUND,
    // Counting down to the start of the current round.
    ROUND_COUNTDOWN,
    // Players are playing the game. Fighting is happening. Combat in progress.
    //  (Here, the game can be paused.)
    ROUND_IN_PROGRESS,
    // Current round just ended due to scores, health, time, or whatever the condition is. Transitions to countdown for the next round.
    ROUND_OVER,
    // Final round of combat has ended. Match end UI menu appears on screen.
    GAME_OVER
}


[Serializable]
public struct HfGame
{
    public int frameNumber;
    public int checksum => GetHashCode();

    public InputData[] inputData;

    public Player[] players;

    public static Rect bounds = new Rect(0, 0, 1280, 720);

    public int hitstopFramesRemaining;
    public static int lastAttackId = 1;
    public static int GetNewAttackId
    {
        get
        {
            lastAttackId++;
            return lastAttackId;
        }
    }

    public CurrentBattleProgress currentBattleProgress;
    public int roundsPlayed;
    //public bool gameStarted;
    //public bool gameEnded;
    //public bool roundStarted;
    //public bool roundEnded;

    // Match end results
    public int winnerPlayerIndex;

    public const int targetFrameRate = 60;
    public const int roundTimerMax = 60;
    public int roundTimerCurrentInFrames;
    public int RoundTimerCurrent => (int)Math.Ceiling(
        ((decimal)roundTimerMax) - 
        ((decimal)roundTimerCurrentInFrames / (decimal)targetFrameRate)
        );

    public HfGame(int numberOfPlayers)
    {
        frameNumber = 0;
        roundTimerCurrentInFrames = 0;
        roundsPlayed = 0;
        currentBattleProgress = CurrentBattleProgress.WAITING_FOR_FIRST_ROUND;

        hitstopFramesRemaining = 0;
        //lastAttackId = 1;

        players = new Player[numberOfPlayers];

        inputData = new InputData[numberOfPlayers];
        for (int p = 0; p < players.Length; p++)
        {
            inputData[p].Init();
        }
        winnerPlayerIndex = -1;
        InitPlayers();
        Debug.Log("Game initialized");
    }

    void InitPlayers()
    {
        for (int p = 0; p < players.Length; p++)
        {
            //players[p] = new Player();

            // Assign selected characters to players (hardcoded for now)
            //if (p == 0)
            //{
                players[p] = new PlayerIna();
            //}

            players[p].playerId = p;
            players[p].teamId = p;
        }

        players[0].position = new Vector2(bounds.width * 0.5f / players.Length, 0f);
        players[1].position = new Vector2(bounds.width * (1 - (0.5f / players.Length)), 0f);
        //players[0].velocity = new Vector2(-200f, 750f) * FRAME_RATE_SPEED_SCALE_MULTIPLIER;
        //players[1].velocity = new Vector2(500f, 1500f) * FRAME_RATE_SPEED_SCALE_MULTIPLIER;
        //players[0].characterName = "Ina";
        //players[1].characterName = "Ina";

        players[1].facingRight = false;

        //players[1].bouncy = true;
    }

    public void AdvanceFrame(long[] inputs)
    {
        if (hitstopFramesRemaining > 0)
        {
            hitstopFramesRemaining--;
        }
        else
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
            // 7. Finally, trigger a call to update visuals (if necessary)
            // *------------------------------------------*
            // 8. Check match end conditions
            // *---------------------------------------------------------------------------------------*

            // 1. Increment frame number
            frameNumber++;
            // (if the timer is not paused for round intros, hitstop, cinematic supers, or anything else...
            // ...then increment the round timer too.)
            roundTimerCurrentInFrames++;
            //Debug.Log($"Frame: {frameNumber}");

            // 2. Input processing
            for (int p = 0; p < players.Length; p++)
            {
                inputData[p].AddCurrentInputs(inputs[p]);
            }

            // 3. Process character animation states
            for (int p = 0; p < players.Length; p++)
            {
                players[p].ProcessAnimationStateLogic(inputData[p]);
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
                //Debug.Log($"Player {p + 1} velocity: {players[p].velocity.x}, {players[p].velocity.y}");
            }

            // 5. Projectile logic (DON'T IMPLEMENT YET)
            //      a. projectile movement
            //      b. projectile creation
            //         (processed after movement so that new ones stay in their initial positions on the frame they're created)

            // 6. Check for hits/blocks
            //      a. check for hitbox/hurtbox overlaps all at once
            //      b. THEN apply interaction logic all at once, to allow for things such as attacks trading (simultaneous hits), etc.
            //         on contact: (send players into hitstun states, apply hitstun/blockstun, etc.)
            for (int p = 0; p < players.Length; p++)
            {
                HitboxData[] currentHitboxes = players[p].GetHitboxes();
                if (currentHitboxes.Length > 0)
                {
                    //Debug.Log(currentHitboxes[0].hitboxRect);
                    for (int q = 0; q < players.Length; q++)
                    {
                        if (players[p].teamId != players[q].teamId)
                        {
                            Rect hurtboxToCheck = players[q].GetHurtbox();
                            //Debug.Log(hurtboxToCheck.Overlaps(currentHitboxes[0].hitboxRect));
                            if (hurtboxToCheck.Overlaps(currentHitboxes[0].hitboxRect))
                            {
                                if (!players[q].alreadyHitAttackIds.Contains(players[p].currentAttackId)) // Check if player has been hit by the same hitbox
                                {
                                    players[q].InflictDamageAndHitstunAndKnockback(currentHitboxes[0], players[p].facingRight, players[p].velocity);
                                    players[q].facingRight = !players[p].facingRight;
                                    players[q].alreadyHitAttackIds.Add(players[p].currentAttackId);

                                    players[p].currentAttackLandedHit = true;

                                    hitstopFramesRemaining = HITSTOP_FRAMES_UNIVERSAL;
                                }
                            }
                        }
                    }
                }
            }

            // 8. Check match end conditions
            CheckMatchEndConditions();
        }

        // Debug info
        //Debug.Log($"Checksum: {checksum}");
    }

    public int CheckMatchEndConditions()
    {
        bool p1Dead = false;
        bool p2Dead = false;
        for (int p = 0; p < players.Length; p++)
        {
            if (players[p].health <= 0)
            {
                if (p == 0)
                {
                    p1Dead = true;
                }
                else
                {
                    p2Dead = true;
                }
            }
        }
        if (p1Dead || p2Dead)
        {
            currentBattleProgress = CurrentBattleProgress.GAME_OVER;
            if (!p1Dead)
            {
                winnerPlayerIndex = 0;
            }
            else if (!p2Dead)
            {
                winnerPlayerIndex = 1;
            }
            else
            {
                winnerPlayerIndex = -1;
            }
        }
        if (roundTimerCurrentInFrames >= roundTimerMax * targetFrameRate)
        {
            currentBattleProgress = CurrentBattleProgress.GAME_OVER;
            if (players[0].health > players[1].health)
            {
                winnerPlayerIndex = 0;
            }
            else if (players[1].health > players[0].health)
            {
                winnerPlayerIndex = 1;
            }
            else
            {
                winnerPlayerIndex = -1;
            }
        }
        return winnerPlayerIndex;
    }

    public static bool ParseOnePlayerInput(long inputs, int playerID, int inputConstant)
    {
        return ((inputs & inputConstant) != 0);
    }

    #region Serialization methods

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
    #endregion
}
