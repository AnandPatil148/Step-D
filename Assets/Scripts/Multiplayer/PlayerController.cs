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
    public GameObject PM;
    public GameObject playerCam;
    //reference to Effects
    public Effects effects;

    //Canvas stuff
    public GameObject mainCanvas;
    public CanvasManagerMulti canvasManager;

    [Header("Input")]
    public bool wInput;
    public bool aInput;
    public bool sInput;
    public bool dInput;
    public bool qInput;
    public bool xInput;

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
    public int StepsCount;

    [Header("DMerits")]
    public int DMeritsCount;

    //obstacle hit
    public bool obstacleHit;
    //look stuff
    public Vector3 offset;
    public Vector3 frontView;
    public Vector3 clampRotation;

    public Vector3 SpawnPoint;

    public bool completeLvlCalled = false;

    private void Start() 
    {
        if(!PV.IsMine)
        {
            //playerCam.GetComponent<Camera>().enabled = false;
            //playerCam.GetComponent<AudioListener>().enabled = false;
            rb.isKinematic = true;
            rb.detectCollisions = true;
            playerCam.SetActive(false);
            mainCanvas.SetActive(false);
            //Destroy(rb);
        }
        else
        {
            canvasManager.roomName.text = PhotonNetwork.CurrentRoom.Name;
            PM = GameObject.Find("LocalPM");
            SpawnPoint = transform.position;
        }
        
        roomManager = RoomManager.Instance;
    }

    private void Update() 
    {   
        if(!PV.IsMine) return;
        
        MyInput();
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
        if(qInput && DMeritsCount != 0)
        {
            DMeritsCount--;
            forwardForce = 8000f;
            effects.OSD.IsActive = false;
        }
    }

    //Brings Player to Spawn point
    private void Restart()
    {

        transform.position = SpawnPoint;
        transform.eulerAngles = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        effects.OSD.IsActive = true;

        forwardForce = effects.OSD.EffectFunction();
        sidewaysForce = 60f;
        jumpForce = 10f;

        obstacleHit = false;
    }

    public void IncreaseSteps()
    {
        StepsCount += 1;
    }

    //Needs more dev
    public void LeaveRoomInGame()
    {
        PhotonNetwork.Destroy(PV);
        PhotonNetwork.Destroy(PM.GetPhotonView());
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MultiplayerMenu");
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

[PunRPC]
    public void RPCLvlComplete(string winnerName)
    {
        roomManager.CompleteLevel(winnerName);
    }

    public void DestroyPC()
    {
        PhotonNetwork.Destroy(PV);
    }

    //checks for triggers
    private void OnTriggerEnter(Collider other)
    {
            
        
        if(other.CompareTag("Step"))
        {
            IncreaseSteps();

            PV.RPC(nameof(DestroyStep), RpcTarget.MasterClient , other.GetComponent<PhotonView>().ViewID);
        }
        
        else if(other.CompareTag("DMerit"))
        {
            DMeritsCount++;
            PV.RPC(nameof(DestroyDMerit), RpcTarget.MasterClient, other.GetComponent<PhotonView>().ViewID);

        }

        else if(other.gameObject.name == "END")
        {
            if(PV.IsMine && !completeLvlCalled)
            {
                completeLvlCalled = true;
                PV.RPC(nameof(RPCLvlComplete), RpcTarget.All, PV.Owner.NickName);
                //Invoke(nameof(DestroyPC), 2);
            }

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
