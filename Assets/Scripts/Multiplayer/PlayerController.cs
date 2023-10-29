using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("References")]
    // room manager reference
    public RoomManager roomManager;
    public Rigidbody rb;
    public PhotonView PV;
    public GameObject playerCam;
    //reference to Effects
    public Effects effects;

    //Canvas stuff
    public TMP_Text winnerName;
    public GameObject mainCanvas;
    public CanvasManagerMulti canvasManager;

    [Header("Input")]
    public bool wInput;
    public bool aInput;
    public bool sInput;
    public bool dInput;
    public bool qInput;
    public bool xInput;

    public bool tabInput;

    [Header("Forces")]
    //right and left movement
    public float forwardForce;
    public float sidewaysForce;

    [Header("Jump")]
    //Jump stuff
    public float jumpForce;
    public bool isGrounded;


    [Header("Steps")]
    //step stuff(temp)
    public int stepCount;

    [Header("DMerits")]
    public int DMerits;

    //obstacle hit
    public bool obstacleHit;
    //look stuff
    public Vector3 offset;
    public Vector3 frontView;
    public Vector3 clampRotation;


    private void Start() 
    {
        if(!PV.IsMine)
        {
            //playerCam.GetComponent<Camera>().enabled = false;
            //playerCam.GetComponent<AudioListener>().enabled = false;
            //rb.isKinematic = true;
            playerCam.SetActive(false);
            mainCanvas.SetActive(false);
            Destroy(rb);
        }

        roomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        canvasManager.roomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    private void Update() 
    {   
        if(!PV.IsMine) return;
        
        MyInput();
        EscPanel();
        Look();
        EffectsCheck();

        

        if (rb.position.y < -1f && obstacleHit == false) Invoke(nameof(Restart),2f);
        
        if(isGrounded && Input.GetKeyDown(KeyCode.Space)) Jump();

        //PhotonNetwork.AutomaticallySyncScene = roomManager.gameEnded;
    }

    private void FixedUpdate() 
    {   
        if (!PV.IsMine) return;
        Movement();    
    }

    //Get All Input From Keyboard
    private void MyInput() 
    {
        //Get All Movement Related  Input From Keyboard
        wInput = Input.GetKey(KeyCode.W);
        aInput = Input.GetKey(KeyCode.A);
        sInput = Input.GetKey(KeyCode.S);
        dInput = Input.GetKey(KeyCode.D);
        xInput = Input.GetKey(KeyCode.X);


        //Get All Effects Related Input From Keyboard
        qInput = Input.GetKeyDown(KeyCode.Q);

    }

    //Adds force
    private void Movement()
    {
        if(wInput) rb.AddForce(Vector3.forward * forwardForce * Time.deltaTime, ForceMode.Force);
        if(sInput) rb.AddForce(Vector3.back * forwardForce * Time.deltaTime, ForceMode.Force);
        if(aInput) rb.AddForce(Vector3.left * sidewaysForce * Time.deltaTime, ForceMode.VelocityChange);
        if(dInput) rb.AddForce(Vector3.right * sidewaysForce * Time.deltaTime, ForceMode.VelocityChange);
    }

    // Makes obj jump
    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce,  ForceMode.Impulse);
        //rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.y);
        isGrounded = false;
    }

    // Camera related stuff
    private void Look()
    {
        playerCam.transform.position = transform.position + offset;
        playerCam.transform.eulerAngles = clampRotation;

        if(xInput) 
        {
            playerCam.transform.eulerAngles = new Vector3(0f,180f,0f);
            playerCam.transform.position = transform.position + frontView;
        }

        if(Input.GetKeyDown(KeyCode.Z)) playerCam.GetComponent<Camera>().fieldOfView -= 30; //Zoom in
        if(Input.GetKeyUp(KeyCode.Z)) playerCam.GetComponent<Camera>().fieldOfView += 30; //Zoom out
    }

    //Effects Check
    private void EffectsCheck()
    {
        if(qInput && DMerits != 0)
        {
            DMerits--;
            forwardForce = 8000f;
            effects.OSD.IsActive = false;
        }
    }

    //Brings Player to Spawn point
    private void Restart()
    {

        transform.position = new Vector3(0,1,0);
        transform.eulerAngles = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        effects.OSD.IsActive = true;

        forwardForce = effects.OSD.EffectFunction();
        sidewaysForce = 90f;
        jumpForce = 10f;

        obstacleHit = false;
    }

    public void IncreaseSteps()
    {
        stepCount += 1;
    }


    public void BackToRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = roomManager.gameEnded;
        PhotonNetwork.Destroy(PV);
        PhotonNetwork.Destroy(transform.parent.GetComponent<PhotonView>());

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }

        SceneManager.LoadScene("MultiplayerMenu");
    }

    public void Exit()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MultiplayerMenu");
        Launcher.Instance.SetPlayerDetails(PV.name);

        Destroy(roomManager.gameObject);
        Destroy(transform.parent);
    }

    public void TabPanel()
    {

        if (Input.GetKeyDown(KeyCode.Tab)) canvasManager.tabPanel.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Tab)) canvasManager.tabPanel.SetActive(false);

        canvasManager.playerCount.text = PhotonNetwork.PlayerList.Length.ToString();
    }

    public void EscPanel()
    {
        if( Input.GetKeyDown(KeyCode.Escape)) canvasManager.escPanel.SetActive(true);
        if( Input.GetKeyUp(KeyCode.Escape)) canvasManager.escPanel.SetActive(false);

    }



