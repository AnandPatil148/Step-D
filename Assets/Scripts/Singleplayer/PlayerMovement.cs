using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{  

    [Header("References")]
    public Rigidbody rb;
    public GameObject playerCam;
    public GameObject cubeBroken;
    public GameObject levelCompleteUI;
    public ScoreSingle scoreScript;
    public Effects effects;


    [Header("Input")]
    public bool wInput;
    public bool aInput;
    public bool sInput;
    public bool dInput;
    private bool qInput;

    [Header("Forces")]
    public float forwardForce;
    public float sidewaysForce;

    [Header("Jump")]
    //Jump stuff
    public float jumpForce;
    public bool isGrounded;

    [Header("Step")]
    //Step score
    public int  step;

    [Header("DMerits")]
    public int DMerits;

    [Header("Obstacle Hit")]
    // Obstacle hit
    public bool obstacleHit;

    public bool gameEnded = false;


    //look stuff
    public Vector3 offset;
    public Vector3 frontView;
    public Vector3 clampRotation;
    

    private void Update() 
    {
        Look();
        MyInput();
        EffectsCheck();

        if (rb.position.y < -1f && obstacleHit == false) Invoke(nameof(Restart),1f);

        if(isGrounded && Input.GetKeyDown(KeyCode.Space)) Jump();

    }

    private void FixedUpdate() 
    {
        Movement();    
    }

    //Gets Input From Keyboard
    private void MyInput()
    {

        //Get All Movement Related  Input From Keyboard
        wInput = Input.GetKey(KeyCode.W);
        aInput = Input.GetKey(KeyCode.A);
        sInput = Input.GetKey(KeyCode.S);
        dInput = Input.GetKey(KeyCode.D);

        //Get All Effects Related Input From Keyboard
        qInput = Input.GetKeyDown(KeyCode.Q);
    }
    

    //Applies Force To RigidBody
    private void Movement()
    {

        if(wInput) rb.AddForce(Vector3.forward * forwardForce * Time.deltaTime, ForceMode.Force);
        if(sInput) rb.AddForce(Vector3.back * forwardForce * Time.deltaTime, ForceMode.Force);
        if(aInput) rb.AddForce(Vector3.left * sidewaysForce * Time.deltaTime, ForceMode.VelocityChange);
        if(dInput) rb.AddForce(Vector3.right * sidewaysForce * Time.deltaTime, ForceMode.VelocityChange);
    }

    //Makes Player Jump
    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }


    //Camera Movement stuff
    private void Look()
    {
        playerCam.transform.position = transform.position + offset;
        playerCam.transform.eulerAngles = clampRotation;
        if(Input.GetKey("x")) //Back look
        {
            playerCam.transform.eulerAngles = new Vector3(0f,180f,0f);
            playerCam.transform.position = transform.position + frontView;
        }
        if(Input.GetKeyDown("z")) playerCam.GetComponent<Camera>().fieldOfView = 30; // Zoom in 
        if(Input.GetKeyUp("z")) playerCam.GetComponent<Camera>().fieldOfView = 60; // Zoom out
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
    
    //Restart Function
    private void Restart()
    {
        if(transform.Find("CubeBroken(Clone)"))
        {
            Destroy(transform.Find("CubeBroken(Clone)").gameObject);
            GetComponent<MeshRenderer>().enabled = true;
        }
        
        transform.position = Vector3.up;
        transform.eulerAngles = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        
        forwardForce = effects.OSD.EffectFunction();
        sidewaysForce = 90f;
        jumpForce = 10f;
        obstacleHit = false;

        effects.OSD.IsActive = true;
    }

    //Increaments score when gone thru a step
    public void UpdateScore()
    {
        step++;
    }

#region Collision Detection System

    private void OnTriggerEnter(Collider collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Step")
        {
            UpdateScore();
            Destroy(collisionInfo.gameObject);
        }

        else if(collisionInfo.gameObject.name == "END" && gameEnded == false)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().CompleteLevel();
            gameEnded = true;
        }
        else if (collisionInfo.gameObject.tag == "DMerit")
        {
            DMerits++;
            Destroy(collisionInfo.gameObject);
        }


    }
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
            //GetComponent<MeshRenderer>().enabled = false;
            //Instantiate(cubeBroken, transform.position, Quaternion.identity).transform.SetParent(transform);

            obstacleHit = true;

            effects.OSD.ObstaclesHit++;

            forwardForce = 0f;
            sidewaysForce = 0f;
            jumpForce = 0f;
            Invoke(nameof(Restart), 2f);
            
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
