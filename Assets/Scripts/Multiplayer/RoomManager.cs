using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager Instance;

    //Step Prefab
    public GameObject step;

    //DMerit Prefab
    public GameObject DMerit;
    
    /// Player manager Object Prefab
    public GameObject PM; 

    //Winer name
    public string winner;

    //Game logic stuff
    public bool gameEnded;
    public int luckA, luckB, luck;

    //List of players
    public GameObject[] Mplayers;

    public GameObject MyPlayer;

    //Instance = this;
    public void Awake() 
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
    

    public override void OnEnable() 
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        switch (scene.buildIndex)
        {
            case 1:
                //calls The player listing method and sets name properly
                Launcher.Instance.OnJoinedRoom();
                Launcher.Instance.SetPlayerDetails(LoginPagePlayfab.Instance.Name);
                winner = null;
                break;
            case 7:
                //calls game loader method which starts game
                GameLoader();
                break;
        }
    }

    public void Update() // get plavers
    {
        MyPlayer = GameObject.FindGameObjectWithTag("MPlayer");
        
    }

    public void GameLoader()
    {

        gameEnded = false;
        
        //instantiate all steps Only By master client
        InstantiateSteps();

        PhotonNetwork.Instantiate(PM.name, Vector3.zero, Quaternion.identity).name = "LocalPM";
    }

    public void InstantiateSteps()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            
            int stepCount = PhotonNetwork.PlayerList.Length * 4;
            
            for (int i = 0; i < stepCount; i++)
            {
                PhotonNetwork.InstantiateRoomObject(step.name,   new Vector3(Random.Range(-5,5), 1, Random.Range(10,150)),  Quaternion.identity);
    
            }

            if( Random.Range(luckA, luckB) == luck)
            {
                PhotonNetwork.InstantiateRoomObject(DMerit.name, new Vector3(Random.Range(-5, 5), 1, Random.Range(10, 150)), Quaternion.identity );
            }


        }
    }
    
     
    /// When Game gets completed
    public void CompleteLevel(GameObject winnerGameObject) 
    {

        gameEnded = true;

        //PhotonNetwork.AutomaticallySyncScene = true;

        //Gets Winner Details
        winner = winnerGameObject.GetComponent<PlayerController>().PV.Owner.NickName;

        PlayerController PC = MyPlayer.GetComponent<PlayerController>();
        PC.winnerName.text = winner;
        //Mplayer.transform.Find("CanvasFinish").transform.Find("LevelComplete").gameObject.SetActive(true);
        PC.mainCanvas.transform.Find("LevelComplete").gameObject.SetActive(true);
        PC.enabled = false;

        if (PhotonNetwork.IsMasterClient)
        {
            Invoke(nameof(BackToRoom), 10);
        }

        
    }

    public void BackToRoom()
    {
        if( gameEnded )
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

}

