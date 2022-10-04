using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static HFConstants;

public class BattleManagerScript : MonoBehaviour
{
    public HfGame game;// = new HfGame(2);

    bool isGamePaused = false;
    bool matchEndScreenUp = false;

    public GameObject prefabPlayerIna;

    public GameObject[] playerGameObjects;

    public PlayerInput[] playerInputs;
    public PlayerInputScript[] playerInputScripts;

    public PlayerAnimationController[] playerAnimationControllers;

    public CharacterGameDataSetScriptableObject CharacterGameDataIna;

    // Testing-relevant GameObjects, Components, Scripts, constants, etc.
    public GameObject boundsGameObject;
    public const float pixelsInWorldUnit = 100f;

    // Round start countdown
    float roundStartTimer = 3.0f;
    bool roundStartCountdownEnded = false;

    // Match end results
    int winnerPlayerIndex = -1;
    string winnerCharacterName = "";

    // UI-relevant values
    public bool pauseHeldActivationShowTimer = false;
    public float pauseHeldActivationTimer = 0f;
    public float pauseHeldActivationTimerMax;

    public BattleUIScript battleUIScript;

    // Input device configuration variables
    // (TODO: have these variables passed in from character select screen,
    //  where players decide on the input devices to be used)
    // (Schemes for reference: KeyboardP1Scheme, KeyboardP2Scheme, GamepadScheme)
    public string controlSchemeP1 = "KeyboardP1Scheme";
    public string controlSchemeP2 = "KeyboardP2Scheme";

    // Start is called before the first frame update
    void Start()
    {

        // Input Setup
        // Set-up references for other player-related components
        playerInputs = new PlayerInput[2];
        playerGameObjects = new GameObject[2];
        playerInputScripts = new PlayerInputScript[2];
        playerAnimationControllers = new PlayerAnimationController[2];
        //playerInputs[0] = PlayerInput.Instantiate(prefabPlayerIna, 0, controlScheme: controlSchemeP1, pairWithDevice: Gamepad.current);
        playerInputs[0] = PlayerInput.Instantiate(prefabPlayerIna, 0, controlScheme: controlSchemeP1, pairWithDevice: Keyboard.current);
        playerInputs[1] = PlayerInput.Instantiate(prefabPlayerIna, 1, controlScheme: controlSchemeP2, pairWithDevice: Keyboard.current);
        playerGameObjects[0] = playerInputs[0].gameObject;
        playerGameObjects[1] = playerInputs[1].gameObject;
        playerInputScripts[0] = playerGameObjects[0].GetComponent<PlayerInputScript>();
        playerInputScripts[1] = playerGameObjects[1].GetComponent<PlayerInputScript>();
        playerAnimationControllers[0] = playerGameObjects[0].GetComponent<PlayerAnimationController>();
        playerAnimationControllers[1] = playerGameObjects[1].GetComponent<PlayerAnimationController>();

        // UI-related set-up and such (not relating directly to game state)
        pauseHeldActivationTimerMax = playerInputScripts[0].p1PauseAction.interactions.Length;

        // || Game State set-up || //
        // THIS FIGHTING GAME IS A TWO PLAYER GAME (FOR NOW)
        game = new HfGame(2);
        boundsGameObject.transform.position = new Vector3(0f, 0f, 50f);
        boundsGameObject.transform.localScale = new Vector3(
            HfGame.bounds.width / pixelsInWorldUnit,
            HfGame.bounds.height / pixelsInWorldUnit,
            1f);
        if(playerAnimationControllers[0] != null)
        {
            playerAnimationControllers[0].SetPlayer(ref game.players[0]);
        }
        if(playerAnimationControllers[1] != null)
        {
            playerAnimationControllers[1].SetPlayer(ref game.players[1]);
        }

        LoadFrameDataForPlayer(0);
        LoadFrameDataForPlayer(1);
    }

