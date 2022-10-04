using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuListController : MonoBehaviour
{
    // Class through which to control ONE list of options per instance of this script.

    UnityEvent submitEvent;
    UnityEvent cancelEvent;
    UnityEvent moveEvent;
    UnityEvent moveUpEvent;
    UnityEvent moveDownEvent;
    UnityEvent moveLeftEvent;
    UnityEvent moveRightEvent;

    public List<MenuOption> menuOptions;
    public int currentlySelectedOptionIndex = 0;
    public MenuCursor menuCursor;

    public MenuOption currentlySelectedMenuOption => menuOptions[currentlySelectedOptionIndex];

    public UnityEvent backOutOfMenuListEvent;

    internal void ProcessInput(PlayerMenuInput menuInput)
    {
        Debug.Log("Process menu input");
        if (menuInput.menuInputValues[0].pMoveInMenuValue.y > 0)
        {
            Debug.Log("Move up");
            currentlySelectedOptionIndex = GetIndex(currentlySelectedMenuOption.optionOneUp);
        }
        else if (menuInput.menuInputValues[0].pMoveInMenuValue.y < 0)
        {
            Debug.Log("Move down");
            currentlySelectedOptionIndex = GetIndex(currentlySelectedMenuOption.optionOneDown);
        }
        if (menuInput.menuInputValues[0].pConfirmValue)
        {
            Debug.Log("Confirm");
            currentlySelectedMenuOption.menuOptionSelectedEvent.Invoke();
        }
        if (menuInput.menuInputValues[0].pBackValue)
        {
            Debug.Log("Back out of menu.");
            backOutOfMenuListEvent.Invoke();
        }
    }

    int GetIndex(MenuOption menuOption)
    {
        return menuOptions.IndexOf(menuOption);
    }
}
