using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher Instance;

    public Player[] players;

    public GameObject roomManager;

    [Header("Room Listing")]
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;

    [Header("Player In Room Listing")]
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameObject;

    [Header("Login Stuff")]
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text playerProfileName;
    [SerializeField] TMP_Text playerStepsCount;
    [SerializeField] TMP_Text playerDMeritsCount;



    //Instance = this
    void Awake()
	{
		Instance = this;
	}


    //Connect to Master Server
    void Start()
    {
        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }


    //Join Lobby After Connecting to Master
    public override void OnConnectedToMaster() 
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;    
    }


    //Open Login Menu after Joinin Lobby
    public override void OnJoinedLobby()
    {
        if(PlayfabManager.Instance.logedin)
        {
		    MenuManager.Instance.OpenMenu("title");
        }
        else
        {
            MenuManager.Instance.OpenMenu("login");
        }
    }

    //Get Player name
    public void SetPlayerDetails(string playerName, int Steps, int DMerits)
    {
        PhotonNetwork.NickName = playerName;
        playerProfileName.text = playerName;
        playerStepsCount.text = Steps.ToString();
        playerDMeritsCount.text = DMerits.ToString();
    }

    //Create Room
    public void CreateRoom()
	{
		if(string.IsNullOrEmpty(roomNameInputField.text))
		{
			return;
		}
		PhotonNetwork.CreateRoom(roomNameInputField.text);
		MenuManager.Instance.OpenMenu("loading");
	}

    //Join Room
    public void JoinRoom(RoomInfo info)
	{
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loading");
	}

#region  RoomMenu Stuff

    //After Joining room instantiate All player prefabs in the room
    public override void OnJoinedRoom()
	{   
        //Instantiates RoomManager if it is not there
        if(!GameObject.Find("RoomManager")) 
        {
            Instantiate(roomManager, Vector3.zero, Quaternion.identity).name = "RoomManager";
        }

        MenuManager.Instance.OpenMenu("room");
        
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        startGameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
    

    //when master client leaves Asign start game option to new master client
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    //New player joins and instantiates his player prefab
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    //start game option
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(7);
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }


    // room creation fails
    public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "Room Creation Failed: " + message;
		//Debug.LogError("Room Creation Failed: " + message);
		MenuManager.Instance.OpenMenu("error");
	}

    //Leave room
    public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loading");
	}


    //After leaving room
    public override void OnLeftRoom()
	{
        Destroy(RoomManager.Instance.gameObject);
		MenuManager.Instance.OpenMenu("title");
	}
#endregion

    //updates room list
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    //Disconnect from Master Server
	public void DisconnectFromMasterAndLogOut()
	{
	    PhotonNetwork.Disconnect();
        PlayfabManager.Instance.Logout();        
        SceneManager.LoadScene("MainMenu");
	}

	
}
