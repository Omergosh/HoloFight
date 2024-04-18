using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBattleInput : MonoBehaviour
{
    public PlayerInput playerInput;

    InputAction p1UpAction;
    InputAction p1LeftAction;
    InputAction p1DownAction;
    InputAction p1RightAction;
    InputAction p1AttackAAction;
    InputAction p1AttackBAction;
    InputAction p1AttackCAction;
    InputAction p1AssistDAction;

    public bool p1UpValue = false;
    public bool p1LeftValue = false;
    public bool p1DownValue = false;
    public bool p1RightValue = false;
    public bool p1AttackAValue = false;
    public bool p1AttackBValue = false;
    public bool p1AttackCValue = false;
    public bool p1AssistDValue = false;

    public InputAction p1PauseAction;
    public bool p1PauseWasPressedThisFrame = false;
    public bool p1PauseWasReleasedThisFrame = false;
    public bool p1PauseFullyHeldForDuration = false;

    public InputAction p1EscapeOrUnpauseAction;
    public bool p1EscapeOrUnpausePressed = false;

    // If the 'A' button was just used to unpause, don't register its inputs until after it's released.
    public bool p1PausePressLockoutAttackA = false;
    
    void Start()
    {   
        p1UpAction = playerInput.actions["Up"];
        p1LeftAction = playerInput.actions["Left"];
        p1DownAction = playerInput.actions["Down"];
        p1RightAction = playerInput.actions["Right"];
        p1AttackAAction = playerInput.actions["AttackA"];
        p1AttackBAction = playerInput.actions["AttackB"];
        p1AttackCAction = playerInput.actions["AttackC"];
        p1AssistDAction = playerInput.actions["AssistD"];
        p1PauseAction = playerInput.actions["Pause"];
        p1EscapeOrUnpauseAction = playerInput.actions["Escape"];
    }

    // Update is called once per frame
    void Update()
    {
        p1UpValue = (p1UpAction.ReadValue<float>() > 0);
        p1LeftValue = (p1LeftAction.ReadValue<float>() > 0);
        p1DownValue = (p1DownAction.ReadValue<float>() > 0);
        p1RightValue = (p1RightAction.ReadValue<float>() > 0);
        p1AttackAValue = (p1AttackAAction.ReadValue<float>() > 0);
        p1AttackBValue = (p1AttackBAction.ReadValue<float>() > 0);
        p1AttackCValue = (p1AttackCAction.ReadValue<float>() > 0);
        p1AssistDValue = (p1AssistDAction.ReadValue<float>() > 0);

        p1PauseWasPressedThisFrame = p1PauseAction.WasPressedThisFrame();
        p1PauseWasReleasedThisFrame = p1PauseAction.WasReleasedThisFrame();
        p1PauseFullyHeldForDuration = (p1PauseAction.triggered);
        //Debug.Log("Pause started: " + p1PauseAction.WasPressedThisFrame());
        //Debug.Log("Pause started: " + p1PauseAction.WasReleasedThisFrame());
        //Debug.Log("Pause trigger: " + p1PauseAction.triggered.ToString());
        //Debug.Log("Pause trigger: " + p1PauseValue.ToString());

        p1EscapeOrUnpausePressed = p1EscapeOrUnpauseAction.WasPressedThisFrame();

        if (!p1AttackAValue) { p1PausePressLockoutAttackA = false; }
    }

    public bool AnyAttackButtonPressed()
    {
        bool attackPressed = false;
        if (p1AttackAValue || p1AttackBValue || p1AttackCValue)
        {
            attackPressed = true;
        }
        return attackPressed;
    }
}
