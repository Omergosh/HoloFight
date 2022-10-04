using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenuInputValues
{
    public InputAction pConfirmAction;
    public InputAction pBackAction;
    public InputAction pMoveInMenuAction;

    public bool pConfirmValue = false;
    public bool pBackValue = false;
    public Vector2 pMoveInMenuValue = new Vector2();
}

public class PlayerMenuInput : MonoBehaviour
{
    public List<PlayerInput> playerInputs = new List<PlayerInput>();

    public List<PlayerMenuInputValues> menuInputValues = new List<PlayerMenuInputValues>();

    void Start()
    {
        //playerInputs = new List<PlayerInput>();
        //AddPlayerMenuInput(0, PlayerConfigurationManager.instance.playerConfigs[0].Input);
    }

    public void InitializeWithPrimaryPlayer(PlayerInput primaryPlayerInput)
    {
        playerInputs = new List<PlayerInput>();
        AddPlayerMenuInput(0, primaryPlayerInput);
    }

    public void InitializeWithAllPlayers()
    {
        playerInputs = new List<PlayerInput>();
        foreach (PlayerConfiguration playerConfig in PlayerConfigurationManager.instance.playerConfigs)
        {
            AddPlayerMenuInput(playerConfig.TeamIndex, playerConfig.Input);
        }
    }

    void AddPlayerMenuInput(int index, PlayerInput pInput)
    {
        if (index + 1 > playerInputs.Count)
        {
            playerInputs.Add(pInput);
            menuInputValues.Add(new PlayerMenuInputValues());
        }

        Debug.Log(playerInputs.Count);
        Debug.Log(index);
        Debug.Log(pInput.playerIndex);
        Debug.Log(PlayerConfigurationManager.instance.playerConfigs.Count);
        playerInputs[index] = PlayerConfigurationManager.instance.playerConfigs[index].Input;
        menuInputValues[index].pConfirmAction = playerInputs[index].actions["Confirm"];
        menuInputValues[index].pBackAction = playerInputs[index].actions["Back"];
        menuInputValues[index].pMoveInMenuAction = playerInputs[index].actions["MoveInMenu"];
        menuInputValues[index].pConfirmAction.Enable();
        menuInputValues[index].pBackAction.Enable();
        menuInputValues[index].pMoveInMenuAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("confirm"
        //    + PlayerConfigurationManager.instance.playerConfigs[0].Input.actions["Confirm"].WasPressedThisFrame().ToString());
        //Debug.Log("confirm2"
        //    + playerInputs[0].actions["Confirm"].WasPressedThisFrame().ToString());
        //Debug.Log("confirm3"
        //    + menuInputValues[0].pConfirmAction.WasPressedThisFrame().ToString());

        if (playerInputs.Count < PlayerConfigurationManager.instance.playerConfigs.Count)
        {
            //foreach (PlayerConfiguration playerConfig in PlayerConfigurationManager.instance.playerConfigs)
            for (int i = playerInputs.Count; i < PlayerConfigurationManager.instance.playerConfigs.Count; i++)
            {
                AddPlayerMenuInput(i, PlayerConfigurationManager.instance.playerConfigs[i].Input);
            }
        }

        for (int i = 0; i < menuInputValues.Count; i++)
        {
            menuInputValues[i].pConfirmValue = menuInputValues[i].pConfirmAction.WasPressedThisFrame();
            menuInputValues[i].pBackValue = menuInputValues[i].pBackAction.WasPressedThisFrame();
            if (menuInputValues[i].pMoveInMenuAction.WasPressedThisFrame())
            {
                menuInputValues[i].pMoveInMenuValue = menuInputValues[i].pMoveInMenuAction.ReadValue<Vector2>();
            }
            else
            {
                menuInputValues[i].pMoveInMenuValue = new Vector2();
            }
        }
    }
}