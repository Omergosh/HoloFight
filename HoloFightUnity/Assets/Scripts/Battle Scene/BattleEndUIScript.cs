using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleEndUIScript : MonoBehaviour
{
    public Image fullscreenOverlay;
    public Button defaultMenuItem;
    public TMP_Text winnerPlayerNumText;
    public TMP_Text winnerCharacterNameText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMenu(EventSystem eventSystem, int winnerPlayerIndex, string winnerCharacterName)
    {
        if (winnerPlayerIndex >= 0) // If there was a winner
        {
        winnerPlayerNumText.text = "Winner: P" + (winnerPlayerIndex + 1).ToString();
        winnerCharacterNameText.text = winnerCharacterName + "!";
        }
        else // If there was a tie
        {
            winnerPlayerNumText.text = "No Winner";
            winnerCharacterNameText.text = "Tie!";
        }

        this.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(defaultMenuItem.gameObject);
    }
}
