using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class CanvasManagerMulti : MonoBehaviour
{
    public PlayerController PC;
    
    //Step score
    public int  steps;
    public int  dmerits;
    public float score;
    public float speed;

    [Header("Main Score Panel")]
    public TMP_Text scoreText;
    public TMP_Text stepScoreText;
    public TMP_Text speedText;
    public TMP_Text DMeritsText;    
    
    [Header("Final Score Panel")]
    public TMP_Text finalScoreText;
    public TMP_Text finalStepScoreText;

    [Header("Esc Panel")]
    public GameObject escPanel;
    public TMP_Text roomName;
    public TMP_Text escScoreText;
    public TMP_Text escStepScoreText;

    [Header("Tab Panel")]
    public GameObject tabPanel;
    public TMP_Text playerCount;

    // Update is called once per frame
    private void Update()
    {
        MPlayerScore(); 

    }
    
    public void MPlayerScore()
    {


        /*
        if(playerController.transform.position.z > score)
        {
            score = playerController.transform.position.z;
            scoreText.text = score.ToString("0");
            finalScoreText.text = scoreText.text;
        }
        */

        
        score = PC.transform.position.z;
        scoreText.text = score.ToString("0");
        finalScoreText.text = scoreText.text;
        escScoreText.text = scoreText.text;

        dmerits = PC.DMerits;
        DMeritsText.text = dmerits.ToString();
        
        steps = PC.stepCount;
        stepScoreText.text = steps.ToString();
        finalStepScoreText.text = stepScoreText.text;
        escStepScoreText.text = stepScoreText.text;
        
        speed = PC.forwardForce;
        speedText.text = speed.ToString();
    }

}
