using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    public GameManager gameManager;
    public RoomManager roomManager;

    private void Start()
    {
        if(GameObject.FindGameObjectWithTag("GameManager"))
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        if(GameObject.FindGameObjectWithTag("RoomManager")) 
        {   
            roomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();   
        }
    }
    /*
    private void OnTriggerEnter(Collider triggerInfo)
    {

        if(gameManager) gameManager.CompleteLevel();

        else roomManager.CompleteLevel(triggerInfo.gameObject);
        
        

    }
    */
}
