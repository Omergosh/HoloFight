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
        Debug.Log("1 Process menu input");
        if (menuInput.menuInputValues[0].pMoveInMenuValue.x < 0)
        {
            Debug.Log("1 Move left");
            p1CurrentlySelectedOptionIndex = GetIndex(p1CurrentlySelectedMenuOption.optionOneLeft);
        }
        else if (menuInput.menuInputValues[0].pMoveInMenuValue.x > 0)
        {
            Debug.Log("1 Move right");
            p1CurrentlySelectedOptionIndex = GetIndex(p1CurrentlySelectedMenuOption.optionOneRight);
        }
        if (menuInput.menuInputValues[0].pConfirmValue)
        {
            Debug.Log("1 Confirm");
            //p1CurrentlySelectedMenuOption.menuOptionSelectedEvent.Invoke();
        }
        if (menuInput.menuInputValues[0].pBackValue)
        {
            Debug.Log("1 Back out of menu.");
            //backOutOfMenuListEvent.Invoke();
        }

        if (menuInput.menuInputValues[1].pMoveInMenuValue.x < 0)
        {
            Debug.Log("2 Move left");
            p2CurrentlySelectedOptionIndex = GetIndex(p2CurrentlySelectedMenuOption.optionOneLeft);
        }
        else if (menuInput.menuInputValues[1].pMoveInMenuValue.x > 0)
        {
            Debug.Log("2 Move right");
            p2CurrentlySelectedOptionIndex = GetIndex(p2CurrentlySelectedMenuOption.optionOneRight);
        }
        if (menuInput.menuInputValues[1].pConfirmValue)
        {
            Debug.Log("1 Confirm");
            //pCurrentlySelectedMenuOption.menuOptionSelectedEvent.Invoke();
        }
        if (menuInput.menuInputValues[1].pBackValue)
        {
            Debug.Log("1 Back out of menu.");
            //backOutOfMenuListEvent.Invoke();
        }
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
