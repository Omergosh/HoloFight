using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public HfGame game;// = new HfGame(2);
    bool isGamePaused = false;

    //public GameObject p1GameObject;
    //public GameObject p2GameObject;
    public PlayerAnimationController p1AnimationController;
    public PlayerAnimationController p2AnimationController;

    public CharacterGameDataSetScriptableObject CharacterGameDataIna;

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
        if(p1AnimationController != null)
        {
            p1AnimationController.SetPlayer(ref game.players[0]);
        }
        if(p2AnimationController != null)
        {
            p2AnimationController.SetPlayer(ref game.players[1]);
        }

        LoadFrameDataForPlayer(0);
        LoadFrameDataForPlayer(1);
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
        //// Update visuals to match current game state
        //if (p1GameObject != null)
        //{
        //    //p1GameObject.transform.position = game.players[0].position;
        //    p1GameObject.transform.position = new Vector3(
        //        game.players[0].position.x - (HfGame.bounds.width / 2),
        //        game.players[0].position.y - (HfGame.bounds.height / 2)
        //        ) / pixelsInWorldUnit;
        //}
        //if (p2GameObject != null)
        //{
        //    //p2GameObject.transform.position = game.players[1].position;
        //    p2GameObject.transform.position = new Vector3(
        //        game.players[1].position.x - (HfGame.bounds.width / 2),
        //        game.players[1].position.y - (HfGame.bounds.height / 2)
        //        ) / pixelsInWorldUnit;
        //}
    }

    public void LoadFrameDataForPlayer(int playerNumber)
    {
        string characterName = game.players[playerNumber].characterName.ToUpper();
        //string characterName = "INA";

        //Debug.Log(CharacterGameDataIna.GetType());
        //Debug.Log(CharacterGameDataIna.GetType().GetFields()[0].Name);
        //Debug.Log(CharacterGameDataIna.GetType().GetFields()[1].Name);
        //Debug.Log(CharacterGameDataIna.GetType().GetFields()[2].Name);
        Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[0].GetValue(CharacterGameDataIna));
        Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[1].GetValue(CharacterGameDataIna));
        Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[2].GetValue(CharacterGameDataIna));

        List<string> allCharacterFrameDataStrings = new List<string>();

        foreach(FieldInfo fieldInfo in CharacterGameDataIna.GetType().GetFields())
        {
            allCharacterFrameDataStrings.Add((string)fieldInfo.GetValue(CharacterGameDataIna));
        }

        //Debug.Log(allCharacterFrameDataStrings);
        //Debug.Log(allCharacterFrameDataStrings.Count);
        //Debug.Log(allCharacterFrameDataStrings[0]);
        //Debug.Log(allCharacterFrameDataStrings[1]);
        //Debug.Log(allCharacterFrameDataStrings[2]);
        //Debug.Log(allCharacterFrameDataStrings[3]);
        //Debug.Log(allCharacterFrameDataStrings[4]);

        foreach(string animationName in allCharacterFrameDataStrings)
        {
            AnimationStateData animationStateData = LoadSingleAnimationFrameData(characterName, animationName);

            game.players[playerNumber].animationsAllData.Add(animationName, animationStateData);
        }

        // Assume/Enforce that if a character has any animations, it MUST have an idle animation to default to and start in.
        if (game.players[playerNumber].animationsAllData.Keys.Count > 0)
        {
            game.players[playerNumber].currentAnimationState = game.players[playerNumber].animationsAllData["idle"];
        }
    }

    public AnimationStateData LoadSingleAnimationFrameData(string characterName, string animationName)
    {
        // Load current values (for animation currently being viewed) from a json file
        // in the frame data directory
        // (e.g.: Assets -> Characters -> FrameData -> INA)
        string str;

        //using (FileStream fs = new FileStream("Assets/Characters/FrameData/FBK/testFrameData.json", FileMode.Open))

        using (FileStream fs = new FileStream($"Assets/Characters/FrameData/{characterName}/{animationName}.json", FileMode.Open))
        {
            using (StreamReader reader = new StreamReader(fs))
            {
                str = reader.ReadLine();
                //reader.Close();
                reader.Dispose();
            }
            //fs.Close();
            fs.Dispose();
        }

        var serializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var loadedFrameData = JsonConvert.DeserializeObject<AnimationStateData>(str, serializerSettings);
        Debug.Log(loadedFrameData);
        Debug.Log($"Loaded current animation's frame data! Character: {characterName} Animation: {animationName}");

        return loadedFrameData;
    }
}
