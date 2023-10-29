using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSingle : MonoBehaviour
{
    public PlayerMovement playerMovement;  

    //Step score
    public int  steps;
    public int  dmerits;
    public float score;

    [Header("Main Score Panel")]
    public TMP_Text scoreText;
    public TMP_Text stepScoreText;
    
    [Header("Final Score Panel")]
    public TMP_Text finalScoreText;
    public TMP_Text finalStepScoreText;

    [Header("D-Merits Panel")]
    public TMP_Text DMeritsText;

    // Update is called once per frame
    private void Update()
    {
        SPlayerPos();
    }

    public void SPlayerPos()
    {
        steps = playerMovement.step;

        if(playerMovement.transform.position.z > score)
        {
            score = playerMovement.transform.position.z;
            scoreText.text = score.ToString("0");
            finalScoreText.text = scoreText.text;      
        }

        dmerits = playerMovement.DMerits;
        DMeritsText.text = dmerits.ToString();

        stepScoreText.text = steps.ToString();
        finalStepScoreText.text = stepScoreText.text;
    }

}
