using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public HfGame game;// = new HfGame(2);
    bool isGamePaused = false;

    public GameObject p1GameObject;
    public GameObject p2GameObject;

    // Testing-relevant GameObjects, Components, Scripts, constants, etc.
    public GameObject boundsGameObject;
    public const float pixelsInWorldUnit = 100f;

    // Start is called before the first frame update
    void Start()
    {
        // THIS FIGHTING GAME IS A TWO PLAYER GAME (FOR NOW)
        game = new HfGame(2);
        boundsGameObject.transform.position = new Vector3(0f, 0f, 50f);
        boundsGameObject.transform.localScale = new Vector3(
            HfGame.bounds.width / pixelsInWorldUnit,
            HfGame.bounds.height / pixelsInWorldUnit);
    }

    void FixedUpdate()
    {
        if (!isGamePaused)
        {
            long[] inputs = new long[game.players.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = game.ReadInputs(i);
            }
            game.AdvanceFrame(inputs);
        }
    }

    void Update()
    {
        // update visuals to match current game state
        if (p1GameObject != null && p2GameObject != null)
        {
            //p1GameObject.transform.position = game.players[0].position;
            p1GameObject.transform.position = new Vector3(
                game.players[0].position.x - (HfGame.bounds.width / 2),
                game.players[0].position.y - (HfGame.bounds.height / 2)
                ) / pixelsInWorldUnit;
        }
        if (p1GameObject != null && p2GameObject != null)
        {
            //p2GameObject.transform.position = game.players[1].position;
            p2GameObject.transform.position = new Vector3(
                game.players[1].position.x - (HfGame.bounds.width / 2),
                game.players[1].position.y - (HfGame.bounds.height / 2)
                ) / pixelsInWorldUnit;
        }
    }
}
