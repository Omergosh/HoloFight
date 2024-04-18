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

    [SerializeField]
    MainMenuManager mainMenuManager;

    private void Update()
    {
        //Debug.Log("player config count: " + PlayerConfigurationManager.instance.playerConfigs.Count.ToString());
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
            //Debug.Log("ready timer done: " + deviceDisplay.CheckReadyTimer().ToString());
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

    public bool AreOtherDevicesOnSameSide(int indexOfDeviceDisplayToCompare)
    {
        bool onSameSide = false;
        for(int j = 0; j < deviceDisplays.Count; j++)
        {
            if(j != indexOfDeviceDisplayToCompare)
            {
                if(deviceDisplays[j].sideSelection == deviceDisplays[indexOfDeviceDisplayToCompare].sideSelection
                    && deviceDisplays[j].isReady)
                {
                    onSameSide = true;
                }
            }
        }
        return onSameSide;
    }

    public void ProcessInput(PlayerMenuInput menuInput)
    {
        if (deviceDisplays.Count > 0)
        {
            for (int i = 0; i < deviceDisplays.Count; i++)
            {
                if (deviceDisplays[i].CheckReadyTimer())
                {
                    if (menuInput.menuInputValues[i].pConfirmValue
                        && deviceDisplays[i].sideSelection != PlayerDeviceSideSelection.NEUTRAL
                        && !deviceDisplays[i].isReady
                        && !AreOtherDevicesOnSameSide(i)
                        )
                    {
                        deviceDisplays[i].isReady = true;
                        GameManager.instance.audioManager.PlaySFX("deviceSetupJoin");
                    }
                }
                if (!deviceDisplays[i].isReady)
                {
                    float inputX = menuInput.menuInputValues[i].pMoveInMenuValue.x;
                    if (inputX < 0)
                    {
                        deviceDisplays[i].MoveLeft();
                        GameManager.instance.audioManager.PlaySFX("menuMove");
                    }
                    else if (inputX > 0)
                    {
                        deviceDisplays[i].MoveRight();
                        GameManager.instance.audioManager.PlaySFX("menuMove");
                    }
                }
                if (menuInput.menuInputValues[i].pBackValue)
                {
                    if (deviceDisplays[i].isReady)
                    {
                        deviceDisplays[i].isReady = false;
                        GameManager.instance.audioManager.PlaySFX("deviceSetupCancel");
                    }
                    else
                    {
                        Reset();
                        mainMenuManager.CloseVersusControllerSetup();
                        return;
                    }
                }
            }
        }
    }

    public void Reset()
    {
        foreach(PlayerInputDeviceDisplay deviceDisplay in deviceDisplays)
        {
            Destroy(deviceDisplay.gameObject, 0.01f);
        }
        deviceDisplays = new List<PlayerInputDeviceDisplay>();
    }
}
