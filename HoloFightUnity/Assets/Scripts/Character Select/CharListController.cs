using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharListController : MonoBehaviour
{
    UnityEvent submitEvent;
    UnityEvent cancelEvent;
    UnityEvent moveEvent;
    UnityEvent moveUpEvent;
    UnityEvent moveDownEvent;
    UnityEvent moveLeftEvent;
    UnityEvent moveRightEvent;

    public List<MenuOption> menuOptions;
    public int p1CurrentlySelectedOptionIndex = 0;
    public int p2CurrentlySelectedOptionIndex = 0;
    public MenuCursor p1menuCursor;
    public MenuCursor p2menuCursor;

    public MenuOption p1CurrentlySelectedMenuOption => menuOptions[p1CurrentlySelectedOptionIndex];
    public MenuOption p2CurrentlySelectedMenuOption => menuOptions[p2CurrentlySelectedOptionIndex];

    public UnityEvent backOutOfMenuListEvent;

    internal void ProcessInput(PlayerMenuInput menuInput)
    {
        PlayerConfiguration p1Config = PlayerConfigurationManager.instance.GetPlayerByTeam(0);
        PlayerConfiguration p2Config = PlayerConfigurationManager.instance.GetPlayerByTeam(1);

        Debug.Log("1 Process menu input");
        if (!p1Config.IsReady)
        {
            if (menuInput.menuInputValues[0].pMoveInMenuValue.x < 0)
            {
                Debug.Log("1 Move left");
                GameManager.instance.audioManager.PlaySFX("menuMove");
                p1CurrentlySelectedOptionIndex = GetIndex(p1CurrentlySelectedMenuOption.optionOneLeft);
            }
            else if (menuInput.menuInputValues[0].pMoveInMenuValue.x > 0)
            {
                Debug.Log("1 Move right");
                GameManager.instance.audioManager.PlaySFX("menuMove");
                p1CurrentlySelectedOptionIndex = GetIndex(p1CurrentlySelectedMenuOption.optionOneRight);
            }
            if (menuInput.menuInputValues[0].pConfirmValue)
            {
                Debug.Log("1 Confirm");
                if (p1CurrentlySelectedMenuOption.optionName == "INA")
                {
                    p1Config.IsReady = true;
                    // trigger event for good button noise
                    GameManager.instance.audioManager.PlaySFX("charSelectReady");
                }
                else
                {
                    // trigger event for bad button noise
                    GameManager.instance.audioManager.PlaySFX("menuInvalid");
                }
            }
            if (menuInput.menuInputValues[0].pBackValue)
            {
                Debug.Log("1 Back out of menu.");
                GameManager.instance.audioManager.PlaySFX("menuBack");
                GameManager.instance.CharSelectExit();

                // These next lines are to ensure there aren't any unforeseen issues from
                // lingering variable assignments caused by this Chararacter Select Scene.
                p1Config.IsReady = false;
                p2Config.IsReady = false;
                return;
            }
        }
        else
        {
            if (menuInput.menuInputValues[0].pBackValue)
            {
                Debug.Log("1 Unready.");
                GameManager.instance.audioManager.PlaySFX("charSelectUnready");
                p1Config.IsReady = false;
            }
        }

        // Player 2
        if (!p2Config.IsReady)
        {
            if (menuInput.menuInputValues[1].pMoveInMenuValue.x < 0)
            {
                Debug.Log("2 Move left");
                GameManager.instance.audioManager.PlaySFX("menuMove");
                p2CurrentlySelectedOptionIndex = GetIndex(p2CurrentlySelectedMenuOption.optionOneLeft);
            }
            else if (menuInput.menuInputValues[1].pMoveInMenuValue.x > 0)
            {
                Debug.Log("2 Move right");
                GameManager.instance.audioManager.PlaySFX("menuMove");
                p2CurrentlySelectedOptionIndex = GetIndex(p2CurrentlySelectedMenuOption.optionOneRight);
            }
            if (menuInput.menuInputValues[1].pConfirmValue)
            {
                Debug.Log("2 Confirm");
                if (p2CurrentlySelectedMenuOption.optionName == "INA")
                {
                    p2Config.IsReady = true;
                    // trigger event for good button noise
                    GameManager.instance.audioManager.PlaySFX("charSelectReady");
                }
                else
                {
                    // trigger event for bad button noise
                    GameManager.instance.audioManager.PlaySFX("menuInvalid");
                }
            }
            if (menuInput.menuInputValues[1].pBackValue)
            {
                Debug.Log("2 Back out of menu.");
                GameManager.instance.audioManager.PlaySFX("menuBack");
                GameManager.instance.CharSelectExit();

                // These next lines are to ensure there aren't any unforeseen issues from
                // lingering variable assignments caused by this Chararacter Select Scene.
                p1Config.IsReady = false;
                p2Config.IsReady = false;
                return;
            }
        }
        else
        {
            if (menuInput.menuInputValues[1].pBackValue)
            {
                Debug.Log("2 Unready.");
                GameManager.instance.audioManager.PlaySFX("charSelectUnready");
                p2Config.IsReady = false;
            }
        }

        // Update visuals
        p1menuCursor.SetCursorPosition(p1CurrentlySelectedMenuOption);
        p2menuCursor.SetCursorPosition(p2CurrentlySelectedMenuOption);
        p1menuCursor.SetColor(p1Config.IsReady ? Color.yellow : Color.white);
        p2menuCursor.SetColor(p2Config.IsReady ? Color.yellow : Color.white);
    }

    int GetIndex(MenuOption menuOption)
    {
        return menuOptions.IndexOf(menuOption);
    }

    void SelectCharacter(int playerIndex, int optionIndex)
    {
        if (playerIndex == 0)
        {
            p1CurrentlySelectedOptionIndex = optionIndex;
        }
        else if (playerIndex == 1)
        {
            p2CurrentlySelectedOptionIndex = optionIndex;
        }
        else
        {
            Debug.Log("Invalid index in SelectCharacter function of CharListController class");
        }
    }

    void ReadyUp(int playerIndex)
    {

    }
}
