using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuOption : MonoBehaviour
{
    public MenuOption optionOneUp;
    public MenuOption optionOneDown;
    public MenuOption optionOneLeft;
    public MenuOption optionOneRight;

    GameManager gmReference;
    public UnityEvent menuOptionSelectedEvent;

    public int optionIndex;
    public string optionName;

    void Start()
    {
        gmReference = GameManager.instance;
    }
}
