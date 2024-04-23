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

    internal void ProcessInput(PlayerMenuInput menuInput, int menuPlayerIndex = 0)
    {
        //Debug.Log("Process menu input");
        if (menuInput.menuInputValues[menuPlayerIndex].pMoveInMenuValue.y > 0)
        {
            Debug.Log("Move up");
            GameManager.instance.audioManager.PlaySFX("menuMove");
            currentlySelectedOptionIndex = GetIndex(currentlySelectedMenuOption.optionOneUp);
        }
        else if (menuInput.menuInputValues[menuPlayerIndex].pMoveInMenuValue.y < 0)
        {
            Debug.Log("Move down");
            GameManager.instance.audioManager.PlaySFX("menuMove");
            currentlySelectedOptionIndex = GetIndex(currentlySelectedMenuOption.optionOneDown);
        }
        if (menuInput.menuInputValues[menuPlayerIndex].pConfirmValue)
        {
            Debug.Log("Confirm");
            GameManager.instance.audioManager.PlaySFX("menuConfirm");
            currentlySelectedMenuOption.menuOptionSelectedEvent.Invoke();
        }
        if (menuInput.menuInputValues[menuPlayerIndex].pBackValue)
        {
            Debug.Log("Back out of menu.");
            GameManager.instance.audioManager.PlaySFX("menuBack");
            backOutOfMenuListEvent.Invoke();
        }
    }

    int GetIndex(MenuOption menuOption)
    {
        return menuOptions.IndexOf(menuOption);
    }
}
