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
    }

    // Update is called once per frame
    void Update()
    {
        switch (charSelectState)
        {
            case CharSelectScreenState.SELECTING_CHARS:
                if (PlayerConfigurationManager.instance.CheckAllPlayersJoinedAndReady())
                {
                    // Characcter choices already recorded(?) - just go to the fight.
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
