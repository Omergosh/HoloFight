using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

public enum PauseMenuState
{
    CLOSED,
    PAUSE_MAIN,
    OPTIONS,
    CONTROLS,
}

public class BattlePauseUIScript : MonoBehaviour
{
    [Header("Pause Timer UI References")]
    [SerializeField] private Image fullscreenOverlay;
    [SerializeField] private Color fullscreenOverlayColor;
    [SerializeField] private ProceduralImage pauseTimerFillShape;

    [Header("Pause UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject buttonBindingsMenuPanel;
    [SerializeField] private MenuListController pauseMenu;
    [SerializeField] private MenuListController optionsMenu;
    [SerializeField] private MenuListController buttonBindingsMenu;
    [SerializeField] private MenuCursor menuCursor;

    [Header("Script References")]
    [SerializeField] private BattleManagerScript battleManagerScript;
    public PlayerBattleInput pausePlayerBattleInput;
    public PlayerMenuInput menuInput;

    [SerializeField] public float pauseTimerFillTimeLimit = 0.4f;
    private float _pauseProgress= 0f;
    public float PauseProgress // value range: [0f, 1f]
    {
        get { return _pauseProgress; }
        set
        {
            if (value > 1f) { _pauseProgress = 1f; }
            else if (value < 0f) { _pauseProgress = 0f; }
            else { _pauseProgress = value; }
        }
    }

    public bool initiatingPause = false;
    public int pausePlayerIndex = 0;
    public PauseMenuState state = PauseMenuState.CLOSED;

    public void Start()
    {
        fullscreenOverlay.gameObject.SetActive(false);
        pauseTimerFillShape.gameObject.SetActive(false);
        pauseMenuPanel.gameObject.SetActive(false);
        //optionsMenuPanel.gameObject.SetActive(false);
        //buttonBindingsMenuPanel.gameObject.SetActive(false);
        menuCursor.gameObject.SetActive(false);

        menuInput.InitializeWithAllPlayers();
        menuInput.enabled = false;

        fullscreenOverlayColor = fullscreenOverlay.color;
        PauseProgress = 0f;
    }

    private void Update()
    {
        switch (state)
        {
            case PauseMenuState.CLOSED:
                if (initiatingPause)
                {
                    PauseProgress += Time.deltaTime / pauseTimerFillTimeLimit;
                    UpdatePauseProgressUI();
                }
                break;
            case PauseMenuState.PAUSE_MAIN:
                pauseMenu.ProcessInput(menuInput);
                menuCursor.SetCursorPosition(pauseMenu.currentlySelectedMenuOption);
                break;
        }
    }

    public void UpdatePauseProgressUI()
    {
        pauseTimerFillShape.fillAmount = PauseProgress;
        fullscreenOverlay.color = new Color(
                fullscreenOverlayColor.r,
                fullscreenOverlayColor.g,
                fullscreenOverlayColor.b,
                fullscreenOverlayColor.a * PauseProgress
            );
    }

    //public void UpdateUI(InputAction pauseInputAction, bool currentlyPaused)
    public void ProcessPauseButtonInputOnly(PlayerBattleInput[] playerBattleInputs, bool currentlyPaused)
    {
        bool pausePressed = false;
        bool pauseReleased = false;
        bool pauseTriggered = false;

        foreach (PlayerBattleInput playerBattleInput in playerBattleInputs)
        {
            if (playerBattleInput.p1PauseAction.WasPressedThisFrame())
            {
                pausePressed = true;
                if (!initiatingPause)
                {
                    pausePlayerIndex = playerBattleInput.playerInput.playerIndex;
                    pausePlayerBattleInput = playerBattleInputs[pausePlayerIndex];
                }
            }
            if (pausePlayerIndex == playerBattleInput.playerInput.playerIndex && playerBattleInput.p1PauseAction.WasReleasedThisFrame())
            {
                pauseReleased = true;
            }
            if (pausePlayerIndex == playerBattleInput.playerInput.playerIndex && playerBattleInput.p1PauseAction.triggered)
            {
                pauseTriggered = true;
            }
        }

        if (!currentlyPaused)
        {
            // Pause
            if (pausePressed)
            {
                // If pause button was just pressed this frame, activate some UI
                //Debug.Log("SHOW pause timer");
                BeginToPause();
            }
            else if (pauseReleased)
            {
                // If pause button UI was shown, remove it because we're not pausing now after all!
                //Debug.Log("HIDE pause timer");
                AbortPause();
            }
            else if (pauseTriggered)
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
            if (pausePressed)
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
        UpdatePauseProgressUI();
        pauseTimerFillShape.gameObject.SetActive(true);

        initiatingPause = true;
    }

    public void AbortPause()
    {
        // Just let go of the pause button without holding it down long enough to fully pause the game.
        PauseProgress = 0f;
        UpdatePauseProgressUI();
        fullscreenOverlay.gameObject.SetActive(false);
        pauseTimerFillShape.gameObject.SetActive(false);
        initiatingPause = false;
    }

    public void Pause()
    {
        PauseProgress = 0f;
        UpdatePauseProgressUI();
        fullscreenOverlay.color = fullscreenOverlayColor;
        fullscreenOverlay.gameObject.SetActive(true);
        pauseMenuPanel.gameObject.SetActive(true);
        pauseTimerFillShape.gameObject.SetActive(false);
        menuCursor.gameObject.SetActive(true);
        initiatingPause = false;

        menuInput.enabled = true;
        state = PauseMenuState.PAUSE_MAIN;

        battleManagerScript.isGamePaused = true;
        PlayerConfigurationManager.instance.EnableMenuInputs();
    }

    public void Unpause()
    {
        PauseProgress = 0f;
        UpdatePauseProgressUI();
        fullscreenOverlay.gameObject.SetActive(false);
        pauseMenuPanel.gameObject.SetActive(false);
        pauseTimerFillShape.gameObject.SetActive(false);
        menuCursor.gameObject.SetActive(false);
        initiatingPause = false;

        menuInput.ResetMenuInputValues();
        menuInput.enabled = false;
        state = PauseMenuState.CLOSED;

        // If player used the attack button to unpause, don't make that player attack on this button press
        if (pausePlayerBattleInput.p1AttackAValue) { pausePlayerBattleInput.p1PausePressLockoutAttackA = true; }

        battleManagerScript.isGamePaused = false;
        PlayerConfigurationManager.instance.DisableMenuInputs();
    }

    public void OpenOptionsMenu()
    {
        Debug.Log("Options!");
    }
    
    public void OpenButtonBindingsMenu()
    {
        Debug.Log("Button Bindings!");
    }
}
