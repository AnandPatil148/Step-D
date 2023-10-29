using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //private references
    public GameObject step;
    public GameObject player;
    public GameObject DMerit;
    [SerializeField] PlayerMovement pm;
    [SerializeField] int luckA, luckB, luck;

    public int stepCount;

    //Game logic stuff
    public float restartDelay = 1f;

    private void Start() 
    {
        
        stepCount = SceneManager.GetActiveScene().buildIndex + 2;
        for (int i = 0; i < stepCount; i++)
        {
            Instantiate(step,   new Vector3(Random.Range(-5, 5), 1, Random.Range(10, 150)),  Quaternion.identity);
        } 

        if( Random.Range(luckA, luckB) == luck)
        {
            Instantiate(DMerit, new Vector3(Random.Range(-5, 5), 1, Random.Range(10, 150)), Quaternion.identity );
        }
        
        pm = Instantiate(player, Vector3.up , Quaternion.identity).GetComponent<PlayerMovement>();
    }
    
    public void CompleteLevel()
    {

        pm.levelCompleteUI.SetActive(true);
        pm.scoreScript.enabled = false;
        pm.rb.velocity = Vector3.zero;
        pm.enabled = false;

    }

}
