using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObstacleSetType1 : MonoBehaviour
{
    public GameObject Parent;
    public GameObject Obstacle;
    public GameObject EmptyObstacle;
    public Vector3[] Positions;
    public int iOfEmptyObstacle;

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            iOfEmptyObstacle = Random.Range(0, Positions.Length);
            gameObject.GetPhotonView().RPC(nameof(InstantiateObstacles), RpcTarget.All, iOfEmptyObstacle);
        }

        
    }

[PunRPC]
    public void InstantiateObstacles(int iOfEmptyObstacle)
    {
    
        Instantiate(EmptyObstacle, Positions[iOfEmptyObstacle], Quaternion.identity).transform.SetParent(Parent.transform,false);
        for (int i = 0; i < Positions.Length; i++)
        {
            if(i == iOfEmptyObstacle)
            {
                continue;
            }

            Instantiate(Obstacle, Positions[i], Quaternion.identity).transform.SetParent(Parent.transform,false);
        }
    }

}
