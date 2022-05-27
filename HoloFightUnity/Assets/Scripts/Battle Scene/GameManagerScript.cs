using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

using static HFConstants;

public class GameManagerScript : MonoBehaviour
{
    public HfGame game;// = new HfGame(2);
    bool isGamePaused = false;

    public InputManagerScript inputManager;
    
    public PlayerAnimationController p1AnimationController;
    public PlayerAnimationController p2AnimationController;

    public CharacterGameDataSetScriptableObject CharacterGameDataIna;

    // Testing-relevant GameObjects, Components, Scripts, constants, etc.
    public GameObject boundsGameObject;
    public const float pixelsInWorldUnit = 100f;

    // UI-relevant values
    public bool pauseHeldActivationShowTimer = false;
    public float pauseHeldActivationTimer = 0f;
    public float pauseHeldActivationTimerMax;

    // Start is called before the first frame update
    void Start()
    {
        // UI-related set-up and such (not relating directly to game state)
        pauseHeldActivationTimerMax = inputManager.p1PauseAction.interactions.Length;


        // || Game State set-up || //
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
                inputs[i] = ReadGameInputs(i);
            }
            game.AdvanceFrame(inputs);
        }
    }

    void Update()
    {
        //// Update visuals to match current game state
        // (nvm lol this happens elsewhere)
        PauseGameCheck();
    }

    long ReadGameInputs(int playerID)
    {
        long input = 0;

        if (playerID == 0)
        {
            if (inputManager.p1UpValue)
            {
                input |= INPUT_UP;
            }
            if (inputManager.p1LeftValue)
            {
                input |= INPUT_LEFT;
            }
            if (inputManager.p1DownValue)
            {
                input |= INPUT_DOWN;
            }
            if (inputManager.p1RightValue)
            {
                input |= INPUT_RIGHT;
            }
            if (inputManager.p1AttackAValue)
            {
                input |= INPUT_ATTACK_A;
            }
            if (inputManager.p1AttackBValue)
            {
                input |= INPUT_ATTACK_B;
            }
            if (inputManager.p1AttackCValue)
            {
                input |= INPUT_ATTACK_C;
            }
            if (inputManager.p1AssistDValue)
            {
                input |= INPUT_DEFEND_D;
            }
        }

        return input;
    }

    void PauseGameCheck()
    {
        // Same function is used for both pausing and unpausing the game.
        // (subject to change if it proves necessary to separate them)
        if (!isGamePaused)
        {
            // Pause
            if (inputManager.p1PauseWasPressedThisFrame)
            {
                // If pause button was just pressed this frame, activate some UI
                Debug.Log("show pause timer");
            }
            else if(inputManager.p1PauseWasReleasedThisFrame)
            {
                // If pause button UI was shown, remove it because we're not pausing now after all!
                Debug.Log("hide pause timer");
            }
            else if(inputManager.p1PauseFullyHeldForDuration)
            {
                // If pause button was held for long enough, actually pause the game
                Debug.Log("GAME PAUSED");
                isGamePaused = true;
            }
        }
        else
        {
            // Unpause
            if (inputManager.p1PauseWasPressedThisFrame)
            {
                // Unlike pausing, unpausing occurs instantly upon pressing the pause button.
                Debug.Log("GAME UNPAUSED");
                isGamePaused = false;
            }
        }
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
        Debug.Log((TextAsset)CharacterGameDataIna.GetType().GetFields()[8].GetValue(CharacterGameDataIna));
        Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[8].GetValue(CharacterGameDataIna).ToString());
        Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[0].GetValue(CharacterGameDataIna).ToString());
        Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[1].GetValue(CharacterGameDataIna));
        Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[2].GetValue(CharacterGameDataIna));

        List<AnimationStateData> allCharacterFrameData = new List<AnimationStateData>();

        // JSON deserialization preparation
        string frameDataString;
        var serializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        foreach (FieldInfo fieldInfo in CharacterGameDataIna.GetType().GetFields())
        {
            Debug.Log(fieldInfo.FieldType.FullName);
            Debug.Log(fieldInfo.FieldType.FullName == "UnityEngine.TextAsset");
            if (fieldInfo.FieldType.FullName == "UnityEngine.TextAsset")
            {
                Debug.Log(fieldInfo.Name);
                frameDataString = fieldInfo.GetValue(CharacterGameDataIna).ToString();
                var loadedFrameData = JsonConvert.DeserializeObject<AnimationStateData>(frameDataString, serializerSettings);
                allCharacterFrameData.Add(loadedFrameData);
            }

        }

        Debug.Log("Loading imported data into character classes");
        foreach (AnimationStateData animationFrameData in allCharacterFrameData)
        {
            string animationName = animationFrameData.animationName;
            Debug.Log(animationFrameData);
            Debug.Log(animationName);

            game.players[playerNumber].animationsAllData.Add(animationName, animationFrameData);
        }

        //Debug.Log(loadedFrameData);
        //Debug.Log($"Loaded current animation's frame data! Character: {characterName} Animation: {animationName}");

        // Assume/Enforce that if a character has any animations, it MUST have an idle animation to default to and start in.
        if (game.players[playerNumber].animationsAllData.Keys.Count > 0)
        {
            game.players[playerNumber].currentAnimationState = game.players[playerNumber].animationsAllData["idle"];
        }
    }

    // || Old version that only works in-editor and not in any Builds of the game || //
    //public void LoadFrameDataForPlayer(int playerNumber)
    //{
    //    string characterName = game.players[playerNumber].characterName.ToUpper();
    //    //string characterName = "INA";

    //    //Debug.Log(CharacterGameDataIna.GetType());
    //    //Debug.Log(CharacterGameDataIna.GetType().GetFields()[0].Name);
    //    //Debug.Log(CharacterGameDataIna.GetType().GetFields()[1].Name);
    //    //Debug.Log(CharacterGameDataIna.GetType().GetFields()[2].Name);
    //    Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[0].GetValue(CharacterGameDataIna));
    //    Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[1].GetValue(CharacterGameDataIna));
    //    Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[2].GetValue(CharacterGameDataIna));

    //    List<string> allCharacterFrameDataStrings = new List<string>();

    //    foreach (FieldInfo fieldInfo in CharacterGameDataIna.GetType().GetFields())
    //    {
    //        allCharacterFrameDataStrings.Add((string)fieldInfo.GetValue(CharacterGameDataIna));
    //    }

    //    //Debug.Log(allCharacterFrameDataStrings);
    //    //Debug.Log(allCharacterFrameDataStrings.Count);
    //    //Debug.Log(allCharacterFrameDataStrings[0]);
    //    //Debug.Log(allCharacterFrameDataStrings[1]);
    //    //Debug.Log(allCharacterFrameDataStrings[2]);
    //    //Debug.Log(allCharacterFrameDataStrings[3]);
    //    //Debug.Log(allCharacterFrameDataStrings[4]);

    //    foreach (string animationName in allCharacterFrameDataStrings)
    //    {
    //        AnimationStateData animationStateData = LoadSingleAnimationFrameData(characterName, animationName);

    //        game.players[playerNumber].animationsAllData.Add(animationName, animationStateData);
    //    }

    //    // Assume/Enforce that if a character has any animations, it MUST have an idle animation to default to and start in.
    //    if (game.players[playerNumber].animationsAllData.Keys.Count > 0)
    //    {
    //        game.players[playerNumber].currentAnimationState = game.players[playerNumber].animationsAllData["idle"];
    //    }
    //}

    // || Old approach that only works in-editor and not in any Builds of the game || //
    //public AnimationStateData LoadSingleAnimationFrameData(string characterName, string animationName)
    //{
    //    // Load current values (for animation currently being viewed) from a json file
    //    // in the frame data directory
    //    // (e.g.: Assets -> Characters -> FrameData -> INA)
    //    string str;

    //    //using (FileStream fs = new FileStream("Assets/Characters/FrameData/FBK/testFrameData.json", FileMode.Open))

    //    using (FileStream fs = new FileStream($"Assets/Characters/FrameData/{characterName}/{animationName}.json", FileMode.Open))
    //    {
    //        using (StreamReader reader = new StreamReader(fs))
    //        {
    //            str = reader.ReadLine();
    //            //reader.Close();
    //            reader.Dispose();
    //        }
    //        //fs.Close();
    //        fs.Dispose();
    //    }

    //    var serializerSettings = new JsonSerializerSettings()
    //    {
    //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    //    };
    //    var loadedFrameData = JsonConvert.DeserializeObject<AnimationStateData>(str, serializerSettings);
    //    Debug.Log(loadedFrameData);
    //    Debug.Log($"Loaded current animation's frame data! Character: {characterName} Animation: {animationName}");

    //    return loadedFrameData;
    //}
}
