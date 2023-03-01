using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    public List<PlayerConfiguration> playerConfigs;

    public GameObject playerConfigPrefab;

    [SerializeField]
    private int maxPlayers = 2;

    public bool onTitleScreen = false;
    public bool primaryPlayerChosen = false;
    public bool configuringPlayerDevices = false;

    private float ignoreJoinTime = 1.5f;
    private bool joinEnabled = false;

    // Special case for allowing a second player to join on keyboard
    public bool firstKeyboardPlayerJoined = false;
    public bool secondKeyboardPlayerJoined = false;

    public static PlayerConfigurationManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Error, SINGLETON - Trying to create another instance of a singleton!");
            Destroy(this.gameObject); // Someday, this will cause a bug, I'm sure.
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
            playerConfigs = new List<PlayerConfiguration>();
            ignoreJoinTime = Time.time + ignoreJoinTime;
        }
    }

    private void Update()
    {
        if (onTitleScreen && !primaryPlayerChosen)
        {
            TitleScreenUpdate();
        }
        else if (configuringPlayerDevices)
        {
            PlayerJoinCheckUpdate();
        }
    }

    public void InitializeOnTitleScreen()
    {
        //GetComponent<PlayerInputManager>().EnableJoining();
        // Reset all PlayerConfigs
        // Remove all PlayerInputPrefabs under the Game Manager
        // (necessary for if a player backs out from the main menu into the title screen)
        foreach (PlayerConfiguration playerConfig in playerConfigs)
        {
            //Destroy(playerConfig.Input.gameObject, 0.01f);
            Destroy(playerConfig.Input.gameObject);
        }
        playerConfigs.Clear();
        firstKeyboardPlayerJoined = false;
        secondKeyboardPlayerJoined = false;
        primaryPlayerChosen = false;
        onTitleScreen = true;
    }

    void TitleScreenUpdate()
    {
        bool quitThisFrame = false;

        if (!firstKeyboardPlayerJoined && playerConfigs.Count < maxPlayers && !primaryPlayerChosen)
        {
            if (Keyboard.current.zKey.isPressed)
            {
                PlayerInput newPI = PlayerInput.Instantiate(playerConfigPrefab, 0, controlScheme: "KeyboardP1Scheme", pairWithDevice: Keyboard.current);
                //GetComponent<PlayerInputManager>().JoinPlayer(controlScheme: "KeyboardP1Scheme", pairWithDevice: Keyboard.current);
                firstKeyboardPlayerJoined = true;
                HandlePlayerJoin(newPI);
                primaryPlayerChosen = true;
            }
        }
        if (!secondKeyboardPlayerJoined && playerConfigs.Count < maxPlayers && !primaryPlayerChosen)
        {
            if (Keyboard.current.uKey.isPressed)
            {
                PlayerInput newPI = PlayerInput.Instantiate(playerConfigPrefab, 0, controlScheme: "KeyboardP2Scheme", pairWithDevice: Keyboard.current);
                //GetComponent<PlayerInputManager>().JoinPlayer(controlScheme: "KeyboardP2Scheme", pairWithDevice: Keyboard.current);
                secondKeyboardPlayerJoined = true;
                HandlePlayerJoin(newPI);
                primaryPlayerChosen = true;
            }
        }
        if (Keyboard.current.escapeKey.isPressed)
        {
            quitThisFrame = true;
        }

        if (playerConfigs.Count < maxPlayers && !primaryPlayerChosen)
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                if(gamepad.selectButton.wasPressedThisFrame || gamepad.bButton.wasPressedThisFrame)
                {
                    quitThisFrame = true;
                }
                else if (gamepad.startButton.wasPressedThisFrame && !primaryPlayerChosen)
                {
                    PlayerInput newPI = PlayerInput.Instantiate(playerConfigPrefab, 0, controlScheme: "GamepadScheme", pairWithDevice: gamepad);
                    //GetComponent<PlayerInputManager>().JoinPlayer(controlScheme: "GamepadScheme", pairWithDevice: gamepad);
                    HandlePlayerJoin(newPI);
                    primaryPlayerChosen = true;
                }
            }
        }

        // If a primary player was decided (because they pressed the correct input), move on.
        if (quitThisFrame)
        {
            GameManager.instance.TitleScreenQuit();
        }
        else if(primaryPlayerChosen)
        {
            GameManager.instance.TitleScreenAdvance();
        }

    }

    public void RemoveNonPrimaryPlayers()
    {
        if (playerConfigs.Count > 1)
        {
            for (int i = 1; i < playerConfigs.Count; i++)
            {
                Destroy(playerConfigs[i].Input.gameObject, 0.01f);
            }
            playerConfigs.RemoveRange(1, playerConfigs.Count - 1);
            Debug.Log("Removed non-primary players.");

            // Reset control-scheme-specific variables on this manager object.
            if (playerConfigs[0].Input.currentControlScheme != "KeyboardP1Scheme") { firstKeyboardPlayerJoined = false; }
            if (playerConfigs[0].Input.currentControlScheme != "KeyboardP2Scheme") { secondKeyboardPlayerJoined = false; }
        }
    }

    static void DebugRemoveNonPrimaryPlayers()
    {
        instance.RemoveNonPrimaryPlayers();
    }

    public void StartConfiguringPlayerDevices()
    {
        configuringPlayerDevices = true;
        ignoreJoinTime = Time.time + ignoreJoinTime;
    }

    public void StopConfiguringPlayerDevices()
    {
        configuringPlayerDevices = false;
    }

    void PlayerJoinCheckUpdate()
    {
        if (Time.time > ignoreJoinTime)
        {
            joinEnabled = true;
            //GetComponent<PlayerInputManager>().EnableJoining();
        }

        if (joinEnabled)
        {
            if (!firstKeyboardPlayerJoined && playerConfigs.Count < maxPlayers)
            {
                if (Keyboard.current.zKey.isPressed)
                {
                    PlayerInput newPI = PlayerInput.Instantiate(playerConfigPrefab,/* 0,*/ controlScheme: "KeyboardP1Scheme", pairWithDevice: Keyboard.current);
                    //GetComponent<PlayerInputManager>().JoinPlayer(controlScheme: "KeyboardP1Scheme", pairWithDevice: Keyboard.current);
                    firstKeyboardPlayerJoined = true;
                    HandlePlayerJoin(newPI);
                }
            }
            if (!secondKeyboardPlayerJoined && playerConfigs.Count < maxPlayers)
            {
                if (Keyboard.current.uKey.isPressed)
                {
                    PlayerInput newPI = GetComponent<PlayerInputManager>().JoinPlayer(controlScheme: "KeyboardP2Scheme", pairWithDevice: Keyboard.current);
                    secondKeyboardPlayerJoined = true;
                    HandlePlayerJoin(newPI);
                }
            }
            if(playerConfigs.Count < maxPlayers)
            {
                foreach(Gamepad gamepad in Gamepad.all)
                {
                    if (gamepad.startButton.wasPressedThisFrame)
                    {
                        // Check if gamepad is already paired to an existing player
                        //Debug.Log(playerConfigs[0].Input);
                        //Debug.Log(InputUser.all);
                        //Debug.Log(InputUser.all[0]);
                        //Debug.Log(InputUser.all[0].pairedDevices.Contains(gamepad));
                        Debug.Log(InputUser.all.Any(u => u.pairedDevices.Contains(gamepad)));
                        bool deviceAlreadyPaired = false;
                        deviceAlreadyPaired = InputUser.all.Any(u => u.pairedDevices.Contains(gamepad));

                        // If not, connect this gamepad to a new joining player
                        if (!deviceAlreadyPaired)
                        {
                            PlayerInput newPI = GetComponent<PlayerInputManager>().JoinPlayer(controlScheme: "GamepadScheme", pairWithDevice: gamepad);
                            HandlePlayerJoin(newPI);
                        }
                    }
                }
            }
        }
    }

    public bool CheckAllPlayersJoined()
    {
        if (playerConfigs.Count < maxPlayers) { return false; }

        return true;
    }

    public bool CheckAllPlayersJoinedAndReady()
    {
        if (playerConfigs.Count < maxPlayers) { return false; }

        foreach (PlayerConfiguration playerConfig in playerConfigs)
        {
            if (!playerConfig.IsReady) { return false; }
        }

        return true;
    }

    public PlayerConfiguration GetPlayerByTeam(int teamId)
    {
        PlayerConfiguration returnPlayerConfig;
        returnPlayerConfig = playerConfigs.Find(p => p.TeamIndex == teamId);
        return returnPlayerConfig;
    }

    public void SetPlayerCharacter(int index, CharacterChoice character)
    {
        playerConfigs[index].PlayerCharacter = character;
    }

    public void ReadyPlayer(int index)
    {
        playerConfigs[index].IsReady = true;
        if (playerConfigs.Count == maxPlayers && playerConfigs.All(p => p.IsReady == true))
        {
            Debug.Log("Everyone is ready");
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player joined: " + pi.playerIndex.ToString());
        GameManager.instance.audioManager.PlaySFX("deviceSetupJoin");

        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            playerConfigs.Add(new PlayerConfiguration(pi));
            Debug.Log(playerConfigs.Count);
        }
    }

    public void EnableMenuInputs()
    {
        //GetPlayerByTeam(0).Input.SwitchCurrentActionMap("MenuActions");
        foreach (PlayerConfiguration pConfig in PlayerConfigurationManager.instance.playerConfigs)
        {
            pConfig.Input.SwitchCurrentActionMap("MenuActions");
        }
    }

    public void DisableMenuInputs()
    {
        //GetPlayerByTeam(0).Input.SwitchCurrentActionMap("FightControls");
        foreach (PlayerConfiguration pConfig in PlayerConfigurationManager.instance.playerConfigs)
        {
            pConfig.Input.SwitchCurrentActionMap("FightControls");
        }
    }
}


public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        TeamIndex = PlayerIndex;
        Input = pi;

        // Default harcoded values for initial incomplete implementation
        //PlayerCharacter = CharacterChoice.INA;
    }

    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public int TeamIndex { get; set; }
    public bool IsReady { get; set; }

    public CharacterChoice PlayerCharacter { get; set; }
}

public enum CharacterChoice { INA, AME, GURA, CALLI, KIARA}