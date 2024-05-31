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
    public GameObject MainPanel;
    public TMP_Text scoreText;
    public TMP_Text stepScoreText;
    public TMP_Text speedText;
    public TMP_Text DMeritsText;    
    
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
        TabPanel();
        EscPanel();

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
        escScoreText.text = scoreText.text;

        dmerits = PC.DMeritsCount;
        DMeritsText.text = dmerits.ToString();
        
        steps = PC.StepsCount;
        stepScoreText.text = steps.ToString();
        escStepScoreText.text = stepScoreText.text;
        
        speed = PC.forwardForce;
        speedText.text = speed.ToString();
    }

    public void TabPanel()
    {

        if (Input.GetKeyDown(KeyCode.Tab)) tabPanel.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Tab)) tabPanel.SetActive(false);

        playerCount.text = PhotonNetwork.PlayerList.Length.ToString();
    }

    public void EscPanel()
    {
        bool open = false;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (open == false)
            {
                open = true;
                escPanel.SetActive(true);
            }
            else if (open == true)
            {
                open = false;
                escPanel.SetActive(false);
            }
        }

    }

}
