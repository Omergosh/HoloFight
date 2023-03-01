using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MainMenuState
{
    AT_ROOT,
    VERSUS_CONTROLLER_SETUP,
    AT_SETTINGS,
    AT_CREDITS,
    INACTIVE // no longer responding to input because of an ongoing scene transition, etc.
}

public class MainMenuManager : MonoBehaviour
{
    MenuListController titleMenu;
    MenuListController mainMenu;
    MenuListController settingsMenu;

    public MenuListController currentlyActiveMenu;
    public MenuCursor menuCursor;
    public DeviceSelectController deviceSelectController;

    public PlayerInput primaryPlayerInput;
    public PlayerMenuInput menuInput;

    public MainMenuState mainMenuState = MainMenuState.AT_ROOT;

    // Start is called before the first frame update
    void Start()
    {
        foreach(PlayerConfiguration playerConfig in PlayerConfigurationManager.instance.playerConfigs)
        {
            //playerConfig.Input.currentActionMap = playerConfig.Input.actions.FindActionMap("MenuActions");
            playerConfig.Input.SwitchCurrentActionMap("MenuActions");
            //playerConfig.Input.SwitchCurrentActionMap("MenuControls");
        }
        PlayerConfigurationManager.instance.RemoveNonPrimaryPlayers();
        primaryPlayerInput = PlayerConfigurationManager.instance.playerConfigs[0].Input;
        menuInput.InitializeWithPrimaryPlayer(primaryPlayerInput);
    }

    // Update is called once per frame
    void Update()
    {
        switch (mainMenuState)
        {
            case MainMenuState.INACTIVE:
                // Do nothing, because you should not be responding to player input right now.
                // Wait for an ongoing process to finish up.
                break;

            case MainMenuState.VERSUS_CONTROLLER_SETUP:
                deviceSelectController.ProcessInput(menuInput);
                if (deviceSelectController.CheckAllPlayersJoinedAndReady())
                {
                    // All player devices are selected and ready!
                    deviceSelectController.AssignPlayerTeamsToConfigs();
                    mainMenuState = MainMenuState.INACTIVE;
                    PlayerConfigurationManager.instance.StopConfiguringPlayerDevices();
                    GameManager.instance.MainMenuToVersusCharSelect();
                }
                break;

            default:
                currentlyActiveMenu.ProcessInput(menuInput);
                menuCursor.SetCursorPosition(currentlyActiveMenu.currentlySelectedMenuOption);
                break;
        }
    }

    #region Root
    public void ExitToTitleScreen()
    {
        GameManager.instance.audioManager.PlaySFX("menuBack");
        GameManager.instance.MainMenuExit();
        PlayerConfigurationManager.instance.onTitleScreen = true;
    }
    #endregion

    #region VersusControllerSetup
    public void StartVersusControllerSetup()
    {
        //GameManager.instance.audioManager.PlaySFX("deviceSetupJoin");
        mainMenuState = MainMenuState.VERSUS_CONTROLLER_SETUP;
        deviceSelectController.gameObject.SetActive(true);
        PlayerConfigurationManager.instance.StartConfiguringPlayerDevices();
    }
    public void CloseVersusControllerSetup()
    {
        //GameManager.instance.audioManager.PlaySFX("deviceSetupCancel");
        mainMenuState = MainMenuState.AT_ROOT;
        deviceSelectController.gameObject.SetActive(false);
        PlayerConfigurationManager.instance.StopConfiguringPlayerDevices();
        PlayerConfigurationManager.instance.RemoveNonPrimaryPlayers();
        menuInput.RemoveNonPrimaryPlayers();
    }
    #endregion

    #region Settings
    public void OpenButtonRebindMenu() { }
    public void CloseButtonRebindMenu() { }
    #endregion

    #region Credits
    public void OpenCreditsView() { }
    public void CloseCreditsView() { }
    #endregion

    public void InvalidOptionSelected()
    {
        // Play a sound effect to let the user know they've selected a very invalid option. Feel bad.
        GameManager.instance.audioManager.PlaySFX("menuInvalid");
        Debug.Log("Option has not been implemented yet.");
    }
}
