using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

public class BattleScenePauseUIScript : MonoBehaviour
{
    public Image fullscreenOverlay;
    public Color fullscreenOverlayColor;
    public GameObject pauseMenuPanel;
    public ProceduralImage pauseTimerFillShape;

    public float pauseTimerFillTimeLimit = 0.4f;
    private float _pauseProgress= 0f;
    public float PauseProgress // value range: [0f, 1f]
    {
        get { return _pauseProgress; }
        set
        {
            if (value > 1f) { _pauseProgress = 1f; }
            else if (value < 0f) { _pauseProgress = 0f; }
            else { _pauseProgress = value; }

            UpdatePauseProgressUI(); // Pretty sure this is bad practice so consider removing/reworking this line later
        }
    }

    public bool initiatingPause = false;

    public void Start()
    {
        fullscreenOverlay.gameObject.SetActive(false);
        pauseMenuPanel.gameObject.SetActive(false);
        pauseTimerFillShape.gameObject.SetActive(false);

        fullscreenOverlayColor = fullscreenOverlay.color;
        PauseProgress = 0f;
    }

    private void Update()
    {
        if (initiatingPause)
        {
            PauseProgress += Time.deltaTime / pauseTimerFillTimeLimit;
        }
    }

    public void UpdatePauseProgressUI()
    {
        pauseTimerFillShape.fillAmount = PauseProgress;
        fullscreenOverlay.color = new Color(
                fullscreenOverlayColor.r,
                fullscreenOverlayColor.g,
                fullscreenOverlayColor.b,
                //fullscreenOverlayColor.a * 0.5f
                fullscreenOverlayColor.a * PauseProgress
            );
        Debug.Log(fullscreenOverlayColor.a);
    }

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
        fullscreenOverlay.color = new Color(
                fullscreenOverlayColor.r,
                fullscreenOverlayColor.g,
                fullscreenOverlayColor.b,
                //fullscreenOverlayColor.a * 0.5f
                fullscreenOverlayColor.a * PauseProgress
            );
        fullscreenOverlay.gameObject.SetActive(true);

        PauseProgress += Time.deltaTime / pauseTimerFillTimeLimit;
        pauseTimerFillShape.gameObject.SetActive(true);

        initiatingPause = true;
    }

    public void AbortPause()
    {
        // Just let go of the pause button without holding it down long enough to fully pause the game.
        PauseProgress = 0f;
        fullscreenOverlay.gameObject.SetActive(false);
        pauseTimerFillShape.gameObject.SetActive(false);
        initiatingPause = false;
    }

    public void Pause()
    {
        PauseProgress = 0f;
        fullscreenOverlay.color = fullscreenOverlayColor;
        fullscreenOverlay.gameObject.SetActive(true);
        pauseMenuPanel.gameObject.SetActive(true);
        pauseTimerFillShape.gameObject.SetActive(false);
        initiatingPause = false;
    }

    public void Unpause()
    {
        PauseProgress = 0f;
        fullscreenOverlay.gameObject.SetActive(false);
        pauseMenuPanel.gameObject.SetActive(false);
        pauseTimerFillShape.gameObject.SetActive(false);
        initiatingPause = false;
    }
}
