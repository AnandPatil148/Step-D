using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using TMPro;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public static LevelComplete Instance;

    public GameObject LevelCompleteUI;
    public TMP_Text txtWinnerName;
    public TMP_Text txtFinalScoreText;
    public TMP_Text txtFinalStepScoreText;


    void Awake()
    {
        Instance = this;
    }

    public void SetFinalDetails(string winnerName, int StepsCount, float Score )
    {
        Debug.Log("Set Final Details called");
        LevelCompleteUI.SetActive(true);
        txtWinnerName.text = winnerName;
        txtFinalStepScoreText.text =StepsCount.ToString() ;
        txtFinalScoreText.text =Score.ToString("0") ;
    }

}