[PunRPC]
    public void DestroyStep(int viewID)
    {
        PhotonView stepToBeDestroyed = PhotonView.Find(viewID);

        if(stepToBeDestroyed)
        {
            PhotonNetwork.Destroy(stepToBeDestroyed);
        }
        else
        {
            return;
        }
    }

[PunRPC]
    public void DestroyDMerit(int viewID)
    {
        PhotonView dmeritToBeDestroyed = PhotonView.Find(viewID);
        if(dmeritToBeDestroyed)
        {
            PhotonNetwork.Destroy(dmeritToBeDestroyed);
        }
        else
        {
            return;
        }
    }

    //checks for triggers
    private void OnTriggerEnter(Collider collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Step")
        {
            IncreaseSteps();
            //PhotonNetwork.Destroy(collisionInfo.gameObject.GetPhotonView());
            //Destroy(collisionInfo.gameObject);

            PV.RPC(nameof(DestroyStep), RpcTarget.MasterClient , collisionInfo.GetComponent<PhotonView>().ViewID);
        }
        
        else if(collisionInfo.gameObject.tag == "DMerit")
        {
            DMerits++;
            PV.RPC(nameof(DestroyDMerit), RpcTarget.MasterClient, collisionInfo.GetComponent<PhotonView>().ViewID);

        }

        else if(collisionInfo.gameObject.name == "END" && roomManager.gameEnded == false)
        {
            roomManager.gameEnded = true;
            roomManager.CompleteLevel(gameObject);
            rb.velocity = Vector3.zero;
        }

        else
        {
            return;
        }
    }


#region Collision Detection System

    private void OnCollisionEnter(Collision collisionInfo) 
    {

        if(collisionInfo.gameObject == gameObject)
        {
            return;
        }

        if(collisionInfo.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }


        if (collisionInfo.collider.tag == "Obstacle" && obstacleHit == false)
        {
            obstacleHit = true;

            forwardForce = 0f;
            sidewaysForce = 0f;
            jumpForce = 0f;
            effects.OSD.ObstaclesHit++;
            Invoke(nameof(Restart), 1f);

            
        }

    }

    private void OnCollisionStay(Collision collisionInfo) 
    {
        if(collisionInfo.gameObject == gameObject)
        {
            return;
        }

        if(collisionInfo.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collisionInfo)
    {
        if(collisionInfo.gameObject == gameObject)
        {
            return;
        }
        isGrounded = false;
    }
#endregion
}
