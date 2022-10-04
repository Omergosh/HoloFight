using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceSelectController : MonoBehaviour
{
    List<PlayerInputDeviceDisplay> deviceDisplays = new List<PlayerInputDeviceDisplay>();
    [SerializeField]
    GameObject deviceDisplayContainer;

    [SerializeField]
    GameObject deviceDisplayPrefab;

    private void Update()
    {
        Debug.Log("player config count: " + PlayerConfigurationManager.instance.playerConfigs.Count.ToString());
        while (PlayerConfigurationManager.instance.playerConfigs.Count > deviceDisplays.Count)
        {
            // Add a new device display
            GameObject newObj = Instantiate(deviceDisplayPrefab, deviceDisplayContainer.transform);
            PlayerInputDeviceDisplay newDisplay = newObj.GetComponent<PlayerInputDeviceDisplay>();
            newDisplay.playerIndex = deviceDisplays.Count;
            newDisplay.ResetReadyTimer();
            deviceDisplays.Add(newDisplay);
        }


        foreach (PlayerInputDeviceDisplay deviceDisplay in deviceDisplays)
        {
            Debug.Log("ready timer done: " + deviceDisplay.CheckReadyTimer().ToString());
            deviceDisplay.UpdateDisplay();
        }
    }

    public bool CheckAllPlayersJoinedAndReady()
    {
        if (!PlayerConfigurationManager.instance.CheckAllPlayersJoined()) { return false; }

        if(PlayerConfigurationManager.instance.playerConfigs.Count > deviceDisplays.Count) { return false; }

        foreach (PlayerInputDeviceDisplay deviceDisplay in deviceDisplays)
        {
            if (!deviceDisplay.isReady) { return false; }
        }

        return true;
    }

    public void AssignPlayerTeamsToConfigs()
    {
        foreach (PlayerInputDeviceDisplay deviceDisplay in deviceDisplays)
        {
            PlayerConfiguration pConfig = PlayerConfigurationManager.instance.playerConfigs[deviceDisplay.playerIndex];
            pConfig.TeamIndex = deviceDisplay.sideSelection == PlayerDeviceSideSelection.LEFT ? 0 : 1;
        }
    }

    public void ProcessInput(PlayerMenuInput menuInput)
    {
        if (deviceDisplays.Count > 0)
        {
            for (int i = 0; i < deviceDisplays.Count; i++)
            {
                if (deviceDisplays[i].CheckReadyTimer())
                {
                    if (menuInput.menuInputValues[i].pConfirmValue && deviceDisplays[i].sideSelection != PlayerDeviceSideSelection.NEUTRAL)
                    {
                        deviceDisplays[i].isReady = true;
                    }
                    if (menuInput.menuInputValues[i].pBackValue)
                    {
                        deviceDisplays[i].isReady = false;
                    }
                }
                if (!deviceDisplays[i].isReady)
                {
                    float inputX = menuInput.menuInputValues[i].pMoveInMenuValue.x;
                    if (inputX < 0)
                    {
                        deviceDisplays[i].MoveLeft();
                    }
                    else if (inputX > 0)
                    {
                        deviceDisplays[i].MoveRight();
                    }
                }
            }
        }
    }
}
