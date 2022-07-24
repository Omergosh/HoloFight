using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleSceneUIScript : MonoBehaviour
{
    // UI references
    public TMP_Text roundTimerText;

    public TMP_Text p1CharNameText;
    public TMP_Text p1HealthText;
    public Slider p1HealthBarSlider;

    public TMP_Text p2CharNameText;
    public TMP_Text p2HealthText;
    public Slider p2HealthBarSlider;

    // Canvas references
    public BattleScenePauseUIScript pauseUIScript;

    // Information needed to update UI
    public HfGame gameState;
    public GameManagerScript gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Update stored game state to reference for UI stuff
        gameState = gameManagerScript.game;

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

    //public void Unpause()
    //{
    //    pauseUIScript.gameObject.SetActive(false);
    //}
}
