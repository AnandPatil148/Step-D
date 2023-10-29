using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{

    [Header("Effect References")]
    public ObstacleSlowDownEffect OSD;

    // Start is called before the first frame update
    void Start()
    {
        OSD.IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

