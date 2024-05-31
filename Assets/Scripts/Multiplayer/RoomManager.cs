using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager Instance;
    public PlayfabManager PlayfabManager;
    public LevelComplete LevelComplete;
    public GameObject Chunk;
    public GameObject ENDChunk;

    //Step Prefab
    public GameObject step;

    //DMerit Prefab
    public GameObject DMerit;
    
    /// Player manager Object Prefab
    public GameObject PMPrefab; 
    public GameObject PM; 
    public GameObject PC;
    public PlayerController PCController;

    
    //Room Objects List
    public GameObject[] StepsRemaining;
    public GameObject[] DMeritsRemaining;
    public GameObject[] Chunks;

    //Winer name
    public string winnerName;

    //Game logic stuff
    public int NumberOfChunks;
    public bool gameEnded;
    public int luckA, luckB, luck;

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
        PlayfabManager = PlayfabManager.Instance;
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
                
                //calls The player listing method and sets Player Details properly
                Launcher.Instance.OnJoinedRoom();
                Launcher.Instance.SetPlayerDetails(PlayfabManager.Name, PlayfabManager.Steps, PlayfabManager.DMerits);
                winnerName = null;
                break;
            case 7:
                LevelComplete = LevelComplete.Instance;
                GameLoader();
                break;
        }
    }

    public void Update() // get plavers
    {
        if(SceneManager.GetActiveScene().buildIndex == 7)
        {
            StepsRemaining = GameObject.FindGameObjectsWithTag("Step");
            DMeritsRemaining = GameObject.FindGameObjectsWithTag("DMerit");
        }
    }

    public void GameLoader()
    {

        gameEnded = false;

        //Instantiate all the chunks
        LoadMap();
        
        //Instantiate all steps Only By master client
        InstantiateSteps();

        // Instantietes the Player Manager
        PM = PhotonNetwork.Instantiate(PMPrefab.name, Vector3.zero, Quaternion.identity);
        PM.name = "LocalPM";
        
    }

    public void LoadMap()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 1; i < NumberOfChunks; i++)
            {
                Chunks.Append(PhotonNetwork.InstantiateRoomObject(Chunk.name, 15 * i * Vector3.forward, Quaternion.identity));
                
            }

            Chunks.Append(PhotonNetwork.InstantiateRoomObject(ENDChunk.name, 15 * NumberOfChunks * Vector3.forward, Quaternion.identity));
            
        }
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
    public void CompleteLevel(string winnerName) 
    {
        gameEnded = true;
        Debug.Log("PC compeleted lvl");

        PCController = PC.GetComponent<PlayerController>();
        PCController.rb.velocity = Vector3.zero;
        PCController.rb.angularVelocity = Vector3.zero;

        PCController.canvasManager.MainPanel.SetActive(false);

        LevelComplete.SetFinalDetails(winnerName,PCController.StepsCount, PCController.transform.position.z );

        PlayfabManager.AddVirtualCurrency("ST", PCController.StepsCount);
        PlayfabManager.AddVirtualCurrency("DM", PCController.DMeritsCount);

        Invoke(nameof(DestroyPMCs), 2);

        if (PhotonNetwork.IsMasterClient)
        {
            if(StepsRemaining != null)
            {
                foreach (GameObject Step in StepsRemaining)
                {
                    PhotonNetwork.Destroy(Step.GetPhotonView());
                }
            }

            if (DMeritsRemaining != null )
            {
                foreach (GameObject DMerit in DMeritsRemaining)
                {
                    PhotonNetwork.Destroy(DMerit.GetPhotonView());
                }
            }

            foreach (GameObject chunk in GameObject.FindGameObjectsWithTag("Chunk"))
            {
                PhotonNetwork.Destroy(chunk.GetPhotonView());
            }
                 
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;

            Invoke(nameof(BackToRoom), 6);
        }
        
    }

    public void DestroyPMCs()
    {
        PhotonNetwork.Destroy(PC.GetPhotonView());
        PhotonNetwork.Destroy(PM.GetPhotonView());
    }

    public void BackToRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

}

