using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharSelectScreenState
{
    SELECTING_CHARS,
    INACTIVE
}

public class CharSelectManager : MonoBehaviour
{
    public CharSelectScreenState charSelectState = CharSelectScreenState.SELECTING_CHARS;

    public CharacterChoice p1Choice;
    public CharacterChoice p2Choice;

    public CharListController charListController;
    public PlayerMenuInput menuInput;

    // Start is called before the first frame update
    void Start()
    {
        menuInput.InitializeWithAllPlayers();
        //foreach (PlayerConfiguration pConfig in PlayerConfigurationManager.instance.playerConfigs)
        //{
        //    pConfig.Input.SwitchCurrentActionMap("MenuActions");
        //}
        PlayerConfigurationManager.instance.EnableMenuInputs();

        // Initialize player values.
        // (necessary for exiting out from FightScene to Character Select)
        foreach (PlayerConfiguration pConfig in PlayerConfigurationManager.instance.playerConfigs)
        {
            pConfig.IsReady = false;
            pConfig.PlayerCharacter = CharacterChoice.INA;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (charSelectState)
        {
            case CharSelectScreenState.SELECTING_CHARS:
                if (PlayerConfigurationManager.instance.CheckAllPlayersJoinedAndReady())
                {
                    // Character choices already recorded(?) - just go to the fight.
                    GameManager.instance.CharSelectToVersus();
                    charSelectState = CharSelectScreenState.INACTIVE;
                }
                else
                {
                    // Actually run logic here.
                    charListController.ProcessInput(menuInput);
                }
                break;
        }
    }
}