    void FixedUpdate()
    {
        switch (game.currentBattleProgress)
        {
            case CurrentBattleProgress.WAITING_FOR_FIRST_ROUND:
                Debug.Log("Waiting for round to start...");
                break;

            case CurrentBattleProgress.ROUND_COUNTDOWN:
                Debug.Log("Countdown.");
                Debug.Log(3);
                Debug.Log(2);
                Debug.Log(1);
                Debug.Log("GO!");
                //game.currentBattleProgress = CurrentBattleProgress.ROUND_IN_PROGRESS;
                break;

            case CurrentBattleProgress.ROUND_IN_PROGRESS:
                if (!isGamePaused)
                {
                    long[] inputs = new long[game.players.Length];
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        inputs[i] = ReadGameInputs(i);
                    }
                    game.AdvanceFrame(inputs);
                }
                break;

            case CurrentBattleProgress.ROUND_OVER:
                Debug.Log("Round over.");
                break;

            case CurrentBattleProgress.GAME_OVER:
                Debug.Log("Game over.");
                break;
            
            default:
                Debug.Log("Unexpected game battle progression state.");
                break;
        }
    }

    void Update()
    {
        if (game.currentBattleProgress == CurrentBattleProgress.WAITING_FOR_FIRST_ROUND)
        {
            battleUIScript.ShowWaitingScreen();
            roundStartCountdownEnded = false;

            // If a button we care about is pressed, start the countdown for the first/next round.
            if (playerInputScripts[0].p1AttackAValue
                || playerInputScripts[0].p1AttackBValue
                || playerInputScripts[0].p1AttackCValue
                )
            {
                game.currentBattleProgress = CurrentBattleProgress.ROUND_COUNTDOWN;
            }
        }

        if (game.currentBattleProgress == CurrentBattleProgress.ROUND_COUNTDOWN)
        {
            roundStartTimer -= Time.deltaTime;
            if (roundStartTimer <= 0)
            {
                game.currentBattleProgress = CurrentBattleProgress.ROUND_IN_PROGRESS;
                roundStartCountdownEnded = true;
            }

            battleUIScript.ShowCountdown(roundStartTimer);
        }

        if (game.currentBattleProgress == CurrentBattleProgress.ROUND_OVER
            || game.currentBattleProgress == CurrentBattleProgress.GAME_OVER)
        {
            roundStartCountdownEnded = false;
        }

        //// Update visuals to match current game state
        // (nvm lol this happens elsewhere)
        PauseGameCheck();

        if (game.currentBattleProgress == CurrentBattleProgress.GAME_OVER && !matchEndScreenUp)
        {
            winnerPlayerIndex = game.winnerPlayerIndex;
            if (winnerPlayerIndex >= 0) // If there was a winner
            {
                winnerCharacterName = game.players[winnerPlayerIndex].characterFullName;
            }
            else // If there was a tie
            {
                // For now, we don't need to do anything else in this case.
            }
            battleUIScript.ShowMatchEndScreen(winnerPlayerIndex, winnerCharacterName);
            matchEndScreenUp = true;
        }
    }

    long ReadGameInputs(int playerID)
    {
        long input = 0;

        if (playerInputScripts[playerID].p1UpValue)
        {
            input |= INPUT_UP;
        }
        if (playerInputScripts[playerID].p1LeftValue)
        {
            input |= INPUT_LEFT;
        }
        if (playerInputScripts[playerID].p1DownValue)
        {
            input |= INPUT_DOWN;
        }
        if (playerInputScripts[playerID].p1RightValue)
        {
            input |= INPUT_RIGHT;
        }
        if (playerInputScripts[playerID].p1AttackAValue)
        {
            input |= INPUT_ATTACK_A;
        }
        if (playerInputScripts[playerID].p1AttackBValue)
        {
            input |= INPUT_ATTACK_B;
        }
        if (playerInputScripts[playerID].p1AttackCValue)
        {
            input |= INPUT_ATTACK_C;
        }
        if (playerInputScripts[playerID].p1AssistDValue)
        {
            input |= INPUT_DEFEND_D;
        }

        return input;
    }

    void PauseGameCheck()
    {
        // Same function is used for both pausing and unpausing the game.
        // (subject to change if it proves necessary to separate them)
        battleUIScript.UpdatePauseUI(playerInputScripts[0].p1PauseAction, isGamePaused);
        if (!isGamePaused && game.currentBattleProgress == CurrentBattleProgress.ROUND_IN_PROGRESS)
        {
            // Pause
            if (playerInputScripts[0].p1PauseWasPressedThisFrame)
            {
                // If pause button was just pressed this frame, activate some UI
                Debug.Log("show pause timer");
            }
            else if(playerInputScripts[0].p1PauseWasReleasedThisFrame)
            {
                // If pause button UI was shown, remove it because we're not pausing now after all!
                Debug.Log("hide pause timer");
            }
            else if(playerInputScripts[0].p1PauseFullyHeldForDuration)
            {
                // If pause button was held for long enough, actually pause the game
                Debug.Log("GAME PAUSED");
                isGamePaused = true;
                //battleUIScript.Pause();
            }
        }
        else
        {
            // Unpause
            if (playerInputScripts[0].p1PauseWasPressedThisFrame)
            {
                // Unlike pausing, unpausing occurs instantly upon pressing the pause button.
                Debug.Log("GAME UNPAUSED");
                isGamePaused = false;
                //battleUIScript.Unpause();
            }
        }
    }

    public void UnpauseGameFromUI()
    {
        isGamePaused = false;
        battleUIScript.Unpause();
    }

    public void RematchFromUI()
    {
        Debug.Log("Rematch!");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void CharSelectFromUI() => Debug.Log("Return to Character Select!");

    public void MainMenuFromUI() => Debug.Log("Return to Main Menu!");

    public void LoadFrameDataForPlayer(int playerNumber)
    {
        string characterName = game.players[playerNumber].characterName.ToUpper();
        //string characterName = "INA";

        //Debug.Log(CharacterGameDataIna.GetType());
        //Debug.Log(CharacterGameDataIna.GetType().GetFields()[0].Name);
        //Debug.Log(CharacterGameDataIna.GetType().GetFields()[1].Name);
        //Debug.Log(CharacterGameDataIna.GetType().GetFields()[2].Name);

        //Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[0].GetValue(CharacterGameDataIna));
        //Debug.Log((TextAsset)CharacterGameDataIna.GetType().GetFields()[8].GetValue(CharacterGameDataIna));
        //Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[8].GetValue(CharacterGameDataIna).ToString());
        //Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[0].GetValue(CharacterGameDataIna).ToString());
        //Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[1].GetValue(CharacterGameDataIna));
        //Debug.Log((string)CharacterGameDataIna.GetType().GetFields()[2].GetValue(CharacterGameDataIna));

        List<AnimationStateData> allCharacterFrameData = new List<AnimationStateData>();

        // JSON deserialization preparation
        string frameDataString;
        var serializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        foreach (FieldInfo fieldInfo in CharacterGameDataIna.GetType().GetFields())
        {
            //Debug.Log(fieldInfo.FieldType.FullName);
            //Debug.Log(fieldInfo.FieldType.FullName == "UnityEngine.TextAsset");
            if (fieldInfo.FieldType.FullName == "UnityEngine.TextAsset")
            {
                //Debug.Log(fieldInfo.Name);
                frameDataString = fieldInfo.GetValue(CharacterGameDataIna).ToString();
                var loadedFrameData = JsonConvert.DeserializeObject<AnimationStateData>(frameDataString, serializerSettings);
                allCharacterFrameData.Add(loadedFrameData);
            }

        }

        Debug.Log("Loading imported data into character classes");
        foreach (AnimationStateData animationFrameData in allCharacterFrameData)
        {
            string animationName = animationFrameData.animationName;
            //Debug.Log(animationFrameData);
            //Debug.Log(animationName);

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
