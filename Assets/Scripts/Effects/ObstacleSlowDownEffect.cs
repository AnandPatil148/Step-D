using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EffectStd;


public class ObstacleSlowDownEffect : IEffect
{
    public int ObstaclesHit = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        EffectName = "ObstacleSlowDownEffect";
        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int EffectFunction()
    {
        
        if(IsActive)
        {
            return 8000 - (ObstaclesHit * 250);
        }

        return 8000;
    }
}
