using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleSceneUIScript : MonoBehaviour
{
    // UI references
    public TMP_Text roundTimerText;
    public TMP_Text p1CharNameText;
    public TMP_Text p1HealthText;
    public TMP_Text p2CharNameText;
    public TMP_Text p2HealthText;

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
        if (p1CharNameText != null && p1HealthText != null)
        {
            p1CharNameText.text = gameState.players[0].characterFullName;
            p1HealthText.text = $"{gameState.players[0].health} / {gameState.players[0].healthMax}";
        }
        if (p2CharNameText != null && p2HealthText != null)
        {
            p2CharNameText.text = gameState.players[1].characterFullName;
            p2HealthText.text = $"{gameState.players[1].health} / {gameState.players[0].healthMax}";
        }
    }
}
