using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerDeviceSideSelection
{
    NEUTRAL,
    LEFT,
    RIGHT
}

public class PlayerInputDeviceDisplay : MonoBehaviour
{
    public TMP_Text textField;
    public Image imageLeft;
    public Image imageRight;

    // state
    public PlayerDeviceSideSelection sideSelection = PlayerDeviceSideSelection.NEUTRAL;
    public bool isReady = false;
    public int playerIndex = -1;

    private float ignoreReadyTime = 0.01f;
    private bool readyEnabled = false;

    // For SOME reason, having this code in the Start function causes the timer to constantly reset
    // all the time and last way longer than it should.
    // For some reason.
    // Leaving this comment here until I figure out why, even though I DID fix the bug by commenting it out.
    //private void Start()
    //{
    //    ResetReadyTimer();
    //}

    private void Update()
    {
        CheckReadyTimer();
    }

    public void ResetReadyTimer() { ignoreReadyTime = Time.time + ignoreReadyTime; }

    public bool CheckReadyTimer()
    {
        if (!readyEnabled)
        {
            if (Time.time > ignoreReadyTime)
            {
                readyEnabled = true;
                return true;
            }
            return false;
        }
        return true;
    }

    public void UpdateDisplay()
    {
        PlayerConfiguration playerConfig = PlayerConfigurationManager.instance.playerConfigs[playerIndex];
        UpdateText(playerConfig.Input.currentControlScheme);
        textField.color = isReady ? Color.yellow : Color.white;
    }

    public void UpdateText(string newText)
    {
        textField.text = newText;
    }

    public bool MoveLeft()
    {
        if (!isReady)
        {
            if (sideSelection == PlayerDeviceSideSelection.NEUTRAL)
            {
                sideSelection = PlayerDeviceSideSelection.LEFT;
                imageLeft.gameObject.SetActive(false);
                imageRight.gameObject.SetActive(true);
                textField.alignment = TextAlignmentOptions.Left;
                return true;
            }
            else if (sideSelection == PlayerDeviceSideSelection.RIGHT)
            {
                sideSelection = PlayerDeviceSideSelection.NEUTRAL;
                imageLeft.gameObject.SetActive(true);
                imageRight.gameObject.SetActive(true);
                textField.alignment = TextAlignmentOptions.Center;
                return true;
            }
            return false;
            // Returns false if no movement occurs, for the sake of triggering the correct SFX events, etc.
        }
        return false;
    }

    public bool MoveRight()
    {
        if (!isReady)
        {
            if (sideSelection == PlayerDeviceSideSelection.NEUTRAL)
            {
                sideSelection = PlayerDeviceSideSelection.RIGHT;
                imageLeft.gameObject.SetActive(true);
                imageRight.gameObject.SetActive(false);
                textField.alignment = TextAlignmentOptions.Right;
                return true;
            }
            else if (sideSelection == PlayerDeviceSideSelection.LEFT)
            {
                sideSelection = PlayerDeviceSideSelection.NEUTRAL;
                imageLeft.gameObject.SetActive(true);
                imageRight.gameObject.SetActive(true);
                textField.alignment = TextAlignmentOptions.Center;
                return true;
            }
            return false;
            // Returns false if no movement occurs, for the sake of triggering the correct SFX events, etc.
        }
        return false;
    }
}
