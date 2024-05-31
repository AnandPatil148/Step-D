using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public GameObject PC;

    public GameObject playerTabListItem;
    public GameObject playerTabListContent;

    public RoomManager roomManager;
    public Vector3 spawnPoint;

    public Player[] players;

    private void Awake() 
    {
        roomManager = RoomManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            spawnPoint = new Vector3(Random.Range(-5,5), 1, Random.Range(1,3));

            CreateController();

            //Invoke(nameof(TabListCreate),2f);
        }
    }

    void CreateController()
    { 
        PC = PhotonNetwork.Instantiate(PC.name, spawnPoint, Quaternion.identity);
        PC.name = "LocalPC";
        roomManager.PC = PC;
    }

    /*
    void TabListCreate()
    {
        playerTabListContent = PC.GetComponent<PlayerController>().mainCanvas.transform.Find("Tab").transform.Find("PlayerTabListContent").gameObject;

        players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerTabListContent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Length; i++ )
        {
            Instantiate(playerTabListItem, playerTabListContent.transform).GetComponent<PlayerTabListItem>().SetUp(players[i], roomManager.Mplayers[i].GetComponent<PlayerController>());
        }
    }
    */
    
}
