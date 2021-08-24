using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
}

public enum PlayerState
{

}

[Serializable]
public class Player
{
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 collisionBoxSize;
    public int health;
    public int hitstun;
    public string characterName;

    public void Serialize(BinaryWriter bw)
    {

    }

    public void Deserialize(BinaryReader br)
    {

    }
}

public class HfGame
{
    public int frameNumber;
    public Player[] players;

    public static Rect bounds = new Rect(0, 0, 1280, 720);
    public float gravity = 1.2f;
    public float frictionMultiplier = 0.9f;

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
        }
        players[0].velocity = new Vector2(200f, 750f) * HFConstants.FRAME_RATE_SPEED_SCALE_MULTIPLIER;
        players[1].velocity = new Vector2(500f, 1500f) * HFConstants.FRAME_RATE_SPEED_SCALE_MULTIPLIER;
    }

    public void AdvanceFrame()
    {
        for (int p = 0; p < players.Length; p++)
        {
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
                players[p].velocity.y -= gravity;
            }
            if (newPosition.y <= HfGame.bounds.yMin && players[p].velocity.y == 0f)
            {
                // Player is on ground. Apply friction!
                if (Mathf.Abs(players[p].velocity.x) < 1f)
                {
                    players[p].velocity.x = 0;
                }
                else
                {
                    players[p].velocity.x *= frictionMultiplier;
                }
            }

            // Enforce screen boundaries
            newPosition.x = Mathf.Clamp(newPosition.x, HfGame.bounds.xMin, HfGame.bounds.xMax);
            newPosition.y = Mathf.Clamp(newPosition.y, HfGame.bounds.yMin, HfGame.bounds.yMax);
            players[p].position = newPosition;

            // Debug info
            Debug.Log($"Player {p + 1} velocity: {players[p].velocity.x}, {players[p].velocity.y}");
        }
    }

    public void ParsePlayerInputs()
    {

    }

    public void Serialize()
    {

    }

    public void Deserialize()
    {

    }
}
