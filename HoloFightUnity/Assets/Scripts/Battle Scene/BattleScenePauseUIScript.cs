using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleScenePauseUIScript : MonoBehaviour
{
    public Image fullscreenOverlay;
    public GameObject pauseMenuPanel;

    public void UpdateUI(InputAction pauseInputAction, bool currentlyPaused)
    {
        if (!currentlyPaused)
        {
            // Pause
            if (pauseInputAction.WasPressedThisFrame())
            {
                // If pause button was just pressed this frame, activate some UI
                //Debug.Log("SHOW pause timer");
                BeginToPause();
            }
            else if (pauseInputAction.WasReleasedThisFrame())
            {
                // If pause button UI was shown, remove it because we're not pausing now after all!
                //Debug.Log("HIDE pause timer");
                AbortPause();
            }
            else if (pauseInputAction.triggered)
            {
                // If pause button was held for long enough, actually pause the game
                //battleUIScript.UpdatePauseUI(playerInputScripts[0].p1PauseAction, isGamePaused);
                //isGamePaused = true;
                Pause();
            }
        }
        else
        {
            // Unpause
            if (pauseInputAction.WasPressedThisFrame())
            {
                // Unlike pausing, unpausing occurs instantly upon pressing the pause button.
                //isGamePaused = false;
                Unpause();
            }
        }
    }

    public void BeginToPause()
    {
        // Currently holding down the pause button,
        // but has not yet held it down long enough to fully pause the game.
        fullscreenOverlay.gameObject.SetActive(true);
    }

    public void AbortPause()
    {
        // Just let go of the pause button without holding it down long enough to fully pause the game.
        fullscreenOverlay.gameObject.SetActive(false);
    }

    public void Pause()
    {
        fullscreenOverlay.gameObject.SetActive(true);
        pauseMenuPanel.gameObject.SetActive(true);
    }

    public void Unpause()
    {
        fullscreenOverlay.gameObject.SetActive(false);
        pauseMenuPanel.gameObject.SetActive(false);
    }
}
