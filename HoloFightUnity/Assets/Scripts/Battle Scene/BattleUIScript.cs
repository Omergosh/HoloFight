using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleUIScript : MonoBehaviour
{
    // Countdown UI + Canvas references
    public GameObject preroundCanvasObject;
    public Image preroundFullscreenOverlay;
    public TMP_Text waitingText;
    public TMP_Text countdownText;
    int roundStartLastNumberShown = 0;

    // UI references
    public TMP_Text roundTimerText;

    public TMP_Text p1CharNameText;
    public TMP_Text p1HealthText;
    public Slider p1HealthBarSlider;

    public TMP_Text p2CharNameText;
    public TMP_Text p2HealthText;
    public Slider p2HealthBarSlider;

    // Canvas references (for in-battle UI)
    public BattlePauseUIScript pauseUIScript;
    public BattleEndUIScript matchEndUIScript;
    public EventSystem eventSystem;
    public BaseInputModule inputModule;

    // Information needed to update UI
    public HfGame gameState;
    public BattleManagerScript battleManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBattleUI();
    }

    public void UpdateBattleUI()
    {
        // Update stored game state to reference for UI stuff
        gameState = battleManagerScript.game;

        // Current/temp: all UI references must be set/assigned before UI will update
        if (roundTimerText != null)
        {
            roundTimerText.text = gameState.RoundTimerCurrent.ToString();
        }
        if (p1CharNameText != null && p1HealthText != null && p1HealthBarSlider != null)
        {
            p1CharNameText.text = gameState.players[0].characterFullName;
            p1HealthText.text = $"{gameState.players[0].health} / {gameState.players[0].healthMax}";
            p1HealthBarSlider.value = gameState.players[0].health;
        }
        if (p2CharNameText != null && p2HealthText != null && p2HealthBarSlider != null)
        {
            p2CharNameText.text = gameState.players[1].characterFullName;
            p2HealthText.text = $"{gameState.players[1].health} / {gameState.players[0].healthMax}";
            p2HealthBarSlider.value = gameState.players[1].health;
        }
    }

    public void UpdatePauseUI(InputAction pauseInputAction, bool currentlyPaused)
    {
        pauseUIScript.UpdateUI(pauseInputAction, currentlyPaused);
        //Debug.Log("Update pause UI");
    }

    //public void Pause()
    //{
    //    pauseUIScript.gameObject.SetActive(true);
    //}

    public void Unpause()
    {
        pauseUIScript.Unpause();
    }

    public void ShowMatchEndScreen(int winnerPlayerIndex, string winnerCharacterName)
    {
        matchEndUIScript.ShowMenu(eventSystem, winnerPlayerIndex, winnerCharacterName);
    }

    // The three methods below and the method above (for interfaces appearing before/between/after rounds)
    // should probably be moved into another class later down the line.
    // For the sake of best practices (i.e. single responsibility) and all that.

    public void ShowWaitingScreen()
    {
        preroundCanvasObject.SetActive(true);
        //preroundFullscreenOverlay.SetActive(true);
        waitingText.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(false);
    }

    public void ShowCountdown(float countdownTime)
    {
        if (roundStartLastNumberShown == 0 && countdownTime > 0f)
        {
            preroundCanvasObject.SetActive(true);
            //preroundFullscreenOverlay.SetActive(true);
            waitingText.gameObject.SetActive(false);
            countdownText.gameObject.SetActive(true);

            countdownText.text = 3.ToString();
            roundStartLastNumberShown = 3;
        }

        if (roundStartLastNumberShown == 3
            && countdownTime < 2f)
        {
            countdownText.text = 2.ToString();
            roundStartLastNumberShown = 2;
        }
        else if (roundStartLastNumberShown == 2
           && countdownTime < 1f)
        {
            countdownText.text = 1.ToString();
            roundStartLastNumberShown = 1;
        }
        else if (roundStartLastNumberShown == 1
            && countdownTime <= 0f)
        {
            EndCountdown();
        }
    }

    public void EndCountdown()
    {
        preroundCanvasObject.SetActive(false);
        //preroundFullscreenOverlay
        waitingText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);

        // Reset for next time a countdown occurs
        roundStartLastNumberShown = 0;
    }
}
