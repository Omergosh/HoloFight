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
public class Player
{
    // State variables - likely to change often
    public Vector2 position;
    public Vector2 velocity;
    public int health = 100;
    public int hitstun = 0;
    public bool isOnGround = true;
    public int currentAttackId = 0;

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
}

[Serializable]
public struct HfGame
{
    public int frameNumber;
    public int checksum => GetHashCode();

    public Player[] players;

    public static Rect bounds = new Rect(0, 0, 1280, 720);

    public HfGame(int numberOfPlayers)
    {
        frameNumber = 0;
        players = new Player[numberOfPlayers];
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
    }

    public void AdvanceFrame(long[] inputs)
    {
        // 1. Increment frame number
        frameNumber++;

        // 2. Execute any relevant input processing that extends beyond the current frame's inputs
        //    (e.g. double taps, holding buttons, buffering inputs...)

        // 3. Progress character animations/states.
        //    Increment/decrement state variables such as hitstun, blockstun, shield health, etc.

        // 4. Process character movement
        // 5. Process projectile/object movement and creation
        // 6. Check for hits/blocks
        // 7. Finally, trigger a call to update visuals
        for (int p = 0; p < players.Length; p++)
        {
            // Parse inputs
            ParsePlayerInputs(inputs[p], p);

            Vector2 newPosition = new Vector2(players[p].position.x, players[p].position.y);

            // Apply change in position from velocity
            newPosition += players[p].velocity;

            // Apply forces such as gravity and friction
            if (newPosition.y <= HfGame.bounds.yMin && players[p].velocity.y <= 0f)
            {
                if (players[p].velocity.y > -5f)
                {
                    players[p].velocity.y = 0f;
                }
                else
                {
                    // Bounce off ground instead of landing if landing velocity is too high.
                    players[p].velocity.y = Mathf.Abs(players[p].velocity.y) * 0.6f;
                }
            }
            else
            {
                players[p].velocity.y -= GRAVITY;
            }
            if (newPosition.y <= HfGame.bounds.yMin && players[p].velocity.y == 0f)
            {
                // Player is on ground. If player is not moving along the ground
                if (!(ParseOnePlayerInput(inputs[p], p, INPUT_LEFT)
                    ^ ParseOnePlayerInput(inputs[p], p, INPUT_RIGHT)))
                {
                    // Apply friction!
                    if (Mathf.Abs(players[p].velocity.x) < 1f)
                    {
                        players[p].velocity.x = 0;
                    }
                    else
                    {
                        players[p].velocity.x *= FRICTION_MULTIPLIER;
                    }
                }
                // Player can jump!
                if (ParseOnePlayerInput(inputs[p], p, INPUT_UP))
                {
                    players[p].velocity.y = players[p].jumpPower;
                }
            }
            // Bump into / bounce off of horizontal walls
            if (newPosition.x <= HfGame.bounds.xMin && players[p].velocity.x <= 0f)
            {
                if (players[p].velocity.x < -5f && !ParseOnePlayerInput(inputs[p], p, INPUT_LEFT))
                {
                    // Bounce off wall instead of stopping if velocity is high enough.
                    players[p].velocity.x = Mathf.Abs(players[p].velocity.x) * 0.8f;
                }
                else
                {
                    players[p].velocity.x = 0f;
                }
            }
            if (newPosition.x >= HfGame.bounds.xMax && players[p].velocity.x >= 0f)
            {
                if (players[p].velocity.x > 5f && !ParseOnePlayerInput(inputs[p], p, INPUT_RIGHT))
                {
                    // Bounce off wall instead of stopping if velocity is high enough.
                    players[p].velocity.x = Mathf.Abs(players[p].velocity.x) * -0.8f;
                }
                else
                {
                    players[p].velocity.x = 0f;
                }
            }

            // Enforce screen boundaries
            newPosition.x = Mathf.Clamp(newPosition.x, HfGame.bounds.xMin, HfGame.bounds.xMax);
            newPosition.y = Mathf.Clamp(newPosition.y, HfGame.bounds.yMin, HfGame.bounds.yMax);
            players[p].position = newPosition;

            // Debug info
            Debug.Log($"Player {p + 1} velocity: {players[p].velocity.x}, {players[p].velocity.y}");
        }

        // Debug info
        Debug.Log($"Checksum: {checksum}");
    }

    public void ParsePlayerInputs(long inputs, int playerID)
    {
        if ((inputs & INPUT_LEFT) != 0)
        {
            //players[playerID].velocity.x--;
            players[playerID].velocity.x = Mathf.Max(
                players[playerID].velocity.x - players[playerID].acceleration,
                players[playerID].maxSpeed * -1
                );
        }
        if ((inputs & INPUT_RIGHT) != 0)
        {
            //players[playerID].velocity.x++;
            players[playerID].velocity.x = Mathf.Min(
                players[playerID].velocity.x + players[playerID].acceleration,
                players[playerID].maxSpeed
                );
        }
    }

    public bool ParseOnePlayerInput(long inputs, int playerID, int inputConstant)
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
