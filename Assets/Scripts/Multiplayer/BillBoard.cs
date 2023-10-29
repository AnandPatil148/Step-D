using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class BillBoard : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] TMP_Text usernameText;
    [SerializeField] PhotonView playerPV;
     
    // Start is called before the first frame update
    void Start()
    {
        usernameText.text = playerPV.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        if(cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }

        if(cam == null)
        {
            return;
        }

        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);
    }
}
